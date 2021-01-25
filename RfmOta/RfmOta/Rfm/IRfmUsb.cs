/*
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

using System;
using System.Collections.Generic;

namespace RfmOta.Rfm
{
    internal interface IRfmUsb : IDisposable
    {
        string Version { get; }
        int OutputPower { get; set; }
        byte PayloadLenght { get; set; }
        bool VariableLenght { get; set; }
        byte FifoThreshold { get; set; }
        byte DioInterruptMask { get; set; }
        int RetryCount { get; set; }
        int Timeout { get; set; }
        bool TransmissionStartCondition { get; set; }
        byte RadioConfig { get; set; }
        IEnumerable<byte> Sync { get; set; }
        public void Reset();
        public void Close();
        public void Open(string serialPort, int baudRate);
        void Send(IList<byte> data);
        /// <summary>
        /// Transmit a packet of data bytes and wait for a response
        /// </summary>
        /// <param name="data">The data to transmit</param>
        /// <param name="timeout">The timeout in milliseconds to wait for a response</param>
        /// <returns>The received packet bytes</returns>
        IList<byte> TransmitReceive(IList<byte> data, int timeout);
        /// <summary>
        /// Transmit a packet of data bytes
        /// </summary>
        /// <param name="data">The data to transmit</param>
        /// <param name="timeout">The timeout in milliseconds to wait for a response</param>
        void Transmit(IList<byte> data, int timeout);
        void SetDioMapping(Dio dio, DioMapping mapping);
    }
}
