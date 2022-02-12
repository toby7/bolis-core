using Newtonsoft.Json;
using WineListComparer.Core.Models;
using WineListComparer.Core.Services;

namespace WineListComparer.Infra.Services;

public class FakeWineService : IWineService
{
    private const string Json = "[\r\n    {\r\n        \"name\": \"Delas Saint-Esprit Côtes-du-Rhône\",\r\n        \"vintage\": \"2019\",\r\n        \"price\": \"109\",\r\n        \"productNumber\": \"7183201\",\r\n        \"searchSentence\": \"Delas, Côtes du Rhône\",\r\n        \"volume\": 750,\r\n        \"url\": \"https://www.systembolaget.se/7183201\",\r\n        \"origin\": {\r\n            \"country\": \"Frankrike\",\r\n            \"level1\": \"Rhonedalen\",\r\n            \"level2\": \"Rött\"\r\n        },\r\n        \"scores\": [\r\n            {\r\n                \"name\": \"Delas Vacqueyras Côtes-du-Rhône\",\r\n                \"supplier\": \"Vivino\",\r\n                \"score\": 4.0,\r\n                \"voteCount\": 1552,\r\n                \"relativePath\": \"/SE/sv/wines/163752953\"\r\n            }\r\n        ]\r\n    },\r\n    {\r\n        \"name\": \"Mâcon-Villages Henri de Villamont\",\r\n        \"vintage\": \"2016\",\r\n        \"price\": \"169\",\r\n        \"productNumber\": \"7991501\",\r\n        \"searchSentence\": \"Henri De Villamont, Bourgogne\",\r\n        \"volume\": 750,\r\n        \"url\": \"https://www.systembolaget.se/7991501\",\r\n        \"origin\": {\r\n            \"country\": \"Frankrike\",\r\n            \"level1\": \"Bourgogne\",\r\n            \"level2\": \"Vitt\"\r\n        },\r\n        \"scores\": [\r\n            {\r\n                \"name\": \"Henri de Villamont Bourgogne Pinot Noir\",\r\n                \"supplier\": \"Vivino\",\r\n                \"score\": 3.5,\r\n                \"voteCount\": 63,\r\n                \"relativePath\": \"/SE/sv/wines/1895058\"\r\n            }\r\n        ]\r\n    }\r\n]";

    public Task<WineResult> ProcessWineList(Stream stream)
    {
        Task.Delay(3000);
        Thread.Sleep(3000);
        var wines = JsonConvert.DeserializeObject<IEnumerable<Wine>>(Json);
        var wineResult = new WineResult(wines);

        return Task.FromResult(wineResult);
    }
}