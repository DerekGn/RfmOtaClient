using RfmOta.Ota;
using System.Collections.Generic;

namespace RfmOta.UnitTests.Ota
{
    public static class TestResponses
    {
        public static List<byte> Empty = new List<byte>();

        public static List<byte> PingOk = new List<byte>() { (byte)ResponseType.Ping };

        public static List<byte> FlashSizeOk = new List<byte>() { (byte)ResponseType.FlashSize, 0xAA, 0x55, 0xAA, 0x55 };

        public static List<byte> EraseOk = new List<byte>() { (byte)ResponseType.Erase };
    }
}
