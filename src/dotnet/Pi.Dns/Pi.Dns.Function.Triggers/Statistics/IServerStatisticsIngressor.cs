using System.Threading.Tasks;

namespace Pi.Dns.Function.Triggers.Statistics
{
    public interface IServerStatisticsIngressor
    {
        /// <summary>
        /// Fetch DNS server statistics from all DNS servers and ingress to storage account
        /// </summary>
        /// <returns></returns>
        Task IngressDnsServerStatistics();
    }
}
