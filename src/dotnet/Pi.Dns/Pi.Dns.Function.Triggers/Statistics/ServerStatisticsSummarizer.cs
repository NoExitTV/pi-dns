using Microsoft.Extensions.Options;
using Pi.Dns.Common.Models;
using Pi.Dns.Common.Settings;
using Pi.Dns.Common.Stores;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pi.Dns.Function.Triggers.Statistics
{
    public class ServerStatisticsSummarizer : IServerStatisticsSummarizer
    {
        private const int HoursToSummarize = 24;

        // Settings
        private readonly DnsServersSettings _dnsServersSettings;

        // Dependency injection
        private readonly IDnsServerStatisticsStore _dnsServerStatisticsStore;
        private readonly ILogger _logger;

        public ServerStatisticsSummarizer(
            IOptions<DnsServersSettings> dnsServersSettings,
            IDnsServerStatisticsStore dnsServerStatisticsStore)
        {
            _dnsServersSettings = dnsServersSettings.Value;
            _dnsServerStatisticsStore = dnsServerStatisticsStore;
            _logger = Log.ForContext("SourceContext", nameof(ServerStatisticsSummarizer));
        }

        public async Task<IEnumerable<SummarizedDnsServerStatistics>> Summarize24h()
        {
            var allServerStatistics = (await GetAllDnsServerStatistics()).OrderBy(s => s.CreatedDate);
            var groupedServerStatistics = allServerStatistics.GroupBy(s => s.ServerName);
            var summarizedStatisticsPerServer = new List<SummarizedDnsServerStatistics>();

            foreach (var group in groupedServerStatistics)
            {
                var serverStatistics = group.ToList();
                summarizedStatisticsPerServer.Add(new SummarizedDnsServerStatistics(serverStatistics));
            }

            summarizedStatisticsPerServer.Add(new SummarizedDnsServerStatistics(allServerStatistics) { ServerName = "all" });
            return summarizedStatisticsPerServer;
        }

        /// <summary>
        /// Get DNS server statistics for all servers
        /// </summary>
        /// <returns></returns>
        private async Task<List<DnsServerStatistics>> GetAllDnsServerStatistics()
        {
            var allDnsServerStatistics = new List<DnsServerStatistics>();

            foreach (var server in _dnsServersSettings.DnsServers)
                allDnsServerStatistics.AddRange(await _dnsServerStatisticsStore.GetServerStatisticsFromDate(server.ServerName, DateTime.UtcNow.AddHours(-HoursToSummarize)));

            return allDnsServerStatistics;
        }
    }
}
