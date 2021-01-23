using System;

namespace RfmOta.Ota.Exceptions
{

    [Serializable]
    public class OtaException : Exception
    {
        public OtaException() { }
        public OtaException(string message) : base(message) { }
        public OtaException(string message, Exception inner) : base(message, inner) { }
        protected OtaException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
