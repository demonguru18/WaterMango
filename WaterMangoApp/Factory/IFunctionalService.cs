using System.Threading.Tasks;
using Serilog;

namespace WaterMangoApp.Factory
{
    public interface IFunctionalService
    {
        Task CreateDefaultAdminUser(ILogger logger);
        Task CreateDefaultAppUser(ILogger logger);
        Task CreateDefaultPlants(ILogger logger);
    }
}