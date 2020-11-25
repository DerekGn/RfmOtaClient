namespace RfmOta.Rfm
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