using System;
using System.Collections.Specialized;
using System.Text;
using DataAccessService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Quartz.Impl;
using WaterMangoApp.Data;
using WaterMangoApp.Extensions;
using WaterMangoApp.Factory;
using WaterMangoApp.Helpers;
using WaterMangoApp.Hubs;
using WaterMangoApp.Jobs;
using WaterMangoApp.Model.IdentityModels;
using WaterMangoApp.Services;


namespace WaterMangoApp
{
    public class Startup
    {
        private IScheduler _quartzScheduler;
        private static readonly string _appEnv = "DEV-LOCAL";
        private static readonly string _appQuartz = "DEV-QUARTZ";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _quartzScheduler = ConfigureQuartz();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*---------------------------------------------------------------------------------------------------*/
            /*                             Email/Functional SERVICE                                              */
            /*---------------------------------------------------------------------------------------------------*/
            services.Configure<AdminUserOptions>(Configuration.GetSection("AdminUserOptions"));
            services.Configure<AppUserOptions>(Configuration.GetSection("AppUserOptions"));
            services.AddTransient<IFunctionalService, FunctionalService>();
            /*---------------------------------------------------------------------------------------------------*/
            /*                              DB CONNECTION OPTIONS                                                */
            /*---------------------------------------------------------------------------------------------------*/
            var t = DBAccess.GetConnectionString(_appQuartz);
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(DBAccess.GetConnectionString(_appEnv)));
            services.AddDbContext<QuartzDbContext>(options => options.UseSqlServer(DBAccess.GetConnectionString(_appQuartz)));
            /*---------------------------------------------------------------------------------------------------*/
            /*                             WATER MY PLANTS SERVICE                                               */
            /*---------------------------------------------------------------------------------------------------*/
            services.AddSingleton<IWaterPlantsService, WaterPlantsService>();
            services.AddTransient<WaterMyPlants>();
            services.AddSingleton(provider => _quartzScheduler);
            /*---------------------------------------------------------------------------------------------------*/
            /* SERILOG LOGGER OPTIONS - Creates Singleton Object and returns same object on subsequent calls     */
            /*---------------------------------------------------------------------------------------------------*/
            services.AddHttpContextAccessor();
            services.ConfigureLogging();
            /*---------------------------------------------------------------------------------------------------*/
            /*                              DEFAULT IDENTITY OPTIONS                                             */
            /*---------------------------------------------------------------------------------------------------*/
            var identityDefaultOptionsConfigurationSection = Configuration.GetSection("IdentityDefaultOptions");
            services.Configure<IdentityDefaultOptions>(identityDefaultOptionsConfigurationSection);
            var identityDefaultOptions = identityDefaultOptionsConfigurationSection.Get<IdentityDefaultOptions>();
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = identityDefaultOptions.PasswordRequireDigit;
                options.Password.RequiredLength = identityDefaultOptions.PasswordRequiredLength;
                options.Password.RequireNonAlphanumeric = identityDefaultOptions.PasswordRequireNonAlphanumeric;
                options.Password.RequireUppercase = identityDefaultOptions.PasswordRequireUppercase;
                options.Password.RequireLowercase = identityDefaultOptions.PasswordRequireLowercase;
                options.Password.RequiredUniqueChars = identityDefaultOptions.PasswordRequiredUniqueChars;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(identityDefaultOptions.LockoutDefaultLockoutTimeSpanInMinutes);
                options.Lockout.MaxFailedAccessAttempts = identityDefaultOptions.LockoutMaxFailedAccessAttempts;
                options.Lockout.AllowedForNewUsers = identityDefaultOptions.LockoutAllowedForNewUsers;

                // User settings
                options.User.RequireUniqueEmail = identityDefaultOptions.UserRequireUniqueEmail;

                // email confirmation require
                options.SignIn.RequireConfirmedEmail = identityDefaultOptions.SignInRequireConfirmedEmail;

            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            /*---------------------------------------------------------------------------------------------------*/
            /*                                 APPSETTINGS SERVICE                                               */
            /*---------------------------------------------------------------------------------------------------*/
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(o => {
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => 
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = appSettings.ValidateIssuerSigningKey,
                    ValidateIssuer = appSettings.ValidateIssuer,
                    ValidateAudience = appSettings.ValidateAudience,
                    ValidIssuer = appSettings.Site,
                    ValidAudience = appSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero

                };
            });
            /*---------------------------------------------------------------------------------------------------*/
            /*                              AUTH SERVICE                                                         */
            /*---------------------------------------------------------------------------------------------------*/
            services.AddTransient<IAuthService, AuthService>();
            /*---------------------------------------------------------------------------------------------------*/
            /*                              Cookie Helper SERVICE                                                */
            /*---------------------------------------------------------------------------------------------------*/
            services.AddTransient<CookieOptions>();
            /*---------------------------------------------------------------------------------------------------*/
            /*                              ENABLE CORS                                                          */
            /*---------------------------------------------------------------------------------------------------*/
            services.AddCors(options => {
                options.AddPolicy("EnableCORS", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build();
                });
            });
            /*---------------------------------------------------------------------------------------------------*/
            /*                              ENABLE API Versioning                                                */
            /*---------------------------------------------------------------------------------------------------*/
             services.AddApiVersioning(
                options =>
                {
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                });
            /*---------------------------------------------------------------------------------------------------*/
            /*                              RazorPages Runtime Services                                          */
            /*---------------------------------------------------------------------------------------------------*/
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });
            services.AddAntiforgery();
            services.AddMvc().AddControllersAsServices().AddRazorRuntimeCompilation().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            
            _quartzScheduler.JobFactory = new WaterPlantJobFactory(app.ApplicationServices);
            _quartzScheduler.Start().Wait();

            app.UseCors("EnableCORS");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<QuartzHub>("/quartzHub");
                
                endpoints.MapControllerRoute(
                    "areas",
                    "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    "restApi",
                    pattern: "api/{controller}/{action}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
        
        /*---------------------------------------------------------------------------------------------------*/
        /*                              Configure Quartz Tasks                                               */
        /*---------------------------------------------------------------------------------------------------*/
        
        public void OnShutDown()
        {
            if (!_quartzScheduler.IsShutdown)
            {
                _quartzScheduler.Shutdown();
            }
        }
        
        public IScheduler ConfigureQuartz()
        {
            var ct = DBAccess.GetConnectionString(_appQuartz);
            
            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "json"},
                { "quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz"},
                { "quartz.jobStore.dataSource", "default"},
                { "quartz.dataSource.default.provider", "SqlServer"},
                { "quartz.dataSource.default.connectionString", ct},
                { "quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz" },
                {"quartz.jobStore.clustered","true" }
            };
            
            var factory = new StdSchedulerFactory(props);
            
            var scheduler = factory.GetScheduler().Result;
            
            scheduler.ListenerManager.AddTriggerListener(new TriggerListener());

            scheduler.ListenerManager.AddJobListener(new JobListener());

            scheduler.ListenerManager.AddSchedulerListener(new SchedulerListener());

            return scheduler;

        }
        
        
    }
}