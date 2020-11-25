using System;
using System.IO;

namespace RfmOta.Ota
{
    interface IOtaService : IDisposable
    {
        bool OtaUpdate(Options options, Stream stream, out uint crc);
    }
}
