using Pi.Dns.Common.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pi.Dns.Common.Stores
{
    public interface IDnsServerStatisticsStore
    {
        /// <summary>
        /// Add DNS server statistics to storage
        /// </summary>
        /// <param name="dnsServerStatistics"></param>
        /// <returns></returns>
        Task Add(DnsServerStatistics dnsServerStatistics);

        /// <summary>
        /// Get all DNS server statistics for a given server newer than a specific date
        /// </summary>
        /// <param name="server"></param>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        Task<IOrderedEnumerable<DnsServerStatistics>> GetServerStatisticsFromDate(string server, DateTime fromDate);
    }
}
