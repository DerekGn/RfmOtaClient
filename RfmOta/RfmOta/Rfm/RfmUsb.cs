﻿/*
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
using System.IO.Ports;
using System.Linq;

namespace RfmOta.Rfm
{
    internal class RfmUsb : IRfmUsb
    {
        private readonly ISerialPortFactory _serialPortFactory;
        private readonly ILogger<IRfmUsb> _logger;

        private IntermediateMode _intermediateMode;
        private EnterCondition _enterCondition;
        private ExitCondition _exitCondition;
        private SerialError _serialError;
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
                SendCommandWithCheck($"s-pl 0x{_payloadLenght:X}");
            }
        }
        public bool VariableLenght
        {
            get => _variableLenght;
            set
            {
                _variableLenght = value;
                SendCommandWithCheck($"s-pf {{_variableLenght ? 1 : 0}}");
            }
        }
        public EnterCondition EnterCondition
        {
            get => _enterCondition;
            set
            {
                _enterCondition = value;
                SendCommandWithCheck($"s-amec 0x{(byte)_enterCondition:X}");
            }
        }
        public IntermediateMode IntermediateMode
        {
            get => _intermediateMode;
            set
            {
                _intermediateMode = value;
                SendCommandWithCheck($"s-im {_intermediateMode}");
            }
        }
        public ExitCondition ExitCondition
        {
            get => _exitCondition;
            set
            {
                _exitCondition = value;
                SendCommandWithCheck($"s-amexc {_exitCondition}");
            }
        }
        public byte FifoThreshold
        {
            get => _fifoThreshold;
            set
            {
                _fifoThreshold = value;
                SendCommandWithCheck($"s-ft {_fifoThreshold}");
            }
        }
        public bool TxStartCondition
        {
            get => _txStartCondition;
            set
            {
                _txStartCondition = value;
                SendCommandWithCheck($"s-tsc {{_txStartCondition ? 1 : 0}}");
            }
        }
        public int RetryCount { get; set; }
        public int Timeout
        {
            get => _serialPort.ReadTimeout;
            set => _serialPort.ReadTimeout = value;
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
#warning TODO cleanup in dispose
                    _serialPort.ErrorReceived += SerialPortErrorReceived;
                    _serialPort.PinChanged += SerialPortPinChanged;
                    _serialPort.ReadTimeout = 500;
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
            _serialPort.Write($"{command}\r\n");

            return _serialPort.ReadLine();
        }
        private void SendCommandWithCheck(string command)
        {
            var result = SendCommand(command);

            if (!result.StartsWith("OK"))
            {
                throw new RfmUsbCommandExecutionException($"Command [{command}] Execution Failed Reason: [{result}]");
            }
        }
        private void SerialPortPinChanged(object sender, SerialPinChangedEventArgs e)
        {
        }
        private void SerialPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            _serialError = e.EventType;
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
