using Microsoft.Extensions.Logging;
using RfmOta.Ports;
using System;
using System.Collections.Generic;
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
            set { 
                _payloadLenght = value;
                SendWithAck($"s-pl {_payloadLenght:0xFF}");
            }
        }
        public bool VariableLenght
        { 
            get => _variableLenght;
            set
            {
                _variableLenght = value;
                SendWithAck($"s-pf {_variableLenght}");
            }
        }
        public EnterCondition EnterCondition
        {
            get => _enterCondition;
            set
            {
                _enterCondition = value;
                SendWithAck($"s-amec {_enterCondition}");
            }
        }
        public IntermediateMode IntermediateMode 
        {
            get => _intermediateMode;
            set 
            {
                _intermediateMode = value;
                SendWithAck($"s-im {_intermediateMode}");
            }
        }
        public ExitCondition ExitCondition
        {
            get => _exitCondition;
            set
            {
                _exitCondition = value;
                SendWithAck($"s-amexc {_exitCondition}");
            }
        }
        public byte FifoThreshold
        {
            get => _fifoThreshold;
            set
            {
                _fifoThreshold = value;
                SendWithAck($"s-ft {_fifoThreshold}");
            }
        }
        public bool TxStartCondition
        {
            get => _txStartCondition;
            set
            {
                _txStartCondition = value;
                SendWithAck($"s-tsc {_txStartCondition}");
            }
        }
        public int RetryCount { get; set; }
        public int Timeout { get ; set; }
        public void Open(string serialPort)
        {
            if(_serialPort == null)
            {
                _serialPort = _serialPortFactory.CreateSerialPortInstance(serialPort);

                _serialPort.Open();
            }
        }
        public void Close()
        {
            if(_serialPort != null && _serialPort.IsOpen)
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

        private void SendWithAck(string text)
        {
            _serialPort.Write(text);

            _autoResetEvent.WaitOne();
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
