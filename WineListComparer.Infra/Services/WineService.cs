using WineListComparer.Core.Clients;
using WineListComparer.Core.Models;
using WineListComparer.Core.Services;
using WineListComparer.Infra.Clients;

namespace WineListComparer.Infra.Services
{
    public sealed class WineService : IWineService
    {
        private readonly IOCRService ocrService;
        private readonly IWineParser parser;
        private readonly ISbApiClient sbApiClient;

        public WineService(IOCRService ocrService, IWineParser parser, ISbApiClient sbApiClient)
        {
            this.ocrService = ocrService;
            this.parser = parser;
            this.sbApiClient = sbApiClient;
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
            var searchTasks = searchSentences.Select(sentence => sbApiClient.SearchAsync(sentence));
            var sbSearchResults = await Task.WhenAll(searchTasks);

            var wines = sbSearchResults
                .Select(searchResult => searchResult.products.FirstOrDefault())
                .Where(x => x is not null)
                .Select(hit => new Wine()
                {
                    Name = $"{hit.productNameBold} {hit.productNameThin}",
                    Price = hit.price.ToString(),
                    Vintage = hit.vintage,
                    SearchSentence = "Todo",//sentence,
                    Volume = hit.volume,
                    ProductNumber = hit.productNumber,
                    Origin = new Origin()
                    {
                        Country = hit.country,
                        Level1 = hit.originLevel1,
                        Level2 = hit.categoryLevel2
                    }
                });

            var wineResult = new WineResult()
            {
                Wines = wines,
            };

            return wineResult;
        }
    }
}
