using Microsoft.Extensions.DependencyInjection;

namespace RfmOta.RfmUsb
{
    internal static class RfmUsbServiceExtensions
    {
        public static IServiceCollection AddOta(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IRfmUsb, RfmUsb>();

            return serviceCollection;
        }
    }
}
