using Microsoft.Extensions.Options;
using Pi.Dns.Common.Models;
using Pi.Dns.Common.Stores;
using Pi.Dns.Function.Triggers.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pi.Dns.Function.Triggers.Statistics
{
    public class ServerStatisticsIngressor : IServerStatisticsIngressor
    {
        private readonly DnsServerApiSettings _dnsServerApiSettings;
        private readonly IDnsServerStatisticsRetreiver _dnsServerStatisticsRetreiver;
        private readonly IDnsServerStatisticsStore _dnsServerStatisticsStore;
        private readonly ILogger _logger;
        public ServerStatisticsIngressor(
            IOptions<DnsServerApiSettings> dnsServerApiSettings,
            IDnsServerStatisticsRetreiver dnsServerStatisticsRetreiver,
            IDnsServerStatisticsStore dnsServerStatisticsStore)
        {
            _dnsServerApiSettings = dnsServerApiSettings.Value;
            _dnsServerStatisticsRetreiver = dnsServerStatisticsRetreiver;
            _dnsServerStatisticsStore = dnsServerStatisticsStore;
            _logger = Log.ForContext("SourceContext", nameof(ServerStatisticsIngressor));
        }

        /// <summary>
        /// Fetch DNS server statistics from all DNS servers and ingress to storage account
        /// </summary>
        /// <returns></returns>
        public async Task IngressDnsServerStatistics()
        {
            try
            {
                var fetchStatisticsTasks = new List<Task<DnsServerStatistics>>();

                foreach (var dnsServerApi in _dnsServerApiSettings.DnsServerApis)
                {
                    fetchStatisticsTasks.Add(_dnsServerStatisticsRetreiver.GetStatistics(dnsServerApi.ServerName, dnsServerApi.ApiKey));
                }

                var simultaneousTasks = new List<Task>();
                await Task.WhenAll(fetchStatisticsTasks.ToArray());

                foreach (var completeTask in fetchStatisticsTasks)
                {
                    var result = completeTask.Result;

                    if (result == null)
                    {
                        _logger.Warning("DNS server statistics was null. Something went wrong while fetching dns server statistics (CompleteTasks: {@CompleteTasks})", fetchStatisticsTasks);
                        continue;
                    }

                    _logger.Debug("Storing statistics result for server {Server} created at {CreatedDate}", result.ServerName, result.CreatedDate);
                    simultaneousTasks.Add(_dnsServerStatisticsStore.Add(result));
                }

                await Task.WhenAll(simultaneousTasks.ToArray());
                _logger.Information("Execution complete!");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Got an unhandled exception while ingressing dns server statistics");
                throw;
            }
        }
    }
}
