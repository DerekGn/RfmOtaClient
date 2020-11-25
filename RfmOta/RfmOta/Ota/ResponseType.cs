
namespace RfmOta.Ota
{
    internal enum ResponseType
    {
        Ok = 0x80,
        Crc = 0x81,
        Ping = 0x82,
        Erase = 0x83,
        FlashSize = 0x84,
        InvalidLocked = 0x85,
        InvalidLength = 0x86,
        InvalidAddress = 0x87
    };
}
