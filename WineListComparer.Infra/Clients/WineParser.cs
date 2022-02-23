using System.Text.RegularExpressions;
using WineListComparer.Core.Parsers;

namespace WineListComparer.Infra.Clients;

public sealed class WineParser : IWineParser
{
    public async Task<string> Parse(string sentence)
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

        return sentence;
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

    //public static string[] Grapes = RedGrapes.Union(GreenGrapes).ToArray();

    public static string[] RedGrapes =
    {
        "syrah",
        "shiraz",
        "cabernet sauvignon",
        "tempranillo",
        "nebiolo",
        "carignan",
        "grenache",
        "garnacha",
        "mourvedre",
        "mourvédre",
        "monastrell",
        "mataro",
        "pinot noir",
        "spätburgunder",
        "zinfandel",
        "primitivo",
        "malbec",
        "barbera",
        "sangiovese",
        "merlot",
        "cabernet franc",
        "corvina",
        "rondinella",
        "petit verdot",
        "petit sirah",
        "aglianico",
        "dolcetto",
        "gamay",

        


    };

    public static string[] GreenGrapes =
    {
        "riesling",
        "chardonnay",
        "sauvignon blanc",
        "chenin blanc",
        "rousanne",
        "marsanne",
        "semillion",
        "glera",
        "gruner veltliner",
        "grüner veltliner",
        "gewurstraminer",
        "pinot gris",
        "pinot grigio",
        "arneis",
        "muscat",
        "pinot blanc",
        "trebbiano"


    };
}