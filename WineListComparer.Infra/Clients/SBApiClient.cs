using System.Net.Http.Json;
using WineListComparer.Core.Clients;

namespace WineListComparer.Infra.Clients;

public sealed class SbApiClient : ISbApiClient
{
    private readonly HttpClient httpClient;

    public SbApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<SbSearchResult> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return null;
        }

        var uri = new Uri(
            $"https://api-extern.systembolaget.se/sb-api-ecommerce/v1/productsearch/search?size=30&page=1&textQuery={query}");

        var response = await httpClient.GetAsync(uri);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<SbSearchResult>();
        }

        return null;
    }
}