using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Options;
using Pi.Dns.Common.Models;
using Pi.Dns.Common.Settings;
using Pi.Dns.Common.Stores;
using Pi.Dns.Function.Notifications.AlertClients;
using Pi.Dns.Function.Notifications.Extensions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Pi.Dns.Function.Notifications.Functions
{
    public static class PostDnsStatistics
    {
        public const string FunctionName = nameof(PostDnsStatistics);

        [FunctionName(FunctionName)]
        public static async Task Run(
            [TimerTrigger("0 0 19 * * *")] TimerInfo myTimer,
            [Inject] IOptions<DnsServersSettings> dnsServersSettings,
            [Inject] IDnsServerStatisticsStore dnsServerStatisticsStore,
            [Inject] IEnumerable<IAlertClient> alertClients)
        {
            var logger = Log.ForContext("SourceContext", FunctionName);

            try
            {
                var hoursOfIncrements = DateTime.UtcNow.GetHoursOfIncrements();
                var printableTimeSpan = hoursOfIncrements.PrintableTimeSpan();

                // Get statistics for all servers
                var allDnsServerStatistics = new List<DnsServerStatistics>();

                foreach (var server in dnsServersSettings.Value.DnsServers)
                    allDnsServerStatistics.AddRange(await dnsServerStatisticsStore.GetServerStatisticsFromDate(server.ServerName, DateTime.UtcNow.AddHours(-hoursOfIncrements)));

                if (allDnsServerStatistics.Count == 0)
                    throw new InvalidOperationException($"Did not get any statistical increments for pidns servers for hours {hoursOfIncrements}");

                var totalNumQueries = allDnsServerStatistics.Sum(s => s.TotalNumQueries);
                var totalAdsBlocked = allDnsServerStatistics.Sum(s => s.NumAnswerNXDOMAIN);

                foreach (var alertClient in alertClients)
                {
                    logger.Information("Sending pidns statistics totalNumQueries {TotalNumQueries} and totalAdsBlocked {TotalAdsBlocked} to {Integration} client",
                        totalNumQueries, totalAdsBlocked, alertClient.Integration);

                    if (!(await alertClient.TrySendPiDnsStatistics(totalNumQueries, totalAdsBlocked, printableTimeSpan)))
                        logger.Warning("AlertClient {Client} returned false when sending pidns statistics alert, indicating a failure", alertClient.Integration);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Got an exception while processing pidns statistics alert");
            }
        }
    }
}
