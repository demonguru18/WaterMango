using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Serilog;
using WaterMangoApp.Data;
using WaterMangoApp.Factory;
using WaterMangoApp.Helpers;
using WaterMangoApp.Model.BusinessModels;
using WaterMangoApp.Model.IdentityModels;

namespace WaterMangoApp.Services
{
    public class FunctionalService  : IFunctionalService
    {
        private readonly AdminUserOptions _adminUserOptions;
        private readonly AppUserOptions _appUserOptions;
        private readonly UserManager<ApplicationUser> _userManager;
        private ILogger _logger;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public FunctionalService(
            IOptions<AppUserOptions>  appUserOptions, 
            IOptions<AdminUserOptions>  adminUserOptions, 
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db,
            IWebHostEnvironment env)
        {
            _adminUserOptions = adminUserOptions.Value;
            _appUserOptions = appUserOptions.Value;
            _userManager = userManager;
            _db = db;
            _env = env;
        }    
        
        public async Task CreateDefaultAdminUser(ILogger logger)
        {
            _logger = logger;
            
            try
            {
                var adminUser = new ApplicationUser
                {
                    Email = _adminUserOptions.Email,
                    UserName = _adminUserOptions.Username,
                    EmailConfirmed = true,
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true,
                    Firstname = _adminUserOptions.Firstname,
                    Lastname = _adminUserOptions.Lastname,
                    UserRole = "Administrator",
                    Country = "Canada"
                };
                
                var result = await _userManager.CreateAsync(adminUser, _adminUserOptions.Password);
                
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine($"Admin User Created => {adminUser.UserName}");
                    _logger.Information($"Admin User Created => {adminUser.UserName}");
                }
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Admin User Creation Error => {ex.Message}");
                _logger.Error($"Admin User Creation Error => {ex.Message}");
            }
        }
        public async Task CreateDefaultAppUser(ILogger logger)
        {
            _logger = logger;
            
            try
            {
                var appUser = new ApplicationUser
                {
                    Email = _appUserOptions.Email,
                    UserName = _appUserOptions.Username,
                    EmailConfirmed = true,
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true,
                    Firstname = _appUserOptions.Firstname,
                    Lastname = _appUserOptions.Lastname,
                    UserRole = "Customer",
                    Country = "Canada"
                };
                
                var result = await _userManager.CreateAsync(appUser, _appUserOptions.Password);
                
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, "Employee");
                    Console.WriteLine($"App User Created => {appUser.UserName}");
                    _logger.Information($"App User Created => {appUser.UserName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"App User Creation Error => {ex.Message}");
                _logger.Error($"App User Creation Error => {ex.Message}");
            }
        }
        public async Task CreateDefaultPlants(ILogger logger)
        {
            _logger = logger;
            var plants = new List<PlantViewModel>
            {
                new PlantViewModel {Name = "African Violet Plant", Status = false, LastWateredAt = DateTime.Now, NextWateredAt = DateTime.Now.AddSeconds(30), TotalWaterTime = 10, ImageUrl = "/uploads/images/plants/img-1.jpg", Description = "Belonging to the Saintpaulia Genus with many species."},
                new PlantViewModel {Name = "Barberton Daisy Plant", Status = false, LastWateredAt = DateTime.Now, NextWateredAt = DateTime.Now.AddSeconds(30), TotalWaterTime = 10, ImageUrl = "/uploads/images/plants/img-2.jpg", Description = "A flowering pot plant displaying striking flowers."},
                new PlantViewModel {Name = "Aluminum Plant", Status = false, LastWateredAt = DateTime.Now, NextWateredAt = DateTime.Now.AddSeconds(30), TotalWaterTime = 10, ImageUrl = "/uploads/images/plants/img-3.jpg", Description = "An easy going house plant that is generally simple to please."},
                new PlantViewModel {Name = "Areca Palm Plant", Status = false, LastWateredAt = DateTime.Now, NextWateredAt = DateTime.Now.AddSeconds(30), TotalWaterTime = 10, ImageUrl = "/uploads/images/plants/img-4.jpg", Description = "A cane type palm growing up to 8ft tall with mulitple stems."},
                new PlantViewModel {Name = "Arrowhead Plant", Status = false, LastWateredAt = DateTime.Now, NextWateredAt = DateTime.Now.AddSeconds(30), TotalWaterTime = 10, ImageUrl = "/uploads/images/plants/img-5.jpg", Description = "Very similar species as the Philodendron and from the same family."},
            };

            try
            {
                await _db.Plants.AddRangeAsync(plants);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Plants Creation Error => {ex.Message}");
                _logger.Error($"Plants Creation Error => {ex.Message}");
            }
        }

        public async Task CreateDefaultQuartzTasks(ILogger logger)
        {
            
        }
    }
}