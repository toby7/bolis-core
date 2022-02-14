using Microsoft.Extensions.Logging;
using WineListComparer.Core.Clients;
using WineListComparer.Core.Models;
using WineListComparer.Core.Parsers;
using WineListComparer.Core.Scrapers;
using WineListComparer.Core.Services;

namespace WineListComparer.Infra.Services;

public sealed class WineService : IWineService
{
    private static readonly string NewLine = Environment.NewLine;
    private static readonly string NewParagraph = $"{NewLine}{NewLine}";

    private readonly IOCRService ocrService;
    private readonly IWineParser parser;
    private readonly ISbApiClient sbApiClient;
    private readonly IWineScoreScraper scoreScraper;
    private readonly ILogger<WineService> logger;

    public WineService(
        IOCRService ocrService,
        IWineParser parser,
        ISbApiClient sbApiClient,
        IWineScoreScraper scoreScraper,
        ILogger<WineService> logger)
    {
        this.ocrService = ocrService;
        this.parser = parser;
        this.sbApiClient = sbApiClient;
        this.scoreScraper = scoreScraper;
        this.logger = logger;
    }

    public async Task<WineResult> ProcessWineList(Stream stream)
    {
        var sentences = await ocrService.ReadImage(stream);

        if (sentences is null || sentences.Length < 1)
        {
            return new WineResult();
        }

        logger.LogTrace($"Number of sentences read from image: {sentences.Length}." +
                        $" {NewParagraph}" +
                        $"{string.Join(NewLine, sentences)}");

        var parserTasks = sentences.Select(sentence => parser.Parse(sentence));
        var searchSentences = (await Task.WhenAll(parserTasks)).Where(x => x is not null);

        logger.LogTrace($"Number of sentences after parsing: {searchSentences.Count()}." +
                        $" {NewParagraph}" +
                        $"{string.Join(NewLine, searchSentences)}");

        var searchTasks = searchSentences.Take(10).Select(sentence => sbApiClient.SearchAsync(sentence));
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
        var wineResult = new WineResult(wines);

        return wineResult;
    }
}