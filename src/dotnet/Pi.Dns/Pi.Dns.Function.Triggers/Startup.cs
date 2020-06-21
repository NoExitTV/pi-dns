using Destructurama;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pi.Dns.Common.Settings;
using Pi.Dns.Common.Stores;
using Pi.Dns.Common.Stores.Storage;
using Pi.Dns.Function.Triggers;
using Pi.Dns.Function.Triggers.Settings;
using Pi.Dns.Function.Triggers.Statistics;
using Serilog;
using Serilog.Exceptions;
using System;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]
namespace Pi.Dns.Function.Triggers
{
    internal class Startup : IWebJobsStartup
    {
        private IConfiguration _configuration;

        public void Configure(IWebJobsBuilder builder)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("host.json", optional: true, reloadOnChange: true)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Proc", "Pi.Dns.Function.Triggers")
                .Destructure.UsingAttributes()
                .CreateLogger();

            builder.AddDependencyInjection(ConfigureServices);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Settings
            services.Configure<DnsServerApiSettings>(_configuration.GetSection(DnsServerApiSettings.ConfigSectionName));
            services.Configure<DnsServerStatisticsStoreSettings>(_configuration.GetSection(DnsServerStatisticsStoreSettings.ConfigSectionName));
            services.Configure<PiDnsWebApiSettings>(_configuration.GetSection(PiDnsWebApiSettings.ConfigSectionName));
            services.Configure<DnsServersSettings>(_configuration.GetSection(DnsServersSettings.ConfigSectionName));

            // Http clients
            services.AddHttpClient<IDnsServerStatisticsRetreiver, DnsServerStatisticsRetreiver>();
            services.AddHttpClient<IDnsServerStatisticsSender, DnsServerStatisticsSender>();

            // Services
            services.AddSingleton<IServerStatisticsIngressor, ServerStatisticsIngressor>();
            services.AddSingleton<IServerStatisticsSummarizer, ServerStatisticsSummarizer>();

            // Stores
            services.AddSingleton<IDnsServerStatisticsStore, DnsServerStatisticsStorage>();
        }
    }
}
