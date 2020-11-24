using System;

namespace RfmOta.Ota
{
    interface IOtaService : IDisposable
    {
        void OtaUpdate(string hexFile, string serialPort);
    }
}
