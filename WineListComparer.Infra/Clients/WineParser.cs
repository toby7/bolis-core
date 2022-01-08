﻿using System.Text.RegularExpressions;
using WineListComparer.Core.Clients;

namespace WineListComparer.Infra.Clients
{
    public sealed class WineParser : IWineParser
    {
        private readonly ISbApiClient sbClient;

        public WineParser(ISbApiClient sbClient)
        {
            this.sbClient = sbClient;
        }

        public async Task<Wine> Parse(string sentence)
        {
            var isSingleWord = sentence.Contains(' ');

            if (isSingleWord)
            {
                if (Regex.IsMatch(sentence, @"^\d\d\d\d$")) // only vintage eg. "2016"
                {
                    return null;
                }
                if (ExcludedWords.Singles.Any(x => x.Equals(sentence, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return null;
                }
                // call
            }

            sentence = Regex.Replace(
                sentence,
                @$"\b({string.Join('|', ExcludedWords.Removals)})\b",
                "",
                RegexOptions.IgnoreCase);

            var counter = 0;
            foreach (var word in ExcludedWords.Singles) // terminate when sentence contains two "singles" or more. Most likely it's grape composition or such. Ex chardonnay, viogner 
            {
                if (!sentence.Contains(word, StringComparison.InvariantCultureIgnoreCase)) continue;
                if (counter == 1)
                {
                    return null;
                }
                counter++;
            }

            sentence = Regex.Replace(sentence, @"\d\d\d\d", "").Trim(); // Remove vintage
            sentence = Regex.Replace(sentence, @"\d*(:-| :-)", ""); // any digit followed by :-, eg. "245:-" or "245 :-"

            while (Regex.IsMatch(sentence, @"[^A-Za-z]$"))
            {
                sentence = Regex.Replace(sentence.Trim(), @"[^A-Za-z]$", "").Trim(); // any trailing non alphabet character
            }

            //return null;
            var searchResult = await sbClient.SearchAsync(sentence);
            var hit = searchResult?.products?.FirstOrDefault();

            if (hit is null)
            {
                return null;
            }

            var wineHit = new Wine()
            {
                Name = $"{hit.productNameBold} {hit.productNameThin}",
                Price = hit.price.ToString(),
                Vintage = hit.vintage,
                SearchSentence = sentence,
                Volume = hit.volume,
                ProductNumber = hit.productNumber,
                Origin = new Origin()
                {
                    Country = hit.country,
                    Level1 = hit.originLevel1,
                    Level2 = hit.categoryLevel2
                }
            };

            return wineHit;
            // Fiska ut druva och årgång i varje sentence för att atcha mot sökning istllet för att ha med det i query
            // Räkna om ord förekommer ovanligt ofta i hela texten och testa då att skala bort dem
        }
    }


    public static class ExcludedWords
    {
        public static string[] Countries =
        {
            "frankrike",
            "france",
            "fra",
            "italien",
            "italy",
            "ita",
            "spanien",
            "spain",
            "spa",
            "usa",
            "australien",
            "australia",
            "aus",
            "sydafrika",
            "south africa"
        };

        public static string[] Removals = Countries;
        //    \b(?:one|two|three)\b
        //\b(word)\b
        public static string[] Singles =
        {
            "vin",
            "viner",
            "wine",
            "wines",
            "vitt",
            "vita",
            "white",
            "whites",
            "rött",
            "röda",
            "red",
            "reds",
            "rosa",
            "rose",
            "rosé",
            "bubbel",
            "mousserande",
            "sparkling",
            "bubbles",
            "france",
            "frankrike",
            "champagne",
            "bordeaux",
            "rhone",
            "bourgogne",
            "burgundy",
            "loire",
            "jura",
            "provance",
            "languedoc",
            "rosillion",
            "alsace",
            "italy",
            "italien",
            "tuscany",
            "toskana",
            "veneto",
            "piemonte",
            "spanien",
            "spain",
            "cava",
            "rioja",
            "ribera del duero"
        };

        public static string[] Regions =
        {
            "champagne",
            "bordeaux",
            "rhone",
            "bourgogne",
            "burgundy",
            "loire",
            "jura",
            "languedoc",
            "rosillion",
            "alsace",
            "tuscany",
            "toskana",
        };

        public static string[] Replacements =
        {
            "Rose",
            "Rosé",
        };
    }

    public interface IWineParser
    {
        Task<Wine> Parse(string characters);
    }

    public class WineResult
    {
        IEnumerable<Wine> Wines { get; set; }
    }

    public class Wine
    {
        public string Name { get; set; }
        public string Vintage { get; set; }
        public string Price { get; set; }
        public string ProductNumber { get; set; }
        public string SearchSentence { get; set; }
        public float Volume { get; set; }
        public string Url => $"https://www.systembolaget.se/{ProductNumber}";
        public Origin Origin { get; set; }
    }

    public class Origin
    {
        public string Country { get; set; }
        public string Level1 { get; set; }
        public string Level2 { get; set; }
    }
}