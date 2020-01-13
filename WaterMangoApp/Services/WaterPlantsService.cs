using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WaterMangoApp.Data;
using WaterMangoApp.Factory;
using WaterMangoApp.Hubs;
using WaterMangoApp.Model.BusinessModels;


namespace WaterMangoApp.Services
{
    public class WaterPlantsService : IWaterPlantsService
    {
        public async Task WaterMyPlantsAsync(ILogger _logger, ApplicationDbContext _db, string plantJobId, IHubContext<QuartzHub> quartzHubContext)
        {
            var taskResult = new TaskViewModel();
            var sw = new Stopwatch();
            sw.Start();
           
            // Go and get the list of all plants
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                var plant = await _db.Plants.Where(x => x.JobId == plantJobId).FirstOrDefaultAsync();

                if (plant != null)
                {
                    // Check if the difference between 'last watered time' and 'current time' is greater than 30 seconds
                        var timeDifference = DateTime.UtcNow.Subtract(plant.LastWateredAt).Seconds;
                        
                        if (timeDifference > 30 && timeDifference < 21600)
                        {
                            taskResult.Message = $"Watering Plant {plant.Name} with Id {plantJobId} was Success.";
                            taskResult.Status = true;
                            taskResult.TaskName = "WaterMyPlants";
                            await _db.Tasks.AddAsync(taskResult);
                            await _db.SaveChangesAsync();
                            transaction.Commit();
                            await quartzHubContext.Clients.All.SendAsync("JobEndMessage", "PlantWaterTask", $"Plant Water Task for {plantJobId}  Completed Successfully");
                            plant.LastWateredAt = DateTime.UtcNow;
                            plant.NextWateredAt = DateTime.UtcNow.AddSeconds(30);
                            plant.Status = true;
                            _db.Plants.Update(plant);
                            await _db.SaveChangesAsync();
                            transaction.Commit();
                        }
                        if (timeDifference < 30)
                        {
                            await quartzHubContext.Clients.All.SendAsync("JobEndMessage", "PlantWaterTask", $"Task {plantJobId} was run less than 30 seconds ago.");
                        }
                        if (timeDifference >= 2100)
                        {
                            // Alert the user 
                            taskResult.Message = $"Watering Plant {plant.Name} with Id {plantJobId} was Fail.";
                            taskResult.Status = false;
                            taskResult.TaskName = "WaterMyPlants";
                            await _db.Tasks.AddAsync(taskResult);
                            await _db.SaveChangesAsync();
                            transaction.Commit();
                            await quartzHubContext.Clients.All.SendAsync("JobEndMessage", "PlantWaterTask", $"Plant Water Task for {plantJobId}  Stuck/Failed");
                        }
                }

            }
        }
    }
}