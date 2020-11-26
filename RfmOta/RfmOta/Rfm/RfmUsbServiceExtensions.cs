using Microsoft.Extensions.DependencyInjection;

namespace RfmOta.Rfm
{
    internal static class RfmUsbServiceExtensions
    {
        public static IServiceCollection AddRfmUsb(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IRfmUsb, RfmUsb>();

            return serviceCollection;
        }
    }
}
