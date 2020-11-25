namespace RfmOta.Rfm
{
    internal enum EnterCondition
    {
        None,
        FifoNotEmpty,
        FifoLevel,
        CrcOk,
        PayloadReady,
        SyncAddress,
        PacketSent,
        FifoEmpty
    }
}