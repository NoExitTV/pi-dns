using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pi.Dns.Function.Notifications.Models;
using Pi.Dns.Function.Notifications.Settings;
using Serilog;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pi.Dns.Function.Notifications.AlertClients
{
    public class TelegramAlertClient : IAlertClient
    {
        public const string IntegrationName = "Telegram";
        public string Integration => IntegrationName;

        private readonly ILogger _logger;
        private readonly TelegramSettings _telegramSettings;
        private readonly HttpClient _httpClient;

        public TelegramAlertClient(
            HttpClient httpClient,
            IOptions<TelegramSettings> telegramSettings)
        {
            _logger = Log.ForContext("SourceContext", nameof(TelegramAlertClient));
            _httpClient = httpClient;
            _telegramSettings = telegramSettings.Value;
        }

        /// <summary>
        /// Send pidns query statistics to Telegram
        /// </summary>
        /// <param name="totalDnsRequests"></param>
        /// <param name="totalAdsBlocked"></param>
        /// <param name="printableTimeSpan"></param>
        /// <returns></returns>
        public async Task<bool> TrySendHetrixToolsAlert(HetrixToolsAlert hetrixToolsAlert)
        {
            try
            {
                var message = "";

                // Down
                if (hetrixToolsAlert.Monitor_status == HetrixToolsAlert.MonitorStatus.Offline)
                {
                    message = $"<b>{hetrixToolsAlert.Monitor_name}</b> has gone <strong>{hetrixToolsAlert.Monitor_status.ToUpper()}</strong>\nTarget: <b>{hetrixToolsAlert.Monitor_target}</b>\nMonitor Type: {hetrixToolsAlert.Monitor_type}\nMonitor Reports: \n";

                    foreach (var obj in hetrixToolsAlert.Monitor_errors.GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                   .Where(p => !p.GetIndexParameters().Any())
                                   .Where(p => p.CanRead && p.CanWrite))
                    {
                        var value = obj.GetValue(hetrixToolsAlert.Monitor_errors, null);
                        if (value != null)
                            message += $"\t-{obj.Name}: {value}\n";
                    }
                }
                // Up
                else if (hetrixToolsAlert.Monitor_status == HetrixToolsAlert.MonitorStatus.Online)
                {
                    message = $"<b>{hetrixToolsAlert.Monitor_name}</b> is now <strong>{hetrixToolsAlert.Monitor_status.ToUpper()}</strong>\nTarget: <b>{hetrixToolsAlert.Monitor_target}</b>\nType: {hetrixToolsAlert.Monitor_type}";
                }

                var telegramRequest = new TelegramRequest(_telegramSettings.TelegramChannel, "HTML", message);
                var telegramResponse = await _httpClient.PostAsync(new Uri($"{_telegramSettings.TelegramUrl}/bot{_telegramSettings.Token}/sendMessage"), new StringContent(JsonConvert.SerializeObject(telegramRequest), Encoding.UTF8, "application/json"));

                return telegramResponse.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Got an exception while sending hetrix tools alert to Telegram");
                return false;
            }
        }

        public async Task<bool> TrySendPiDnsStatistics(long totalDnsRequests, long totalAdsBlocked, string printableTimeSpan)
        {
            try
            {
                var message = $"During the last {printableTimeSpan}, <a href=\"https://pi-dns.com\">pi-dns.com</a> have served <b>{totalDnsRequests.ToString("n0", new CultureInfo("en-US"))}</b> DNS requests and blocked <b>{totalAdsBlocked.ToString("n0", new CultureInfo("en-US"))}</b> ads!";
                var telegramRequest = new TelegramRequest(_telegramSettings.TelegramChannel, "HTML", message);
                var telegramResponse = await _httpClient.PostAsync($"{_telegramSettings.TelegramUrl}/bot{_telegramSettings.Token}/sendMessage", new StringContent(JsonConvert.SerializeObject(telegramRequest), Encoding.UTF8, "application/json"));
                return telegramResponse.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Got an exception while sending pidns statistics alert to Telegram");
                return false;
            }
        }
    }
}
