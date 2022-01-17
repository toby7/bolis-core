using HtmlAgilityPack;

namespace WineListComparer.Infra.Scrapers;

public class VivinoHtmlDocument : HtmlDocument
{
    public string GetRelativePath()
    {
        var href = this
            .DocumentNode
            .SelectSingleNode("//a")
            .Attributes["href"]
            .Value;

        return href;
    }

    public string GetName()
    {
        return this
            .DocumentNode
            .Descendants()
            .FirstOrDefault(x => x.HasClass("bold"))?
            .InnerText
            .Trim() ?? string.Empty;
    }

    public string GetScore()
    {
        return this
            .DocumentNode
            .Descendants()
            .FirstOrDefault(x => x.HasClass("average__number"))?
            .InnerText
            .Trim() ?? string.Empty;
    }

    public string GetVotesCount()
    {
        return this
            .DocumentNode
            .Descendants()
            .FirstOrDefault(x => x.HasClass("text-micro"))?
            .InnerText
            .Replace("betyg", "")
            .Trim() ?? string.Empty;
    }
}

