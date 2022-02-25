using System.Text.RegularExpressions;
using WineListComparer.Core.Parsers;

namespace WineListComparer.Infra.Clients;

public sealed class WineParser : IWineParser
{
    public async Task<string> Parse(string sentence)
    {
        if (sentence.StartsWith('(') && sentence.EndsWith(')')) // sentence is wrapped by (). Presumably grape composition.
        {
            return null;
        }

        var isSingleWord = !sentence.Contains(' ');

        if (isSingleWord)
        {
            if (Regex.IsMatch(sentence, @"^\d\d\d\d$")) // only vintage eg. "2016"
            {
                return string.Empty;
            }
            if (WineWordLibrary.Singles.Any(x => x.Equals(sentence, StringComparison.InvariantCultureIgnoreCase)))
            {
                return string.Empty;
            }
            // call
        }

        sentence = Regex.Replace( // Replace specific words with empty string, ex a country should not be present in the name.
            sentence,
            @$"\b({string.Join('|', WineWordLibrary.Replacements)})\b",
            "",
            RegexOptions.IgnoreCase);

        var counter = 0;
        foreach (var word in WineWordLibrary.Singles) // terminate when sentence contains two "singles" or more. Most likely it's grape composition or such. Ex chardonnay, viogner 
        {
            if (!sentence.Contains(word, StringComparison.InvariantCultureIgnoreCase)) continue;
            if (counter == 1)
            {
                return string.Empty;
            }
            counter++;
        }

        sentence = Regex.Replace(sentence, @"\(\d\d\d\d\)", "").Trim(); // Remove vintage, (2008)
        sentence = Regex.Replace(sentence, @"\d\d\d\d", "").Trim(); // Remove vintage, 2008
        sentence = Regex.Replace(sentence, @"\d*(:-| :-)", ""); // any digit followed by :-, eg. "245:-" or "245 :-"

        while (Regex.IsMatch(sentence, @"^[^A-Za-z]"))
        {
            sentence = Regex.Replace(sentence.Trim(), @"^[^A-Za-z]", "").Trim(); // starts with non alphabet character
        }

        while (Regex.IsMatch(sentence, @"[^A-Za-z]$"))
        {
            sentence = Regex.Replace(sentence.Trim(), @"[^A-Za-z]$", "").Trim(); // any trailing non alphabet character
        }

        if (WineWordLibrary.Noise.Any(x => x.Equals(sentence, StringComparison.OrdinalIgnoreCase))) 
        {
            return string.Empty;
        }

        return sentence;
    }
}


public static class WineWordLibrary
{
    public static IEnumerable<string> Countries = new []
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
        "south africa",
        "sa",
        "argentina",
        "argentine",
        "arg",
        "ungern",
        "hungary"
    };

    public static IEnumerable<string> Regions = new []
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
        "kalifornien",
    };

    //public static IEnumerable<string> Replacements = new []
    //{
    //    "Rose",
    //    "Rosé",
    //};



    public static IEnumerable<string> RedGrapes = new []
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
        "Pinot Meunier",
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

    public static IEnumerable<string> GreenGrapes = new []
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

    public static IEnumerable<string> Grapes = RedGrapes.Concat(GreenGrapes);

    public static IEnumerable<string> Singles = new[]
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
        "blåbär",
        "jordgubb",
        "plommon",
        "körsbär",
        "kaffe",
        "viol",
        "citron",
        "citrus",
        "smak",
        "lime",
        "blommor",
        "mineral",
        "sälta",
        "vanilj",
        "fat",
        "ekfat",
        "bär",
        "lingon",
        "tranbär",
        "hallon",
        "frukt",
        "fylligt",
        "medelfylligt",
        "svartpeppar",
        "choklad",
        "friskt",
        "kryddigt",
        "sött",

    }.Concat(Grapes);

    public static IEnumerable<string> Classifications = new[]
    {
        "docg",
        "doc"
    };

    public static IEnumerable<string> Irrelevants = new[]
    {
        "eco",
        "ecological",
        "ecologic",
        "eko",
        "ekologisk",
        "ekologiskt",
        "flaska",
        "bottle",
        "glas",
        "glass"
    };

    public static IEnumerable<string> Replacements = Countries.Concat(Classifications).Concat(Irrelevants);

    public static IEnumerable<string> NonWineSentences = new[]
    {
        "Valpolicella Ripasso",
        "husets röda",
        "pinot noir",
        "Côtes-du-Rhône",
        "Cotes-du-Rhone",
        "cotes du rhone"
    };

    public static IEnumerable<string> Noise = Singles
        .Concat(NonWineSentences)
        .Concat(Regions)
        .Concat(Countries)
        .Concat(Grapes)
        .Concat(Replacements);

}