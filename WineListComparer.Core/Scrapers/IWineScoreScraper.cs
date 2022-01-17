using WineListComparer.Core.Models;

namespace WineListComparer.Core.Scrapers;

public interface IWineScoreScraper
{
    string Supplier { get; }
    Task<WineScore> Scrape(string query);
}

