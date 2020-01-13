using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;
using WaterMangoApp.Data;
using Serilog;
using WaterMangoApp.Jobs;
using WaterMangoApp.Model.BusinessModels;
using WaterMangoApp.Model.HttpModels;

namespace WaterMangoApp.Controllers.Api.v1
{
    [ApiVersion( "1.0" )]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PlantController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger _logger;
        private readonly IScheduler _scheduler;

        
        public PlantController(ApplicationDbContext db, ILogger logger, IScheduler scheduler)
        {
            _db = db;
            _logger = logger;
            _scheduler = scheduler;
        }
       
        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> GetListOfPlants()
        {
            try
            {
                var listOfPlants = await _db.Plants.ToListAsync();
                
                if (listOfPlants != null)
                {
                    return Ok(listOfPlants);
                }
            }
            catch (Exception ex)
            {
                const string error = "Error Was generated GetListOfPlants Method";
                _logger.Error($"Unable to Confirm Email of user with ID '{error}'.");
                _logger.Error($"Error Message => '{ex.Message}'.");
                return BadRequest();
            }
            return BadRequest();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAlertData()
        {
            try
            {
                var listOfAlerts= await _db.Tasks.ToListAsync();
                
                if (listOfAlerts != null)
                {
                    return Ok(listOfAlerts);
                }
            }
            catch (Exception ex)
            {
                const string error = "Error Was generated GetListOfPlants Method";
                _logger.Error($"Unable to Confirm Email of user with ID '{error}'.");
                _logger.Error($"Error Message => '{ex.Message}'.");
                return BadRequest();
            }
            return BadRequest();
        }

        [HttpPost("[action]/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StarWateringPlant([FromRoute] int id)
        {
            try
            {
                var job = JobBuilder.Create<WaterMyPlants>()
                    .WithIdentity($"{id}-PWSVC", "WaterMangoPlantGroup").StoreDurably().Build();  
                
                await _scheduler.AddJob(job, true);

                var plant = await _db.Plants.FindAsync(id);
                
                if (plant != null)
                {
                    if (string.IsNullOrEmpty(plant.JobId))
                    {
                        plant.JobId = $"{id}-PWSVC";
                        _db.Update(plant);
                        await _db.SaveChangesAsync();
                    }
                }

                var trigger = TriggerBuilder.Create().ForJob(job)
                    .UsingJobData("PlantId", id)
                    .WithIdentity($"{id}-Trigger", "WaterMangoPlantTriggers")
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever())
                    .Build();

                await _scheduler.ScheduleJob(trigger);

                return Ok($"Task for watering plant with id : {id} has been created.");

            }
            catch (ObjectAlreadyExistsException ex)
            {
                _logger.Error(ex.Message);

                return BadRequest("Task already Running or Exists");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);

                return BadRequest("Please Check logs for Details.");
            }
        }
        
       
        [HttpGet("[action]")]
        public async Task<IActionResult> GetCardInfo()
        {
            try
            {
                var tasks = await _db.Tasks.ToListAsync();
                var cardResponse = new CardInfoResponseModel
                {
                    PlantCount = await _db.Plants.CountAsync(),
                    AlertCount = tasks.Count(x => !x.Status),
                    SuccessCount = tasks.Count(x => x.Status),
                    UserCount = await _db.Users.CountAsync(),
                };

                return Ok(cardResponse);
            }
            catch (Exception ex)
            {
                const string error = "Error Was generated GetCardInfo Method";
                _logger.Error($"Unable to Preload Card Info'{error}'.");
                _logger.Error($"Error Message => '{ex.Message}'.");
                return BadRequest();
            }
        }
        
        
        [HttpDelete("[action]/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StopWateringTask([FromRoute] string id)
        {
            try
            {
                var result = await _scheduler.DeleteJob(new JobKey(id, "WaterMangoPlantGroup"));

                if (!result)
                {
                    return BadRequest("Task Does not exist or is already deleted");
                }

                var plant = await _db.Plants.Where(x => x.JobId == id).FirstOrDefaultAsync();
                if (plant != null)
                {
                    plant.Status = false;
                    _db.Update(plant);
                   await _db.SaveChangesAsync();
                }
                return Ok("Task Deleted");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return BadRequest("Please Check logs for Details.");
        }

    }
}