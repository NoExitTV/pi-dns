using Pi.Dns.Function.Notifications.Models;
using System.Threading.Tasks;

namespace Pi.Dns.Function.Notifications.AlertClients
{
    public interface IAlertClient
    {
        /// <summary>
        /// What does the IAlertClient integrate with? I.e. Telegra, Twitter
        /// </summary>
        string Integration { get; }

        /// <summary>
        /// Try send uptime notification form hetrix tools
        /// </summary>
        /// <param name="hetrixToolsAlert"></param>
        /// <returns></returns>
        Task<bool> TrySendHetrixToolsAlert(HetrixToolsAlert hetrixToolsAlert);

        /// <summary>
        /// Try send pidns query statistics
        /// </summary>
        /// <param name="totalDnsRequests"></param>
        /// <param name="totalAdsBlocked"></param>
        /// <param name="printableTimeSpan"></param>
        /// <returns></returns>
        Task<bool> TrySendPiDnsStatistics(long totalDnsRequests, long totalAdsBlocked, string printableTimeSpan);
    }
}
