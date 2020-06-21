using Pi.Dns.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pi.Dns.Function.Triggers.Statistics
{
    public interface IDnsServerStatisticsSender
    {
        Task SendSummarizedStatistics(IEnumerable<SummarizedDnsServerStatistics> summarizedStatisticsPerServer);
    }
}
