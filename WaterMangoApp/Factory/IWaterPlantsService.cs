using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using WaterMangoApp.Data;
using WaterMangoApp.Hubs;

namespace WaterMangoApp.Factory
{
    public interface IWaterPlantsService
    {
        Task WaterMyPlantsAsync(ILogger _logger, ApplicationDbContext _db, string plantJobId,
            IHubContext<QuartzHub> quartzHubContext);
    }
}