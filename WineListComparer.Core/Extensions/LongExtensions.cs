namespace WineListComparer.Core.Extensions;

public static class LongExtensions
{
    public static double ToMegabytes(this long bytes)
    {
        return (bytes / 1024f) / 1024f;
    }
}