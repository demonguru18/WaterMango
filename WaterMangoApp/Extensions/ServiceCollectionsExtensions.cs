using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Compact;

namespace WaterMangoApp.Extensions
{
    public static class ServiceCollectionsExtensions
    {
        /*---------------------------------------------------------------------------------------------------*/
        /* SERILOG LOGGER EXTENSION                                                                          */
        /*---------------------------------------------------------------------------------------------------*/
        public static void ConfigureLogging(this IServiceCollection services)
        {
            services.AddSingleton(options =>
            {
                var env = options.GetService<IWebHostEnvironment>();
                var httpContextAccessor = options.GetService<IHttpContextAccessor>();
                
                ILogger log = new LoggerConfiguration()
                             .MinimumLevel.Debug()
                             .Enrich.FromLogContext()
                             .Enrich.WithProperty("Application", "CMS_APP")
                             .Enrich.WithProperty("RequestType", httpContextAccessor.HttpContext.Request.Method)
                             .Enrich.WithProperty("ContentType", httpContextAccessor.HttpContext.Request.ContentType)
                             .Enrich.WithProperty("Host", httpContextAccessor.HttpContext.Request.Host)
                             .Enrich.WithProperty("Protocol", httpContextAccessor.HttpContext.Request.Protocol)
                             .Enrich.WithProperty("Path", httpContextAccessor.HttpContext.Request.Path)
                             .Enrich.WithProperty("PathBase", httpContextAccessor.HttpContext.Request.PathBase)
                             .Enrich.WithProperty("RemoteIpAddress", httpContextAccessor.HttpContext.Connection.RemoteIpAddress)
                             .Enrich.WithProperty("LocalPort", httpContextAccessor.HttpContext.Connection.LocalPort)
                             .Enrich.WithProperty("RemotePort", httpContextAccessor.HttpContext.Connection.RemotePort)
                             .Enrich.WithProperty("StatusCode", httpContextAccessor.HttpContext.Response.StatusCode)                             
                             .Enrich.WithProperty("MachineName", Environment.MachineName)
                             .Enrich.WithProperty("CurrentManagedThreadId", Environment.CurrentManagedThreadId)
                             .Enrich.WithProperty("OSVersion", Environment.OSVersion)
                             .Enrich.WithProperty("Version", Environment.Version)
                             .Enrich.WithProperty("UserName", Environment.UserName)
                             .Enrich.WithProperty("ProcessId", Process.GetCurrentProcess().Id)
                             .Enrich.WithProperty("ProcessName", Process.GetCurrentProcess().ProcessName)
                             .WriteTo.File(formatter: new CompactJsonFormatter(),
                                           path: Path.Combine(env.ContentRootPath + $"{Path.DirectorySeparatorChar}Logs{Path.DirectorySeparatorChar}", $"error_{DateTime.Now:yyyyMMdd}.json"))
                             .CreateLogger();
                return log;
            });
        }
    }
}