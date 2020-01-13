using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WaterMangoApp.Factory;

namespace WaterMangoApp.Data
{
    public static class DbContextInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, QuartzDbContext quartzDbContext, IFunctionalService functional,  ILogger logger)
        {
            await context.Database.EnsureCreatedAsync();
            await quartzDbContext.Database.EnsureCreatedAsync();
            
            // Here we will check, if db contains any users. If db is not empty, then db has been seeded
            if (context.ApplicationUsers.Any())
            {
                return;
            }
            
            // We want to now create a Super Admin user if Users are not yet created. So they can be managed
            await functional.CreateDefaultAdminUser(logger);
            
            // We want to now create a App user if Users are not yet created. So they can be managed
            await functional.CreateDefaultAppUser(logger);
            
            // Populate Plants table 
            await functional.CreateDefaultPlants(logger);
        }

        public static async Task InitializeQuartz(QuartzDbContext quartzDbContext)
        {
            await quartzDbContext.Database.EnsureCreatedAsync();
        }
    }
}