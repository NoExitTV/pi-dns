using Pi.Dns.Common.Models;
using System.Threading.Tasks;

namespace Pi.Dns.Function.Triggers.Statistics
{
    public interface IDnsServerStatisticsRetreiver
    {
        /// <summary>
        /// Get DNS server statistics from a given Pi-Dns server
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        Task<DnsServerStatistics> GetStatistics(string serverName, string apiKey);
    }
}
