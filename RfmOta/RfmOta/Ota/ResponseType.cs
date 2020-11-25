
namespace RfmOta.Ota
{
    internal enum ResponseType
    {
        Ok,
        Crc,
        FlashSize,
        InvalidLength,
        InvalidAddress,
        InvalidLocked
    };
}
