using Microsoft.Azure.WebJobs;
using Pi.Dns.Function.Triggers.Statistics;
using Serilog;
using System.Threading.Tasks;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Pi.Dns.Function.Triggers.Functions
{
    public static class ServerStatisticsIngress
    {
        public const string FunctionName = nameof(ServerStatisticsIngress);

        /// <summary>
        /// Trigger ingress of DNS server statistics every 15th minute
        /// </summary>
        /// <param name="timerInfo"></param>
        /// <param name="serverStatisticsIngressor"></param>
        /// <param name="serverStatisticsSummarizer"></param>
        /// <param name="dnsServerStatisticsSender"></param>
        /// <returns></returns>
        [FunctionName(FunctionName)]
        public static async Task Run(
            [TimerTrigger("0 */15 * * * *")] TimerInfo timerInfo,
            [Inject] IServerStatisticsIngressor serverStatisticsIngressor,
            [Inject] IServerStatisticsSummarizer serverStatisticsSummarizer,
            [Inject] IDnsServerStatisticsSender dnsServerStatisticsSender)
        {
            var logger = Log.ForContext("SourceContext", FunctionName);
            logger.Information("Executing function {Function}", FunctionName);
            await serverStatisticsIngressor.IngressDnsServerStatistics();
            var summarizedStatisticsPerServer = await serverStatisticsSummarizer.Summarize24h();
            await dnsServerStatisticsSender.SendSummarizedStatistics(summarizedStatisticsPerServer);
            logger.Information("Function {Function} executed successfully", FunctionName);
        }
    }
}
