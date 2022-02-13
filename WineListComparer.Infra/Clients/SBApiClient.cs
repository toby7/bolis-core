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
            return new SbSearchResult();
        }

        var uri = new Uri(
            $"https://api-extern.systembolaget.se/sb-api-ecommerce/v1/productsearch/search?size=30&page=1&textQuery={query}");

        var response = await httpClient.GetAsync(uri);

        if (response.IsSuccessStatusCode is false) return new SbSearchResult();

        var searchResult = await response.Content.ReadFromJsonAsync<SbSearchResult>();
        searchResult.SearchSentence = query;

        return searchResult;
    }
}