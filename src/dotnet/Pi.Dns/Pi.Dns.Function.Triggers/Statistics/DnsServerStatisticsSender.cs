using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pi.Dns.Common.Models;
using Pi.Dns.Function.Triggers.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pi.Dns.Function.Triggers.Statistics
{
    public class DnsServerStatisticsSender : IDnsServerStatisticsSender
    {
        private readonly PiDnsWebApiSettings _piDnsWebApiSettings;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public DnsServerStatisticsSender(
            IOptions<PiDnsWebApiSettings> piDnsWebApiSettings,
            HttpClient httpClient)
        {
            _piDnsWebApiSettings = piDnsWebApiSettings.Value;
            _httpClient = httpClient;
            _logger = Log.ForContext("SourceContext", nameof(DnsServerStatisticsSender));
        }

        public async Task SendSummarizedStatistics(IEnumerable<SummarizedDnsServerStatistics> summarizedStatisticsPerServer)
        {
            var simultaneousTasks = new List<Task>();

            foreach (var summarizedServerStatistics in summarizedStatisticsPerServer)
            {
                _logger.Information("Sending DNS server statistics from server {Server}", summarizedServerStatistics?.ServerName);

                try
                {
                    var queryParameters = new Dictionary<string, string>
                    {
                        { "api_key", _piDnsWebApiSettings.ApiKey }
                    };

                    var requestUri = QueryHelpers.AddQueryString(_piDnsWebApiSettings.Url, queryParameters);
                    var serializedContent = JsonConvert.SerializeObject(summarizedServerStatistics);
                    var content = new StringContent(serializedContent, Encoding.UTF8, "application/json");
                    _logger.Information("Sending POST request to {Url} with content {@Content}", _piDnsWebApiSettings.Url, content);
                    simultaneousTasks.Add(_httpClient.PostAsync(requestUri, content));
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Got a unhandled exception while sending DNS statistics for server {Server}", summarizedServerStatistics?.ServerName);
                }
            }

            await Task.WhenAll(simultaneousTasks);
        }
    }
}
