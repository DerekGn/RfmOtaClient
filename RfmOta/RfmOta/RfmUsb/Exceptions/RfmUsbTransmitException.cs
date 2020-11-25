namespace RfmOta.RfmUsb.Exceptions
{

    [System.Serializable]
    public class RfmUsbTransmitException : System.Exception
    {
        public RfmUsbTransmitException() { }
        public RfmUsbTransmitException(string message) : base(message) { }
        public RfmUsbTransmitException(string message, System.Exception inner) : base(message, inner) { }
        protected RfmUsbTransmitException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
