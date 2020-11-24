namespace RfmOta.RfmUsb
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