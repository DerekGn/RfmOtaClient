using System;

namespace RfmOta.RfmUsb
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

        public void Open(string serialPort);

        public void Close();
    }
}
