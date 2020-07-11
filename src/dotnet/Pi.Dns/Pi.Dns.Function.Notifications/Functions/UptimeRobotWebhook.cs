using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Pi.Dns.Function.Notifications.AlertClients;
using Pi.Dns.Function.Notifications.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Pi.Dns.Function.Notifications.Functions
{
    public static class UptimeRobotWebhook
    {
        public const string FunctionName = nameof(UptimeRobotWebhook);

        [FunctionName(FunctionName)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Inject] IEnumerable<IAlertClient> alertClients)
        {
            var logger = Log.ForContext("SourceContext", FunctionName);

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (string.IsNullOrEmpty(requestBody))
                    throw new ArgumentNullException("Request body can not be null or empty");

                var uptimeRobotAlert = JsonConvert.DeserializeObject<UptimeRobotAlert>(requestBody);

                // Send alert to all known alert clients
                foreach (var alertClient in alertClients)
                {
                    logger.Information("Sending UptimeRobotAlert to {Integration} client", alertClient.Integration);
                    if (!(await alertClient.TrySendUptimeRobotAlert(uptimeRobotAlert)))
                        logger.Warning("AlertClient {Client} returned false when sending UptimeRobotAlert {@Alert}, indicating a failure", alertClient.Integration, uptimeRobotAlert);
                }

                return new OkObjectResult("Ok");
            }
            catch (Exception e)
            {
                logger.Error(e, "Got an exception while processing hetrix tools alert");
                return new StatusCodeResult(500); // Always respond with HTTP status code 500 for now
            }
        }
    }
}
