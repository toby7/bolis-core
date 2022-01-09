namespace WineListComparer.Core.Services;

public interface IOCRService
{
    Task<string[]> ReadImage(Stream stream);
}