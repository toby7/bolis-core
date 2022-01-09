namespace WineListComparer.Core.Models;

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