
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RfmOta.Ota;
using RfmOta.RfmUsb;
using System;
using System.IO;
using Xunit;

namespace RfmOta.UnitTests.Ota
{
    public class OtaServiceTests
    {
        private readonly IOtaService _otaService;
        private readonly Mock<IRfmUsb> _mockRfmusb;

        public OtaServiceTests()
        {
            _mockRfmusb = new Mock<IRfmUsb>();

            _otaService = new OtaService(Mock.Of<ILogger<IOtaService>>(), _mockRfmusb.Object);
        }

        [Fact]
        public void TestOtaUpdateGetFlashSizeFailed()
        {
            // Arrange
            var stream = SetupHexStream();

            // Act
            var result = _otaService.OtaUpdate(new Options() { }, stream, out uint crc);

            // Assert
            result.Should().BeTrue();
        }

        private Stream SetupHexStream()
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            sw.WriteLine(":10010000214601360121470136007EFE09D2190140");
            sw.WriteLine(":100110002146017E17C20001FF5F16002148011928");
            sw.WriteLine(":10012000194E79234623965778239EDA3F01B2CAA7");
            sw.WriteLine(":100130003F0156702B5E712B722B732146013421C7");
            sw.WriteLine(":00000001FF");

            sw.Flush();
            ms.Position = 0;

            return ms;
        }
    }
}
