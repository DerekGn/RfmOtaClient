
namespace RfmOta.Ota
{
    internal enum ResponseType
    {
        Crc = 0x80,
        Ping = 0x81,
        Erase = 0x82,
        FlashSize = 0x83,
        InvalidLocked = 0x84,
        InvalidLength = 0x85,
        InvalidAddress = 0x86
    };
}
