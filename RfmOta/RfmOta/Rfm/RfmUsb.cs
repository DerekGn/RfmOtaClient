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

        private IntermediateMode _intermediateMode;
        private EnterCondition _enterCondition;
        private ExitCondition _exitCondition;
        private ISerialPort _serialPort;
        private bool _txStartCondition;
        private bool _variableLenght;
        private byte _payloadLenght;
        private byte _fifoThreshold;
        private string _version;

        public RfmUsb(ILogger<IRfmUsb> logger, ISerialPortFactory serialPortFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serialPortFactory = serialPortFactory ?? throw new ArgumentNullException(nameof(serialPortFactory));
        }

        public string Version
        {
            get
            {
                if (string.IsNullOrEmpty(_version))
                {
                    _version = SendCommand("g-fv");
                }

                return _version;
            }
        }
        public byte PayloadLenght
        {
            get => _payloadLenght;
            set
            {
                _payloadLenght = value;
                SendCommandWithCheck($"s-pl 0x{_payloadLenght:X}", ResponseOk);
            }
        }
        public bool VariableLenght
        {
            get => _variableLenght;
            set
            {
                _variableLenght = value;
                SendCommandWithCheck($"s-pf 0x0{(_variableLenght ? 0x01 : 0x00):X}", ResponseOk);
            }
        }
        public EnterCondition EnterCondition
        {
            get => _enterCondition;
            set
            {
                _enterCondition = value;
                SendCommandWithCheck($"s-amec 0x0{(byte)_enterCondition:X}", "[0x0006]-Packet Sent");
            }
        }
        public IntermediateMode IntermediateMode
        {
            get => _intermediateMode;
            set
            {
                _intermediateMode = value;
                SendCommandWithCheck($"s-im 0x0{(byte)_intermediateMode:X}", "[0x0002]-Rx");
            }
        }
        public ExitCondition ExitCondition
        {
            get => _exitCondition;
            set
            {
                _exitCondition = value;
                SendCommandWithCheck($"s-amexc 0x0{(byte)_exitCondition:X}", "[0x0003]-Crc Ok");
            }
        }
        public byte FifoThreshold
        {
            get => _fifoThreshold;
            set
            {
                _fifoThreshold = value;
                SendCommandWithCheck($"s-ft 0x{_fifoThreshold:X}", ResponseOk);
            }
        }
        public bool TxStartCondition
        {
            get => _txStartCondition;
            set
            {
                _txStartCondition = value;
                SendCommandWithCheck($"s-tsc 0x{(_txStartCondition ? 0x01 : 0x00)}", ResponseOk);
            }
        }
        public int RetryCount { get; set; }
        public int Timeout
        {
            get => _serialPort.ReadTimeout;
            set
            {
                _serialPort.ReadTimeout = value;
                _serialPort.WriteTimeout = value;
            }
        }
        public void Open(string serialPort)
        {
            try
            {
                if (_serialPort == null)
                {
                    _serialPort = _serialPortFactory.CreateSerialPortInstance(serialPort);

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

        public void Close()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }
        public void Send(IList<byte> data)
        {
            List<byte> request = new List<byte>
            {
                (byte)data.Count
            };
            request.AddRange(data);

            _serialPort.Write($"{BitConverter.ToString(data.ToArray())}");
        }
        public IList<byte> SendAwait(IList<byte> data)
        {
            List<byte> request = new List<byte>
            {
                (byte)data.Count
            };
            request.AddRange(data);

            return new List<byte>();
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
                    if(_serialPort != null)
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
