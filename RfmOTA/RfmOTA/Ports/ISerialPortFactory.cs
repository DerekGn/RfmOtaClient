using System.Collections.Generic;

namespace RfmOta.Ports
{
    internal interface ISerialPortFactory
    {
        IList<string> GetSerialPorts();
        ISerialPort CreateSerialPortInstance(string serialPort);
    }
}
