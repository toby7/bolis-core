using WineListComparer.Core.Clients;
using WineListComparer.Core.Settings;
using WineListComparer.Infra.Clients;

namespace WineListComparer.API.Startup;

public static class ServiceCollectionExtension
{
    public static WebApplicationBuilder AddInfrastructureServices(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddSbApiClient(builder.Configuration);
        builder.Services.AddWineParser();

        return builder;
    }

    private static IServiceCollection AddSbApiClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<ISbApiClient, SbApiClient>(httpClient =>
        {
            var settings = configuration.GetSection("SbApiSettings").Get<SbApiSettings>();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", settings.Key);
            httpClient.DefaultRequestHeaders.Add("contact", settings.Contact);
        });

        services.AddSingleton<IWineParser, WineParser>();

        return services;
    }

    private static IServiceCollection AddWineParser(
        this IServiceCollection services)
    {
        services.AddSingleton<IWineParser, WineParser>();

        return services;
    }
}
