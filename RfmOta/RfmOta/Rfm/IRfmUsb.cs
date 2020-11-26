using System;
using System.Collections.Generic;

namespace RfmOta.Rfm
{
    internal interface IRfmUsb : IDisposable
    {
        byte PayloadLenght { get; set; }
        bool VariableLenght { get; set; }
        EnterCondition EnterCondition { get; set; }
        IntermediateMode IntermediateMode { get; set; }
        ExitCondition ExitCondition { get; set; }
        byte FifoThreshold { get; set; }
        public bool TxStartCondition { get; set; }
        public int RetryCount { get; set; }
        public int Timeout { get; set; }
        public void Open(string serialPort);
        public void Close();
        IList<byte> SendAwait(IList<byte> data);
        void Send(IList<byte> data);
    }
}
