using WineListComparer.Core.Clients;
using WineListComparer.Core.Models;
using WineListComparer.Core.Parsers;
using WineListComparer.Core.Scrapers;
using WineListComparer.Core.Services;

namespace WineListComparer.Infra.Services;

public sealed class WineService : IWineService
{
    private readonly IOCRService ocrService;
    private readonly IWineParser parser;
    private readonly ISbApiClient sbApiClient;
    private readonly IWineScoreScraper scoreScraper;

    public WineService(IOCRService ocrService, IWineParser parser, ISbApiClient sbApiClient, IWineScoreScraper scoreScraper)
    {
        this.ocrService = ocrService;
        this.parser = parser;
        this.sbApiClient = sbApiClient;
        this.scoreScraper = scoreScraper;
    }

    public async Task<WineResult> ProcessWineList(Stream stream)
    {
        var sentences = await ocrService.ReadImage(stream);

        if (sentences is null || sentences.Length < 1)
        {
            return new WineResult();
        }

        var parserTasks = sentences.Select(sentence => parser.Parse(sentence));
        var searchSentences = await Task.WhenAll(parserTasks);
        var searchTasks = searchSentences.Take(3).Select(sentence => sbApiClient.SearchAsync(sentence));
        var sbSearchResults = (await Task.WhenAll(searchTasks)).Where(x => x.Products != null && x.Products.Any());

        var sbSearchHits = sbSearchResults
            .Select(searchResult => new SbHit()
            {
                SearchSentence = searchResult.SearchSentence,
                Product = searchResult.Products?.FirstOrDefault() ?? new Product()
            });


        var wineTasks = sbSearchHits
            .Select(async hit => new Wine()
            {
                Name = $"{hit.Product.productNameBold} {hit.Product.productNameThin}",
                Price = hit.Product.price.ToString(),
                Vintage = hit.Product.vintage,
                SearchSentence = hit.SearchSentence,
                Volume = hit.Product.volume,
                ProductNumber = hit.Product.productNumber,
                Origin = new Origin()
                {
                    Country = hit.Product.country,
                    Level1 = hit.Product.originLevel1,
                    Level2 = hit.Product.categoryLevel2
                },
                Scores = new [] { await scoreScraper.Scrape(hit.SearchSentence) }
            });

        var wines = await Task.WhenAll(wineTasks);

        var wineResult = new WineResult()
        {
            Wines = wines,
        };

        return wineResult;
    }
}