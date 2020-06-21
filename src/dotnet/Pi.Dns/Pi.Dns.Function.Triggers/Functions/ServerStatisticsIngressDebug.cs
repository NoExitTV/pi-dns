#if DEBUG
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Pi.Dns.Function.Triggers.Statistics;
using Serilog;
using System.Threading.Tasks;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Pi.Dns.Function.Triggers.Functions
{
    public static class ServerStatisticsIngressDebug
    {
        [FunctionName("ServerStatisticsIngressDebug")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Inject] IServerStatisticsIngressor serverStatisticsIngressor,
            [Inject] IServerStatisticsSummarizer serverStatisticsSummarizer,
            [Inject] IDnsServerStatisticsSender dnsServerStatisticsSender)
        {
            var logger = Log.ForContext("SourceContext", nameof(ServerStatisticsIngressDebug));
            await ServerStatisticsIngress.Run(new TimerInfo(null, null), serverStatisticsIngressor, serverStatisticsSummarizer, dnsServerStatisticsSender);
            return new OkObjectResult("OK");
        }
    }
}
#endif
