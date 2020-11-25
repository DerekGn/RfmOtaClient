using HexIO;
using Microsoft.Extensions.Logging;
using RfmOta.RfmUsb;
using RfmOta.RfmUsb.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RfmOta.Ota
{
    internal class OtaService : IOtaService
    {
        private readonly ILogger<IOtaService> _logger;
        private readonly List<Func<bool>> _steps;
        private readonly IRfmUsb _rfmUsb;

        private uint _flashSize;
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

            _stream = stream;

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
                () => { return true; });
        }

        private bool Reboot()
        {
            return HandleRfmUsbOperation(
                nameof(OtaService),
                () => { return true; });
        }

        private bool SendHexData()
        {
            return HandleRfmUsbOperation(
                nameof(OtaService),
                () =>
                {
                    //using IntelHexReader hexReader = new IntelHexReader(stream);

                    //while (hexReader.Read(out uint address, out IList<byte> data))
                    //{
                    //    _logger.LogInformation($"Writing Address: [0x{address:X4}] Count: [0x{data.Count:X2}]" +
                    //        $" Data: [{BitConverter.ToString(data.ToArray()).Replace("-", "")}]");

                    //    var responseData = _rfmUsb.Transmit(new List<byte>());
                    //}
                    return true;
                });
        }

        private bool EraseFlash()
        {
            return HandleRfmUsbOperation(
                nameof(OtaService),
                () => { return true; });
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

                    _flashSize = BitConverter.ToUInt32(response.ToArray(), 1);
                    _logger.LogInformation($"Flash Size: [{_flashSize}]");

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

                    if (response[0] != (byte)ResponseType.Ping)
                    {
                        _logger.LogInformation($"BootLoader Invalid ping Reponse: [{response[0]}]");

                        return false;
                    }

                    _logger.LogInformation("BootLoader Ping Ok");

                    return true;
                });
        }

        private bool SendAndValidateResponse(IList<byte> request, int expectedSize, ResponseType expectedResponse, out IList<byte> response)
        {
            response = _rfmUsb.Transmit(request);

            if (!(response.Count > 0 && response.Count < expectedSize))
            {
                _logger.LogError($"Invalid response received [{BitConverter.ToString(response.ToArray())}]");

                return false;
            }

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

            _rfmUsb.PayloadLenght = 0xFF;

            _rfmUsb.VariableLenght = true;

            _rfmUsb.EnterCondition = EnterCondition.PacketSent;

            _rfmUsb.IntermediateMode = IntermediateMode.Rx;

            _rfmUsb.ExitCondition = ExitCondition.CrcOkTimeout;

            _rfmUsb.FifoThreshold = 0x02;

            _rfmUsb.TxStartCondition = false;

            _rfmUsb.RetryCount = options.RetryCount;

            _rfmUsb.Timeout = options.Timeout;
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
