/*
* MIT License
*
* Copyright (c) 2020 Derek Goslin http://corememorydump.blogspot.ie/
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

using HexIO;
using Microsoft.Extensions.Logging;
using RfmOta.Rfm;
using RfmOta.Rfm.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RfmOta.Ota
{
    internal class OtaService : IOtaService
    {
        private readonly ILogger<IOtaService> _logger;
        private readonly List<Func<bool>> _steps;
        private readonly IRfmUsb _rfmUsb;

        private uint _startAddress;
        private uint _flashSize;
        private uint _writeSize;
        private Stream _stream;
        private uint _crc;

        public OtaService(ILogger<IOtaService> logger, IRfmUsb rfmUsb)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _rfmUsb = rfmUsb ?? throw new ArgumentNullException(nameof(rfmUsb));

            _steps = new List<Func<bool>>
            {
                () => PingBootLoader(),
                () => GetFlashSize(),
                () => EraseFlash(),
                () => SendHexData(),
                () => GetCrc(),
                () => Reboot()
            };
        }

        public bool OtaUpdate(Options options, Stream stream, out uint crc)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _stream = stream ?? throw new ArgumentNullException(nameof(stream));

            bool result = true;

            try
            {
                crc = 0;
                InitaliseRfmUsb(options);

                foreach (var step in _steps)
                {
                    if (!step()) 
                    {
                        result = false;
                        break;
                    }
                }

                crc = _crc;
            }
            finally
            {
                _rfmUsb?.Close();
            }

            return result;
        }

        private bool GetCrc()
        {
            return HandleRfmUsbOperation(
                nameof(OtaService),
                () =>
                {
                    if (!SendAndValidateResponse(
                        new List<byte>() { 0x01, (byte)RequestType.Crc },
                        PayloadSizes.CrcResponse, ResponseType.Crc, out IList<byte> response))
                    {
                        return false;
                    }

                    _crc = BitConverter.ToUInt32(response.ToArray(), 1);
                    _logger.LogInformation($"Flash Crc: [0x{_crc:X}]");

                    return true;
                });
        }

        private bool Reboot()
        {
            return HandleRfmUsbOperation(
                nameof(OtaService),
                () =>
                {
                    _rfmUsb.Send(new List<byte>() { 0x01, (byte)RequestType.Reboot });

                    return true;
                });
        }

        private bool SendHexData()
        {
            return HandleRfmUsbOperation(
                nameof(OtaService),
                () =>
                {
                    using IntelHexReader hexReader = new IntelHexReader(_stream);

                    while (hexReader.Read(out uint address, out IList<byte> data))
                    {
                        _logger.LogInformation($"Writing Address: [0x{address:X}] Count: [0x{data.Count:X2}]" +
                            $" Data: [{BitConverter.ToString(data.ToArray()).Replace("-", "")}]");

                        var requestData = new List<byte>() { (byte)RequestType.Write };
                        requestData.AddRange(BitConverter.GetBytes(address));
                        requestData.AddRange(data);

                        if (!SendAndValidateResponse(
                            new List<byte>() { 0x01, (byte)RequestType.Write },
                            PayloadSizes.OkResponse, ResponseType.Ok, out IList<byte> response))
                        {
                            return false;
                        }
                    }

                    return true;
                });
        }

        private bool EraseFlash()
        {
            return HandleRfmUsbOperation(
                nameof(OtaService),
                () =>
                {
                    if (!SendAndValidateResponse(
                        new List<byte>() { 0x01, (byte)RequestType.Erase },
                        PayloadSizes.EraseResponse, ResponseType.Erase, out IList<byte> response))
                    {
                        return false;
                    }

                    _logger.LogInformation("BootLoader Erase Ok");

                    return true;
                });
        }

        private bool GetFlashSize()
        {
            return HandleRfmUsbOperation(
                nameof(OtaService),
                () =>
                {
                    if (!SendAndValidateResponse(
                        new List<byte>() { 0x01, (byte)RequestType.FlashSize },
                        PayloadSizes.FlashSizeResponse, ResponseType.FlashSize, out IList<byte> response))
                    {
                        return false;
                    }

                    _startAddress = BitConverter.ToUInt32(response.ToArray(), 2);
                    _flashSize = BitConverter.ToUInt32(response.ToArray(), 6);
                    _writeSize = BitConverter.ToUInt32(response.ToArray(), 10);
                    _logger.LogInformation($"App Start Address: [0x{_startAddress:X}] Flash Size: [0x{_flashSize:X}] Write Size: [0x{_writeSize:X}]");

                    return true;
                });
        }

        private bool PingBootLoader()
        {
            return HandleRfmUsbOperation(
                nameof(OtaService),
                () =>
                {
                    if (!SendAndValidateResponse(
                        new List<byte>() { 0x01, (byte)RequestType.Ping },
                        PayloadSizes.PingResponse, ResponseType.Ping, out IList<byte> response))
                    {
                        return false;
                    }

                    _logger.LogInformation("BootLoader Ping Ok");

                    return true;
                });
        }

        private bool SendAndValidateResponse(IList<byte> request,
            int expectedSize, ResponseType expectedResponse,
            out IList<byte> response, [CallerMemberName] string memberName = "")
        {
            response = _rfmUsb.SendAwait(request);

            if (response.Count == 0 || response.Count < expectedSize)
            {
                _logger.LogError($"Invalid response received [{BitConverter.ToString(response.ToArray())}]");

                return false;
            }

            if (response[0] != (byte)expectedSize)
            {
                _logger.LogInformation($"BootLoader Invalid {memberName} Response Length: [{response[0]}]");

                return false;
            }

            if (response[1] != (byte)expectedResponse)
            {
                _logger.LogInformation($"BootLoader Invalid {memberName} Response: [0x{response[1]:X}]");

                return false;
            }

            _logger.LogInformation($"BootLoader {memberName} Ok");

            return true;
        }

        private bool HandleRfmUsbOperation(string className, Func<bool> operation, [CallerMemberName] string memberName = "")
        {
            Stopwatch sw = new Stopwatch();
            bool result;

            try
            {
                sw.Start();
                result = operation();
                sw.Stop();

                _logger.LogDebug($"Executed [{className}].[{memberName}] in [{sw.Elapsed.TotalMilliseconds}] ms");
            }
            catch (RfmUsbTransmitException ex)
            {
                _logger.LogError($"A transmission exception occurred executing [{className}].[{memberName}] Reason: [{ex.Message}]");
                _logger.LogDebug(ex, $"A transmission exception occurred executing [{className}].[{memberName}]");

                return false;
            }

            return result;
        }

        private void InitaliseRfmUsb(Options options)
        {
            _rfmUsb.Open(options.SerialPort);

            _rfmUsb.Reset();

            _logger.LogInformation(_rfmUsb.Version);

            _rfmUsb.VariableLenght = true;

            _rfmUsb.FifoThreshold = 0x01;

            _rfmUsb.SetDioMapping(Dio.Dio1, DioMapping.DioMapping1);

            _rfmUsb.DioInterruptMask = 0x01;

            _rfmUsb.RetryCount = options.RetryCount;

            //_rfmUsb.Timeout = options.Timeout;
        }

        #region
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _rfmUsb?.Close();
                    _rfmUsb?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~OtaService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
