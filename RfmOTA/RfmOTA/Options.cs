using CommandLine;
using System;

namespace RfmOta
{
    class Options
    {
        [Option('f', "file", Required = true, HelpText = "Hex File to be processed")]
        public string HexFile { get; set; }

        [Option('s', "serial", Required = true, HelpText = "The serial port that an RfmUsb device is connected")]
        public string SerialPort { get; set; }

        [Option('r', "retry", Required = false, Default = 1, HelpText = "The number of transmission retries")]
        public int RetryCount { get; set; }

        [Option('t', "timeout", Required = false, Default = 1, HelpText = "The response timeout")]
        public TimeSpan Timeout { get; set; }
    }
}
