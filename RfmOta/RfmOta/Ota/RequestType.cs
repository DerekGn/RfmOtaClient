namespace RfmOta.Ota
{
    internal enum RequestType
    {
        Crc,
        Ping,
        Erase,
        Reboot,
        WriteEnd,
        FlashSize,
        WriteStart
    };
}
