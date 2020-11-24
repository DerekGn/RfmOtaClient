using Microsoft.Extensions.DependencyInjection;

namespace RfmOta.Ota
{
    internal static class OtaServiceExtensions
    {
        public static IServiceCollection AddOta(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IOtaService, OtaService>();

            return serviceCollection;
        }
    }
}
