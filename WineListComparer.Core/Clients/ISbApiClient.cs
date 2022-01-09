namespace WineListComparer.Core.Clients;

public interface ISbApiClient
{
    Task<SbSearchResult> SearchAsync(string query);
}