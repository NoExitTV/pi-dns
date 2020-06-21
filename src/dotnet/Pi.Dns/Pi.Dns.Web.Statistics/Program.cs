using Destructurama;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using System;

namespace Pi.Dns.Web.Statistics
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                {
                    configurationBuilder.SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("host.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();

                    var configuration = configurationBuilder.Build();
                })
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .Enrich.WithProperty("Proc", "Pi.Dns.Web.Statistics")
                    .Destructure.UsingAttributes();
                })
                .UseStartup<Startup>();
            });
    }
}
