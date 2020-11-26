using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RfmOta.Ota;
using RfmOta.Rfm;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace RfmOta.UnitTests.Ota
{
    public class OtaServiceTests : BaseTests
    {
        private readonly IOtaService _otaService;
        private readonly Mock<IRfmUsb> _mockRfmusb;

        public OtaServiceTests(ITestOutputHelper output) : base(output)
        {
            _mockRfmusb = new Mock<IRfmUsb>();

            _otaService = new OtaService(Mock.Of<ILogger<IOtaService>>(), _mockRfmusb.Object);
        }

        [Fact]
        public void TestOtaUpdatePingBootloaderFailed()
        {
            // Arrange
            var stream = SetupHexStream();

            _mockRfmusb.Setup(_ => _.SendAwait(It.IsAny<IList<byte>>()))
                .Returns(TestResponses.Empty);

            // Act
            var result = _otaService.OtaUpdate(new Options() { }, stream, out _);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestOtaUpdateGetFlashSizeFailed()
        {
            // Arrange
            var stream = SetupHexStream();

            _mockRfmusb.SetupSequence(_ => _.SendAwait(It.IsAny<IList<byte>>()))
                .Returns(TestResponses.PingOk)
                .Returns(TestResponses.Empty);

            // Act
            var result = _otaService.OtaUpdate(new Options() { }, stream, out _);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestOtaUpdateEraseFailed()
        {
            // Arrange
            var stream = SetupHexStream();

            _mockRfmusb.SetupSequence(_ => _.SendAwait(It.IsAny<IList<byte>>()))
                .Returns(TestResponses.PingOk)
                .Returns(TestResponses.FlashSizeOk)
                .Returns(TestResponses.Empty);

            // Act
            var result = _otaService.OtaUpdate(new Options() { }, stream, out _);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestOtaUpdateSendHexDataFailed()
        {
            // Arrange
            var stream = SetupHexStream();

            _mockRfmusb.SetupSequence(_ => _.SendAwait(It.IsAny<IList<byte>>()))
                .Returns(TestResponses.PingOk)
                .Returns(TestResponses.FlashSizeOk)
                .Returns(TestResponses.EraseOk)
                .Returns(TestResponses.Empty);

            // Act
            var result = _otaService.OtaUpdate(new Options() { }, stream, out _);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestOtaUpdateGetCrcFailed()
        {
            // Arrange
            var stream = SetupHexStream();

            _mockRfmusb.SetupSequence(_ => _.SendAwait(It.IsAny<IList<byte>>()))
                .Returns(TestResponses.PingOk)
                .Returns(TestResponses.FlashSizeOk)
                .Returns(TestResponses.EraseOk)
                .Returns(TestResponses.Ok)
                .Returns(TestResponses.Ok)
                .Returns(TestResponses.Ok)
                .Returns(TestResponses.Ok)
                .Returns(TestResponses.Empty);

            // Act
            var result = _otaService.OtaUpdate(new Options() { }, stream, out _);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestOtaUpdateRebootFailed()
        {
            // Arrange
            var stream = SetupHexStream();

            _mockRfmusb.SetupSequence(_ => _.SendAwait(It.IsAny<IList<byte>>()))
                .Returns(TestResponses.PingOk)
                .Returns(TestResponses.FlashSizeOk)
                .Returns(TestResponses.EraseOk)
                .Returns(TestResponses.Empty);

            // Act
            var result = _otaService.OtaUpdate(new Options() { }, stream, out _);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestOtaUpdateSuceeded()
        {
            // Arrange
            var stream = SetupHexStream();

            _mockRfmusb.SetupSequence(_ => _.SendAwait(It.IsAny<IList<byte>>()))
               .Returns(TestResponses.PingOk)
               .Returns(TestResponses.FlashSizeOk)
               .Returns(TestResponses.EraseOk)
               .Returns(TestResponses.Ok)
               .Returns(TestResponses.Ok)
               .Returns(TestResponses.Ok)
               .Returns(TestResponses.Ok)
               .Returns(TestResponses.Crc);

            // Act
            var result = _otaService.OtaUpdate(new Options() { }, stream, out _);

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
