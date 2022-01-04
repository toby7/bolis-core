using System.Net.Http.Json;

namespace WineListComparer.Infra.Clients
{
    public sealed class SBApiClient : ISBClient
    {
        public async Task<SBSearchResult> SearchAsync(string query)
        {
            var httpClient = new HttpClient()
            {
                DefaultRequestHeaders = { { "Ocp-Apim-Subscription-Key", "" } }
            };
            var uri = new Uri(
                $"https://api-extern.systembolaget.se/sb-api-ecommerce/v1/productsearch/search?size=30&page=1&textQuery={query}");

            var response = await httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SBSearchResult>();
            }

            return null;
        }
    }

    public interface ISBClient
    {
        Task<SBSearchResult> SearchAsync(string query);
    }
}
