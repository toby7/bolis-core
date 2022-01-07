using Microsoft.Extensions.DependencyInjection;
using WineListComparer.Core.Clients;
using WineListComparer.Infra.Clients;

namespace WineListComparer.Infra;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpClient<ISbApiClient, SbApiClient>(httpClient =>
        {
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "");
            httpClient.DefaultRequestHeaders.Add("contact", "");
        });

        services.AddSingleton<IWineParser, WineParser>();

        return services;
    }
}
