using System;
using System.Collections.Generic;
using System.Text;

namespace RfmOta.Rfm.Exceptions
{

    [Serializable]
    public class RfmUsbCommandExecutionException : Exception
    {
        public RfmUsbCommandExecutionException() { }
        public RfmUsbCommandExecutionException(string message) : base(message) { }
        public RfmUsbCommandExecutionException(string message, Exception inner) : base(message, inner) { }
        protected RfmUsbCommandExecutionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
