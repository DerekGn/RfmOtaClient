namespace RfmOta.Ota
{
    internal enum RequestType
    {
        Crc,
        Erase,
        Reboot,
        WriteEnd,
        FlashSize,
        WriteStart
    };
}
