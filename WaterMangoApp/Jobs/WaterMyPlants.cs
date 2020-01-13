using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Serilog;
using WaterMangoApp.Data;
using WaterMangoApp.Factory;
using WaterMangoApp.Hubs;

namespace WaterMangoApp.Jobs
{
    public class WaterMyPlants : IJob
    {
        private readonly IWaterPlantsService _waterPlantsService;
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;
        private readonly IHubContext<QuartzHub> _quartzHubContext;

        public WaterMyPlants(IWaterPlantsService waterPlantsService, IServiceProvider provider, ILogger logger, IHubContext<QuartzHub> quartzHubContext)
        {
            _waterPlantsService = waterPlantsService;
            _provider = provider;
            _logger = logger;
            _quartzHubContext = quartzHubContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var plantJobId = context.JobDetail.Key.Name;
            await _quartzHubContext.Clients.All.SendAsync("JobEndMessage", "PlantWaterTask", $"Plant Water Task for {plantJobId} Started");
            using var scope = _provider.CreateScope();
            var _db = scope.ServiceProvider.GetService<ApplicationDbContext>();
            await _waterPlantsService.WaterMyPlantsAsync(_logger, _db, plantJobId, _quartzHubContext);
        }
    }
}