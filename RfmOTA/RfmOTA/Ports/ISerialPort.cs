using System;

namespace RfmOta.Ports
{
    internal interface ISerialPort : IDisposable
    {
        bool IsOpen { get; }

        void Open();

        void Close();
    }
}
