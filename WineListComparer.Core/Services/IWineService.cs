using WineListComparer.Core.Models;

namespace WineListComparer.Core.Services
{
    public interface IWineService
    {
        Task<WineResult> ProcessWineList(Stream stream);
    }
}
