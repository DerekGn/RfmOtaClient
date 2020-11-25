using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RfmOta.Ota;
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

            _serviceProvider = BuildServiceProvider(args);

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

                if (otaservice.OtaUpdate(options, stream, out uint crc))
                {
                    logger.LogWarning($"OTA flash update completed. Crc: [{crc}]");
                }
                else
                {
                    logger.LogWarning("OTA flash update failed");
                }
            }
            catch (FileNotFoundException)
            {
                logger.LogWarning($"Unable to find file: [{options.HexFile}]");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unhandled exception occurred");
            }
        }

        static void HandleParseError(IEnumerable<Error> errors)
        {
        }

        private static ServiceProvider BuildServiceProvider(string[] args)
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
