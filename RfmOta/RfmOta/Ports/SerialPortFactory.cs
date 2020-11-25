using System.Collections.Generic;

namespace RfmOta.Ports
{
    internal class SerialPortFactory : ISerialPortFactory
    {
        public ISerialPort CreateSerialPortInstance(string serialPort)
        {
            return new SerialPort(serialPort);
        }

        public IList<string> GetSerialPorts()
        {
            return System.IO.Ports.SerialPort.GetPortNames();
        }
    }
}
