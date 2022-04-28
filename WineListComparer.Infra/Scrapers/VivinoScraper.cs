using Microsoft.Extensions.Logging;
using WineListComparer.Core.Models;
using WineListComparer.Core.Scrapers;

namespace WineListComparer.Infra.Scrapers;

public sealed class VivinoScraper : IWineScoreScraper
{
    private const string Url = @"https://www.vivino.com/search/wines?q={0}";
    private readonly ILogger<VivinoScraper> logger;

    public VivinoScraper(ILogger<VivinoScraper> logger)
    {
        this.logger = logger;
    }

    public string Supplier => "Vivino";

    public async Task<WineScore> Scrape(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new WineScore();
        }

        var httpClient = new HttpClient();
        var result = await httpClient.GetStringAsync(string.Format(Url, query));

        var htmlDoc = new VivinoHtmlDocument();
        htmlDoc.LoadHtml(result);

        var firstHitNode = htmlDoc.DocumentNode
            .Descendants()
            .FirstOrDefault(x => x.HasClass("wine-card__content"));

        if (firstHitNode is null)
        {
            return new WineScore();
        }

        htmlDoc.LoadHtml(firstHitNode.InnerHtml);

        var wineScore = new WineScore()
        {
            Supplier = this.Supplier,
            Name = htmlDoc.GetName(),
            Score = htmlDoc.GetScore(),
            VoteCount = htmlDoc.GetVotesCount(),
            RelativePath = htmlDoc.GetRelativePath()
        };

        logger.LogInformation($"Done scraping for '{query}' on '{this.Supplier}'.");
        return wineScore;
    }
}

