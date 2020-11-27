﻿/*
* MIT License
*
* Copyright (c) 2020 Derek Goslin http://corememorydump.blogspot.ie/
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

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

        [Option('t', "timeout", Required = false, Default = 1000, HelpText = "The response timeout in milli seconds")]
        public int Timeout { get; set; }
    }
}
