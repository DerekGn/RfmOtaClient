
using System;
using System.IO.Ports;

namespace RfmOta.Ports
{
    internal class SerialPort : ISerialPort
    {
        private readonly System.IO.Ports.SerialPort _serialPort;

        public SerialPort(string serialPort)
        {
            if(string.IsNullOrWhiteSpace(serialPort))
            {
                throw new ArgumentOutOfRangeException(nameof(serialPort));
            }

            _serialPort = new System.IO.Ports.SerialPort
            {
                PortName = serialPort,
            };

            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            _serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorReceivedHandler);
        }

        public bool IsOpen => _serialPort != null && _serialPort.IsOpen;

        public void Open()
        {
            _serialPort.Open();
        }

        public void Close()
        {
            _serialPort.Close();
        }
        public void Write(string text)
        {
            _serialPort.Write(text);
        }

        private void ErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e)
        {
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (System.IO.Ports.SerialPort)sender;
        }

        #region
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
        // ~SerialPort()
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
