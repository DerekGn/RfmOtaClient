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

using RfmOta.Ota;
using System.Collections.Generic;

namespace RfmOta.UnitTests.Ota
{
    public static class TestResponses
    {
        public static List<byte> Empty = new List<byte>();

        public static List<byte> Ok = new List<byte>() { (byte)ResponseType.Ok };

        public static List<byte> PingOk = new List<byte>() { (byte)ResponseType.Ping };

        public static List<byte> EraseOk = new List<byte>() { (byte)ResponseType.Erase };

        public static List<byte> Crc = new List<byte>() { (byte)ResponseType.Crc, 0xAA, 0x55, 0xAA, 0x55 };

        public static List<byte> FlashSizeOk = new List<byte>() { (byte)ResponseType.FlashSize, 0xAA, 0x55, 0xAA, 0x55 };
    }
}
