using System;

namespace RfmOta.Rfm.Exceptions
{

    [Serializable]
    public class RfmUsbSerialPortNotFoundException : Exception
    {
        public RfmUsbSerialPortNotFoundException() { }
        public RfmUsbSerialPortNotFoundException(string message) : base(message) { }
        public RfmUsbSerialPortNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected RfmUsbSerialPortNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
