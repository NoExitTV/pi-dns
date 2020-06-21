using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Pi.Dns.Common.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pi.Dns.Function.Triggers.Statistics
{
    public class DnsServerStatisticsRetreiver : IDnsServerStatisticsRetreiver
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public DnsServerStatisticsRetreiver(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _logger = Log.ForContext("SourceContext", nameof(DnsServerStatisticsRetreiver));
        }

        /// <summary>
        /// Get DNS server statistics from a given Pi-Dns server
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public async Task<DnsServerStatistics> GetStatistics(string serverName, string apiKey)
        {
            _logger.Information("Getting DNS server statistics from server {Server}", serverName);

            try
            {
                var queryParameters = new Dictionary<string, string>
                {
                    { "api_key", apiKey }
                };

                var apiUrl = $"https://{serverName}.pi-dns.com/UnboundControlStats";
                var requestUri = QueryHelpers.AddQueryString(apiUrl, queryParameters);
                _logger.Information("Sending GET request to {Url}", apiUrl);
                var httpResponse = await _httpClient.GetAsync(requestUri);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.Warning("Got HTTP status code {StatusCode} indicating error", httpResponse.StatusCode);
                    return null;
                }

                var result = JsonConvert.DeserializeObject<DnsServerStatistics>(await httpResponse.Content.ReadAsStringAsync());
                result.ServerName = serverName;
                result.CreatedDate = DateTime.UtcNow;
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Got a unhandled exception while getting DNS statistics for server {Server}", serverName);
                return null;
            }
        }
    }
}
