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

using Microsoft.Extensions.Logging;
using Moq;
using RfmOta.Ports;
using RfmOta.Rfm;
using Xunit;
using Xunit.Abstractions;

namespace RfmOta.UnitTests.Rfm
{
    public class RfmUsbTests : BaseTests
    {
        private readonly Mock<ISerialPortFactory> _mockSerialPortFactory;
        private readonly Mock<ISerialPort> _mockSerialPort;
        private readonly IRfmUsb _rfmUsb;

        public RfmUsbTests(ITestOutputHelper output) : base(output)
        {
            _mockSerialPortFactory = new Mock<ISerialPortFactory>();
            _mockSerialPort = new Mock<ISerialPort>();

            _rfmUsb = new RfmUsb(Mock.Of<ILogger<IRfmUsb>>(), _mockSerialPortFactory.Object);
        }

        [Fact]
        public void TestOpen()
        {
            // Arrange
            _mockSerialPortFactory
                .Setup(_ => _.CreateSerialPortInstance(It.IsAny<string>()))
                .Returns(_mockSerialPort.Object);

            // Act
            _rfmUsb.Open("ComPort", 9600);

            // Assert
            _mockSerialPortFactory
                .Verify(_ => _.CreateSerialPortInstance(It.IsAny<string>()), Times.Once);

            _mockSerialPort.Verify(_ => _.Open(), Times.Once);
        }

        //[Fact]
        //public void TestOpen()
        //{
        //    // Arrange
        //    _mockSerialPortFactory
        //        .Setup(_ => _.CreateSerialPortInstance(It.IsAny<string>()))
        //        .Returns(_mockSerialPort.Object);

        //    // Act
        //    _rfmUsb.Open("ComPort");

        //    // Assert
        //    _mockSerialPortFactory
        //        .Verify(_ => _.CreateSerialPortInstance(It.IsAny<string>()), Times.Once);

        //    _mockSerialPort.Verify(_ => _.Open(), Times.Once);
        //}
    }
}
