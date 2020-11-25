namespace RfmOta.Ota
{
    internal enum RequestType
    {
        Crc,
        Ping,
        Erase,
        Reboot,
        Write,
        FlashSize
    };
}
