
using Microsoft.Extensions.DependencyInjection;

namespace RfmOta.Ports
{
    internal static class PortsServiceExtensions
    {
        public static IServiceCollection AddPorts(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISerialPortFactory, SerialPortFactory>();

            return serviceCollection;
        }
    }
}
