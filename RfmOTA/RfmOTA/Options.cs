using CommandLine;

namespace RfmOta
{
    class Options
    {
        [Option('f', "file", Required = true, HelpText = "Hex File to be processed")]
        public string HexFile { get; set; }

        [Option('s', "serial", Required = true, HelpText = "The serial port that an RfmUsb device is connected")]
        public string SerialPort { get; set; }
    }
}
