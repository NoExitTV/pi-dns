#if DEBUG
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Options;
using Pi.Dns.Common.Settings;
using Pi.Dns.Common.Stores;
using Pi.Dns.Function.Notifications.AlertClients;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Pi.Dns.Function.Notifications.Functions
{
    public static class PostDnsStatisticsDebug
    {
        [FunctionName("PostDnsStatisticsDebug")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Inject] IOptions<DnsServersSettings> dnsServersSettings,
            [Inject] IDnsServerStatisticsStore dnsServerStatisticsStore,
            [Inject] IEnumerable<IAlertClient> alertClients)
        {
            var logger = Log.ForContext("SourceContext", nameof(PostDnsStatisticsDebug));
            await PostDnsStatistics.Run(new TimerInfo(null, null), dnsServersSettings, dnsServerStatisticsStore, alertClients);
            return new OkObjectResult("OK");
        }
    }
}
# endif
