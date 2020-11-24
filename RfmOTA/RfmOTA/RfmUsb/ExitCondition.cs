namespace RfmOta.RfmUsb
{
    internal enum ExitCondition
    {
        None,
        FifoEmpty,
        FifoLevelTimeout,
        CrcOkTimeout,
        PayloadReadyTimeout,
        SyncAddressTimeout,
        PacketSent,
        Timeout
    }
}