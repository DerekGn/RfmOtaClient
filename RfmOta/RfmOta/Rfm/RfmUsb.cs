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

using Microsoft.Extensions.Logging;
using RfmOta.Ports;
using RfmOta.Rfm.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RfmOta.Rfm
{
    internal class RfmUsb : IRfmUsb
    {
        private const string ResponseOk = "OK";

        private readonly ISerialPortFactory _serialPortFactory;
        private readonly ILogger<IRfmUsb> _logger;

        private ISerialPort _serialPort;

        public RfmUsb(ILogger<IRfmUsb> logger, ISerialPortFactory serialPortFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serialPortFactory = serialPortFactory ?? throw new ArgumentNullException(nameof(serialPortFactory));
        }
        ///<inheritdoc/>
        public string Version => SendCommand("g-fv");
        ///<inheritdoc/>
        public byte PayloadLenght
        {
            get => SendCommand("g-pl").ToBytes().First();
            set => SendCommandWithCheck($"s-pl 0x{value:X}", ResponseOk);
        }
        ///<inheritdoc/>
        public bool VariableLenght
        {
            get => SendCommand("g-pf").ToBytes().First() == 1;
            set => SendCommandWithCheck($"s-pf 0x0{(value ? 0x01 : 0x00):X}", ResponseOk);
        }
        ///<inheritdoc/>
        public byte FifoThreshold
        {
            get => SendCommand("g-ft").ToBytes().First();
            set => SendCommandWithCheck($"s-ft 0x{value:X}", ResponseOk);
        }
        ///<inheritdoc/>
        public byte DioInterruptMask
        {
            get => SendCommand("g-di").ToBytes().First();
            set => SendCommandWithCheck($"s-di 0x{value:X}", ResponseOk);
        }
        ///<inheritdoc/>
        public int RetryCount { get; set; }
        ///<inheritdoc/>
        public int Timeout
        {
            get => _serialPort.ReadTimeout;
            set
            {
                _serialPort.ReadTimeout = value;
                _serialPort.WriteTimeout = value;
            }
        }
        ///<inheritdoc/>
        public bool TransmissionStartCondition
        {
            get => SendCommand("g-tsc").ToBytes().First() == 1;
            set => SendCommandWithCheck($"s-tsc 0x0{(value ? 0x01 : 0x00):X}", ResponseOk);
        }
        ///<inheritdoc/>
        public byte RadioConfig
        {
            get => SendCommand($"g-rc").ToBytes().First();
            set => SendCommandWithCheck($"s-rc {value}", ResponseOk);
        }
        ///<inheritdoc/>
        public IEnumerable<byte> Sync
        {
            get => SendCommand($"g-sync").ToBytes();
            set => SendCommandWithCheck($"s-sync {BitConverter.ToString(value.ToArray()).Replace("-", string.Empty)}", ResponseOk);
        }
        ///<inheritdoc/>
        public int OutputPower
        {
            get => int.Parse(SendCommand($"g-op"));
            set => SendCommandWithCheck($"s-op {value}", ResponseOk);
        }
        ///<inheritdoc/>
        public void Open(string serialPort)
        {
            try
            {
                if (_serialPort == null)
                {
                    _serialPort = _serialPortFactory.CreateSerialPortInstance(serialPort);

                    _serialPort.BaudRate = 28800;
                    _serialPort.NewLine = "\r\n";
                    _serialPort.DtrEnable = true;
                    _serialPort.RtsEnable = true;
                    _serialPort.ReadTimeout = 500;
                    _serialPort.WriteTimeout = 500;
                    _serialPort.Open();
                }
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogDebug(ex, "Exception occurred opening serial port");

                throw new RfmUsbSerialPortNotFoundException(
                    $"Unable to open serial port [{serialPort}] Reason: [{ex.Message}]. " +
                    $"Available Serial Ports: [{string.Join(", ", _serialPortFactory.GetSerialPorts())}]");
            }
        }
        ///<inheritdoc/>
        public void Close()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }
        ///<inheritdoc/>
        public void Reset()
        {
            SendCommandWithCheck($"e-r", ResponseOk);
        }
        ///<inheritdoc/>
        public void Send(IList<byte> data)
        {
            _serialPort.Write($"{BitConverter.ToString(data.ToArray()).Replace("-", string.Empty)}");
        }
        ///<inheritdoc/>
        public IList<byte> TransmitReceive(IList<byte> data, int timeout)
        {
            int retries = RetryCount;

            do
            {
                var response = SendCommand($"e-txrx {BitConverter.ToString(data.ToArray()).Replace("-", string.Empty)} {timeout}");

                if (response.Contains("TX") || response.Contains("RX"))
                {
                    if (retries == 0)
                    {
                        throw new RfmUsbTransmitException($"Packet transmission failed: [{response}]");
                    }

                    retries--;
                }
                else
                {
                    return response.ToBytes();
                }

            } while (true);
        }
        ///<inheritdoc/>
        public void Transmit(IList<byte> data, int timeout)
        {
            SendCommand($"e-tx {BitConverter.ToString(data.ToArray()).Replace("-", string.Empty)} {timeout}");
        }
        ///<inheritdoc/>
        public void SetDioMapping(Dio dio, DioMapping mapping)
        {
            SendCommandWithCheck($"s-dio {(int)dio} {(int)mapping}", "[0x0001]-Map 01");
        }
        private string SendCommand(string command)
        {
            _serialPort.Write($"{command}\n");

            var response = _serialPort.ReadLine();

            _logger.LogDebug($"Command: [{command}] Result: [{response}]");

            return response;
        }
        private void SendCommandWithCheck(string command, string response)
        {
            var result = SendCommand(command);

            if (!result.StartsWith(response))
            {
                throw new RfmUsbCommandExecutionException($"Command: [{command}] Execution Failed Reason: [{result}]");
            }
        }

        #region IDisposible
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_serialPort != null)
                    {
                        _serialPort.Close();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~RfmUsb()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }

        #endregion
    }
}
