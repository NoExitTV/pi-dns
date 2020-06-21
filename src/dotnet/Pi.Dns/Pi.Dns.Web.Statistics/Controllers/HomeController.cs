using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Pi.Dns.Common.Models;
using Pi.Dns.Common.Stores;
using Pi.Dns.Web.Statistics.Models;
using Pi.Dns.Web.Statistics.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pi.Dns.Web.Statistics.Controllers
{
    public class HomeController : Controller
    {
        // Settings
        private readonly DisplayableDnsServerSettings _dnsServersSettings;

        // Dependency injection
        private readonly IDnsServerStatisticsStore _dnsServerStatisticsStore;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger _logger;

        public HomeController(
            IOptions<DisplayableDnsServerSettings> dnsServersSetting,
            IDnsServerStatisticsStore dnsServerStatisticsStore,
            IMemoryCache memoryCache)
        {
            _dnsServersSettings = dnsServersSetting.Value;
            _dnsServerStatisticsStore = dnsServerStatisticsStore;
            _memoryCache = memoryCache;
            _logger = Log.ForContext("SourceContext", nameof(HomeController));
        }

        public async Task<IActionResult> IndexAsync(string server)
        {
            try
            {
                _logger.Information("Preparing statistics for server {Server}", server);

                if (!ValidateServer(server))
                    return View(new HomeViewModel(_dnsServersSettings));

                List<DnsServerStatistics> serverStatisticsHistory;

                if (server == "all")
                {
                    serverStatisticsHistory = await GetAllDnsServerStatistics();

                    if (serverStatisticsHistory == null || serverStatisticsHistory.Count == 0)
                    {
                        return View(new HomeViewModel(_dnsServersSettings));
                    }

                    serverStatisticsHistory = AggregateAllServerStatistics(serverStatisticsHistory);
                }
                else
                {
                    serverStatisticsHistory = await GetDnsServerStatistics(server);

                    if (serverStatisticsHistory == null || serverStatisticsHistory.Count == 0)
                    {
                        return View(new HomeViewModel(_dnsServersSettings));
                    }
                }


                // Prepare statistics for model
                var queryTypeDimensions = new List<string>
                {
                    "A",
                    "SOA",
                    "Null",
                    "TXT",
                    "AAA",
                    "SRV",
                    "DNSKEY",
                    "ANY"
                };

                var queryTypeValues = new List<int>
                {
                    serverStatisticsHistory.Sum(s => s.NumQueryTypeA),
                    serverStatisticsHistory.Sum(s => s.NumQueryTypeSOA),
                    serverStatisticsHistory.Sum(s => s.NumQueryTypeNull),
                    serverStatisticsHistory.Sum(s => s.NumQueryTypeTXT),
                    serverStatisticsHistory.Sum(s => s.NumQueryTypeAAA),
                    serverStatisticsHistory.Sum(s => s.NumQueryTypeSRV),
                    serverStatisticsHistory.Sum(s => s.NumQueryTypeDNSKEY),
                    serverStatisticsHistory.Sum(s => s.NumQueryTypeAny)
                };

                var answerTypeDimensions = new List<string>
                {
                    "NOERROR",
                    "FORMERR",
                    "SERVFAIL",
                    "NXDOMAIN",
                    "NOTIMPL",
                    "REFUSED",
                    "NODATA"
                };

                var answerTypeValues = new List<int>
                {
                    serverStatisticsHistory.Sum(s => s.NumAnswerNOERROR),
                    serverStatisticsHistory.Sum(s => s.NumAnswerFORMERR),
                    serverStatisticsHistory.Sum(s => s.NumAnswerSERVFAIL),
                    serverStatisticsHistory.Sum(s => s.NumAnswerNXDOMAIN),
                    serverStatisticsHistory.Sum(s => s.NumAnswerNOTIMPL),
                    serverStatisticsHistory.Sum(s => s.NumAnswerREFUSED),
                    serverStatisticsHistory.Sum(s => s.NumAnswerNODATA)
                };

                // Fill model with statistics
                var homeViewModel = new HomeViewModel(_dnsServersSettings)
                {
                    ServerName = serverStatisticsHistory.FirstOrDefault()?.ServerName,
                    ExtendedStatistics = serverStatisticsHistory,
                    QueryTypeDimensions = queryTypeDimensions,
                    QueryTypeValues = queryTypeValues,
                    AnswerTypeDimensions = answerTypeDimensions,
                    AnswerTypeValues = answerTypeValues
                };

                _logger.Debug("Created a HomeViewModel with {Count} point of statistics", serverStatisticsHistory?.Count);

                return View(homeViewModel);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Got an exception while preparing statistics for server {Server}", server);
                throw;
            }
        }

        /// <summary>
        /// Ugly validation of server
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        private bool ValidateServer(string server)
        {
            return _dnsServersSettings.DisplayableDnsServers.Any(s => s.ServerName == server);
        }

        /// <summary>
        /// Get DNS server statistics for a single server.
        /// First, try get statistics from memory cache, and if not exist, query storage
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        private async Task<List<DnsServerStatistics>> GetDnsServerStatistics(string server)
        {
            if (!_memoryCache.TryGetValue(server, out IEnumerable<DnsServerStatistics> dnsServerStatistics))
            {
                dnsServerStatistics = await _dnsServerStatisticsStore.GetServerStatisticsFromDate(server, DateTime.UtcNow.AddDays(-1));
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(20)).SetSlidingExpiration(TimeSpan.FromMinutes(10));
                _memoryCache.Set(server, dnsServerStatistics, cacheEntryOptions);
            }

            return dnsServerStatistics?.ToList();
        }

        /// <summary>
        /// Get DNS server statistics for all servers
        /// </summary>
        /// <returns></returns>
        private async Task<List<DnsServerStatistics>> GetAllDnsServerStatistics()
        {
            var allDnsServerStatistics = new List<DnsServerStatistics>();

            foreach (var server in _dnsServersSettings.DisplayableDnsServers)
            {
                if (server.ServerName != "all")
                    allDnsServerStatistics.AddRange(await GetDnsServerStatistics(server.ServerName));
            }

            return allDnsServerStatistics;
        }

        /// <summary>
        /// Group and concatenate all DNS server statistics increments on CreatedDate
        /// </summary>
        /// <param name="serverStatistics"></param>
        /// <returns></returns>
        private List<DnsServerStatistics> AggregateAllServerStatistics(List<DnsServerStatistics> serverStatistics)
        {
            // Hack, remove seconds and milliseconds from DateTime when grouping
            var groupedByTimestamp = serverStatistics.GroupBy(s => new DateTime(s.CreatedDate.AddSeconds(-s.CreatedDate.Second).Ticks - (s.CreatedDate.Ticks % TimeSpan.TicksPerSecond), s.CreatedDate.Kind));
            var concatenatedStatistics = new List<DnsServerStatistics>();

            foreach (var group in groupedByTimestamp)
            {
                var newStat = new DnsServerStatistics
                {
                    ServerName = "all",
                    CreatedDate = group.Key
                };

                foreach (var property in typeof(DnsServerStatistics).GetProperties())
                {
                    if (property.PropertyType == typeof(int))
                    {
                        var value = group.Sum(s => (int)s.GetType().GetProperty(property.Name).GetValue(s));
                        property.SetValue(newStat, value);
                    }
                    else if (property.PropertyType == typeof(double))
                    {
                        var value = group.Sum(s => (double)s.GetType().GetProperty(property.Name).GetValue(s));
                        property.SetValue(newStat, value);
                    }
                }

                newStat.DomainsOnBlocklist /= group.Count();
                concatenatedStatistics.Add(newStat);
            }

            return concatenatedStatistics.OrderBy(stats => stats.CreatedDate)?.ToList();
        }
    }
}