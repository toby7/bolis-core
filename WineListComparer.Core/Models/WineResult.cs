namespace WineListComparer.Core.Models;

public class WineResult
{
    public WineResult()
    {
        Wines = Enumerable.Empty<Wine>();
        Id = Guid.NewGuid();
    }

    public WineResult(IEnumerable<Wine> wines)
    {
        Wines = wines;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; }
    public IEnumerable<Wine> Wines { get;}
}