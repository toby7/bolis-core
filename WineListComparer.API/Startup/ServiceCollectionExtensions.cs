﻿using WineListComparer.Core.Clients;
using WineListComparer.Core.Parsers;
using WineListComparer.Core.Scrapers;
using WineListComparer.Core.Services;
using WineListComparer.Core.Settings;
using WineListComparer.Infra.Clients;
using WineListComparer.Infra.Scrapers;
using WineListComparer.Infra.Services;

namespace WineListComparer.API.Startup;

public static class ServiceCollectionExtension
{
    public static WebApplicationBuilder AddInfrastructureServices(
        this WebApplicationBuilder builder)
    {
        builder.AddOptions();

        builder.Services.AddSbApiClient(builder.Configuration);
        builder.Services.AddSingleton<IWineParser, WineParser>();
        builder.Services.AddSingleton<IOCRService, OCRService>();
        builder.Services.AddSingleton<IWineScoreScraper, VivinoScraper>();

        var useFake = builder.Configuration.GetSection("UseFake").Get<bool>();
        if (useFake)
        {
            builder.Services.AddSingleton<IWineService, FakeWineService>();
        }
        else
        {
            builder.Services.AddSingleton<IWineService, WineService>();
        }

        return builder;
    }

    private static WebApplicationBuilder AddOptions(
        this WebApplicationBuilder builder)
    {
        builder.Services.Configure<OCRSettings>(
            builder.Configuration.GetSection("OCRSettings"));

        return builder;
    }

    private static IServiceCollection AddSbApiClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<ISbApiClient, SbApiClient>(httpClient =>
        {
            var settings = configuration
                .GetSection("SbApiSettings")
                .Get<SbApiSettings>();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", settings.Key);
            httpClient.DefaultRequestHeaders.Add("contact", settings.Contact);
            httpClient.DefaultRequestVersion = new Version(2, 0);
        });

        return services;
    }
}
