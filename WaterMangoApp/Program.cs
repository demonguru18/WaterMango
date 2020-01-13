using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using WaterMangoApp.Data;
using WaterMangoApp.Factory;

namespace WaterMangoApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var env = new HostingEnvironment
            {
                ContentRootPath = Directory.GetCurrentDirectory()
            };

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "CMS_APP")
                .Enrich.WithProperty("MachineName", Environment.MachineName)
                .Enrich.WithProperty("CurrentManagedThreadId", Environment.CurrentManagedThreadId)
                .Enrich.WithProperty("OSVersion", Environment.OSVersion)
                .Enrich.WithProperty("Version", Environment.Version)
                .Enrich.WithProperty("UserName", Environment.UserName)
                .Enrich.WithProperty("ProcessId", Process.GetCurrentProcess().Id)
                .Enrich.WithProperty("ProcessName", Process.GetCurrentProcess().ProcessName)
                .WriteTo.File(formatter: new CompactJsonFormatter(),
                    path: Path.Combine(env.ContentRootPath + $"{Path.DirectorySeparatorChar}Logs{Path.DirectorySeparatorChar}", $"load_error_{DateTime.Now:yyyyMMdd}.json"))
                .CreateLogger();
                
               var host = CreateWebHostBuilder(args).Build();   
               
               using (var scope = host.Services.CreateScope())
               {
                   var services = scope.ServiceProvider;
               
                   try
                   {
                       var logger = Log.Logger;
                       var context = services.GetRequiredService<ApplicationDbContext>();
                       var quartzDbContext = services.GetRequiredService<QuartzDbContext>();
                       var functional = services.GetRequiredService<IFunctionalService>();
                       DbContextInitializer.Initialize(context,  quartzDbContext, functional, logger).Wait();
                   }
                   catch (Exception ex)
                   {
                       Log.Error(ex, "An error occurred while seeding the database.");
                   }
               }
               host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
    }
}