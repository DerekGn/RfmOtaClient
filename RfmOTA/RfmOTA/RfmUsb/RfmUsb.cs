using Microsoft.Extensions.Logging;
using RfmOta.Ports;
using System;
using System.Collections.Generic;

namespace RfmOta.RfmUsb
{
    internal class RfmUsb : IRfmUsb
    {
        private readonly ISerialPortFactory _serialPortFactory;
        private readonly ILogger<IRfmUsb> _logger;

        public RfmUsb(ILogger<IRfmUsb> logger, ISerialPortFactory serialPortFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serialPortFactory = serialPortFactory ?? throw new ArgumentNullException(nameof(serialPortFactory));
        }

        public byte PayloadLenght { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool VariableLenght { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public EnterCondition EnterCondition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IntermediateMode IntermediateMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ExitCondition ExitCondition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public byte FifoThreshold { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool TxStartCondition { get; set; }
        public int RetryCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Timeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Open(string serialPort)
        {
        }

        public void Close()
        {
        }
        public IList<byte> Transmit(IList<byte> data)
        {
            throw new NotImplementedException();
        }

        private bool TryCreateSerialPortInstance(string serialPort, out ISerialPort instance)
        {
            instance = _serialPortFactory.CreateSerialPortInstance(serialPort);

            return instance != null;
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
