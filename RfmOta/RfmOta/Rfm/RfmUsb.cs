using Microsoft.Extensions.Logging;
using RfmOta.Ports;
using RfmOta.Rfm.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace RfmOta.Rfm
{
    internal class RfmUsb : IRfmUsb
    {
        private readonly ISerialPortFactory _serialPortFactory;
        private readonly AutoResetEvent _autoResetEvent;
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

        public RfmUsb(ILogger<IRfmUsb> logger, ISerialPortFactory serialPortFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serialPortFactory = serialPortFactory ?? throw new ArgumentNullException(nameof(serialPortFactory));

            _autoResetEvent = new AutoResetEvent(false);
        }

        public byte PayloadLenght
        {
            get => _payloadLenght;
            set
            {
                _payloadLenght = value;
                SendCommand($"s-pl 0x{_payloadLenght:X}");
            }
        }
        public bool VariableLenght
        {
            get => _variableLenght;
            set
            {
                _variableLenght = value;
                SendCommand($"s-pf {{_variableLenght ? 1 : 0}}");
            }
        }
        public EnterCondition EnterCondition
        {
            get => _enterCondition;
            set
            {
                _enterCondition = value;
                SendCommand($"s-amec 0x{((byte)_enterCondition):X}");
            }
        }
        public IntermediateMode IntermediateMode
        {
            get => _intermediateMode;
            set
            {
                _intermediateMode = value;
                SendCommand($"s-im {_intermediateMode}");
            }
        }
        public ExitCondition ExitCondition
        {
            get => _exitCondition;
            set
            {
                _exitCondition = value;
                SendCommand($"s-amexc {_exitCondition}");
            }
        }
        public byte FifoThreshold
        {
            get => _fifoThreshold;
            set
            {
                _fifoThreshold = value;
                SendCommand($"s-ft {_fifoThreshold}");
            }
        }
        public bool TxStartCondition
        {
            get => _txStartCondition;
            set
            {
                _txStartCondition = value;
                SendCommand($"s-tsc {_txStartCondition}");
            }
        }
        public int RetryCount { get; set; }
        public int Timeout { get => _serialPort.ReadTimeout; set => _serialPort.ReadTimeout = value; }
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
                    _serialPort.DataReceived += SerialPortDataReceived;
                    _serialPort.ErrorReceived += SerialPortErrorReceived;
                    _serialPort.PinChanged += SerialPortPinChanged;
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
        public IList<byte> SendAwait(IList<byte> data)
        {
            List<byte> request = new List<byte>
            {
                (byte)data.Count
            };
            request.AddRange(data);

            return new List<byte>();
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
        private void SendCommand(string command)
        {
            _serialPort.Write($"{command}\r\n");

            var result = _serialPort.ReadLine();

            if(result != "OK")
            {
                throw new RfmUsbCommandExecutionException($"Command [{command}] Execution Failed Reason: [{result}]");
            }
        }

        private void SerialPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            _serialError = e.EventType;

            _autoResetEvent.Set();
        }

        private void SerialPortPinChanged(object sender, SerialPinChangedEventArgs e)
        {
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _autoResetEvent.Set();
        }

        #region IDisposible
        private bool disposedValue;
        
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
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
