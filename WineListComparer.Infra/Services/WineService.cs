using System.Diagnostics;
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

    public async Task<WineResult> Process(Stream stream)
    {
        logger.LogInformation("Starting to read image with OCR service.");
        var sentences = await ocrService.ReadImage(stream);
        logger.LogInformation("Done reading image with OCR service.");

        if (sentences is null || sentences.Length < 1)
        {
            return new WineResult();
        }

        logger.LogInformation($"Number of sentences read from image: {sentences.Length}." +
                              $"{NewParagraph}" +
                              $"{string.Join(NewLine, sentences)}");

        var parsedSentences = await Task.WhenAll(sentences.Select(sentence => Task.Run(() => parser.Parse(sentence))));
        var searchSentences = parsedSentences
            .Distinct()
            .Where(sentence => !string.IsNullOrWhiteSpace(sentence))
            .ToArray();

        logger.LogInformation($"Number of sentences after parsing: {searchSentences.Count()}." +
                              $"{NewParagraph}" +
                              $"{string.Join(NewLine, searchSentences)}");

        var buildWineResultFromSentencesTasks = searchSentences
            .Take(50)
            .Select(sentence => scoreScraper.Scrape(sentence).ContinueWith(async scoreTask =>
            {
                var result = await scoreTask;
                var noHitWasFound = string.IsNullOrWhiteSpace(result.Score) || string.IsNullOrWhiteSpace(result.Name);
                if (noHitWasFound)
                {
                    return null;
                }

                var wine = new Wine
                {
                    SearchSentence = sentence,
                    Name = result.Name,
                    Scores = new [] { new WineScore()
                    {
                        Supplier = scoreScraper.Supplier,
                        Score = result.Score,
                        VoteCount = result.VoteCount
                    } },
                    ProductNumber = Guid.NewGuid().ToString()
                };

                var sbSearchResult = await sbApiClient.SearchAsync(sentence);
                if (sbSearchResult.Products is null || !sbSearchResult.Products.Any())
                {
                    return wine;
                }

                var sbHit = sbSearchResult.Products[0];

                wine.Price = sbHit.price.ToString();
                wine.Vintage = sbHit.vintage;
                wine.Volume = sbHit.volume;
                wine.ProductNumber = sbHit.productNumber;
                wine.Origin = new Origin()
                {
                    Country = sbHit.country,
                    Level1 = sbHit.originLevel1,
                    Level2 = sbHit.categoryLevel2
                };

                return wine;
            }).Unwrap());

        var wines = (await Task.WhenAll(buildWineResultFromSentencesTasks)).Where(wine => wine is not null);

        logger.LogInformation($"Number of hits: {wines.Count()}" +
                              $"{NewParagraph}" +
                              $"{string.Join(NewLine, wines.Select(x => x.Name))}");

        var wineResult = new WineResult(wines);

        return wineResult;
    }
}