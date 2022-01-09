namespace WineListComparer.Core.Parsers;

public interface IWineParser
{
    Task<string> Parse(string sentence);
}