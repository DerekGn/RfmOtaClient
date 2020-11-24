using HexIO;
using Microsoft.Extensions.Logging;
using RfmOta.RfmUsb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RfmOta.Ota
{
    internal class OtaService : IOtaService
    {
        private readonly ILogger<IOtaService> _logger;
        private readonly IRfmUsb _rfmUsb;

        public OtaService(ILogger<IOtaService> logger, IRfmUsb rfmUsb)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _rfmUsb = rfmUsb ?? throw new ArgumentNullException(nameof(rfmUsb));

            //_serialPortFactory = serialPortFactory ?? throw new ArgumentNullException(nameof(serialPortFactory));
        }

        public void OtaUpdate(string hexFile, string serialPort)
        {
            if (!File.Exists(hexFile))
            {
                _logger.LogWarning($"Unable to open file: [{hexFile}]");

                return;
            }

            try
            {
                InitaliseRfmUsb(serialPort);

                using var fileStream = File.OpenRead(hexFile);
                using IntelHexReader hexReader = new IntelHexReader(fileStream);

                while (hexReader.Read(out uint address, out IList<byte> data))
                {
                    _logger.LogInformation($"Writing Address: [0x{address:X4}] Count: [0x{data.Count:X2}]" +
                        $" Data: [{BitConverter.ToString(data.ToArray()).Replace("-", "")}]");

                    //_rfmUsb.Transmit();
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                _rfmUsb?.Close();
            }
        }

        private void InitaliseRfmUsb(string serialPort)
        {
            _rfmUsb.Open(serialPort);

            _rfmUsb.PayloadLenght = 0xFF;

            _rfmUsb.VariableLenght = true;

            _rfmUsb.EnterCondition = EnterCondition.PacketSent;

            _rfmUsb.IntermediateMode = IntermediateMode.Rx;

            _rfmUsb.ExitCondition = ExitCondition.CrcOkTimeout;

            _rfmUsb.FifoThreshold = 0x02;

            _rfmUsb.TxStartCondition = false;
        }

        #region
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _rfmUsb?.Close();
                    _rfmUsb?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~OtaService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
