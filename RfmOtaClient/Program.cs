/*
* MIT License
*
* Copyright (c) 2021 Derek Goslin http://corememorydump.blogspot.ie/
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RfmUsb.Exceptions;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace RfmOta
{
    class Program
    {
        private static ServiceProvider _serviceProvider;
        private static IConfiguration _configuration;

        static void Main(string[] args)
        {
            _configuration = SetupConfiguration(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            _serviceProvider = BuildServiceProvider();

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }

        static void RunOptions(Options options)
        {
            var logger = _serviceProvider.GetService<ILogger<Program>>();

            try
            {
                using var stream = File.OpenRead(options.HexFile);
                using var otaservice = _serviceProvider.GetService<IOtaService>();

                if (otaservice.OtaUpdate(options.SerialPort, options.BaudRate, (byte) options.OutputPower, stream, out uint crc))
                {
                    logger.LogInformation($"OTA flash update completed. Crc: [0x{crc:X}]");
                }
                else
                {
                    logger.LogWarning("OTA flash update failed");
                }
            }
            catch(RfmUsbSerialPortNotFoundException ex)
            {
                logger.LogError(ex.Message);
            }
            catch (RfmUsbCommandExecutionException ex)
            {
                logger.LogError(ex.Message);
            }
            catch (FileNotFoundException)
            {
                logger.LogError($"Unable to find file: [{options.HexFile}]");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unhandled exception occurred");
            }
        }

        static void HandleParseError(IEnumerable<Error> errors)
        {
        }

        private static ServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging(builder => builder.AddSerilog())
                .AddSingleton(_configuration)
                .AddLogging()
                .AddOta();

            return serviceCollection.BuildServiceProvider();
        }

        private static IConfiguration SetupConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
        }
    }
}
