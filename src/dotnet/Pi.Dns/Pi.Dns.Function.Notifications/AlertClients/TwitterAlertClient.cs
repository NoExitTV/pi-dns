using Microsoft.Extensions.Options;
using Pi.Dns.Function.Notifications.Models;
using Pi.Dns.Function.Notifications.Settings;
using Serilog;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Tweetinvi;

namespace Pi.Dns.Function.Notifications.AlertClients
{
    public class TwitterAlertClient : IAlertClient
    {
        public const string IntegrationName = "Twitter";
        public string Integration => IntegrationName;

        private readonly ILogger _logger;
        private readonly TwitterSettings _twitterSettings;

        public TwitterAlertClient(
            IOptions<TwitterSettings> twitterSettings)
        {
            _logger = Log.ForContext("SourceContext", nameof(TwitterAlertClient));
            _twitterSettings = twitterSettings.Value;
        }

        /// <summary>
        /// Send hetrix tools uptime notification to Twitter
        /// </summary>
        /// <param name="hetrixToolsAlert"></param>
        /// <returns></returns>
        public Task<bool> TrySendHetrixToolsAlert(HetrixToolsAlert hetrixToolsAlert)
        {
            try
            {
                Auth.SetUserCredentials(_twitterSettings.ConsumerKey, _twitterSettings.ConsumerSecret, _twitterSettings.AccessToken, _twitterSettings.AccessTokenSecret);
                var tweet = "";

                // Down
                if (hetrixToolsAlert.Monitor_status == HetrixToolsAlert.MonitorStatus.Offline)
                {
                    tweet = $"{hetrixToolsAlert.Monitor_name} has gone {hetrixToolsAlert.Monitor_status.ToUpper()}\nTarget: {hetrixToolsAlert.Monitor_target}\nMonitor Type: {hetrixToolsAlert.Monitor_type}\n";
                }
                // Up
                else if (hetrixToolsAlert.Monitor_status == HetrixToolsAlert.MonitorStatus.Online)
                {
                    tweet = $"{hetrixToolsAlert.Monitor_name} is now {hetrixToolsAlert.Monitor_status.ToUpper()}\nTarget: {hetrixToolsAlert.Monitor_target}\nType: {hetrixToolsAlert.Monitor_type}";
                }

                var publishedTweet = Tweet.PublishTweet(tweet);
                return Task.FromResult(publishedTweet?.Id != null && publishedTweet?.Id != default);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Got an exception while sending hetrix tools alert to Twitter");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Send pidns query statistics to Twitter
        /// </summary>
        /// <param name="totalDnsRequests"></param>
        /// <param name="totalAdsBlocked"></param>
        /// <param name="printableTimeSpan"></param>
        /// <returns></returns>
        public Task<bool> TrySendPiDnsStatistics(long totalDnsRequests, long totalAdsBlocked, string printableTimeSpan)
        {
            try
            {
                Auth.SetUserCredentials(_twitterSettings.ConsumerKey, _twitterSettings.ConsumerSecret, _twitterSettings.AccessToken, _twitterSettings.AccessTokenSecret);
                var tweet = $"During the last {printableTimeSpan}, pi-dns.com have served {totalDnsRequests.ToString("n0", new CultureInfo("en-US"))} DNS requests and blocked {totalAdsBlocked.ToString("n0", new CultureInfo("en-US"))} ads!\n";

                // For now, just hard-code a few hashtags. Make these configurable later
                tweet += "\n#pidns #adblock #DNS #DNSoverHTTPS #DNSoverTLS #DoH #DoT #EncryptedDNS #adblockDNS #Privacy #Ads #pihole";

                var publishedTweet = Tweet.PublishTweet(tweet);
                return Task.FromResult(publishedTweet?.Id != null && publishedTweet?.Id != default);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Got an exception while sending pidns statistics alert to Twitter");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Send UptimeRobot uptime notification to Twitter
        /// </summary>
        /// <param name="uptimeRobotAlert"></param>
        /// <returns></returns>
        public Task<bool> TrySendUptimeRobotAlert(UptimeRobotAlert uptimeRobotAlert)
        {
            try
            {
                Auth.SetUserCredentials(_twitterSettings.ConsumerKey, _twitterSettings.ConsumerSecret, _twitterSettings.AccessToken, _twitterSettings.AccessTokenSecret);
                var tweet = "";

                // Down
                if (uptimeRobotAlert.AlertType == UptimeRobotAlert.AlertTypes.Down)
                {
                    tweet = $"{uptimeRobotAlert.FriendlyName} has gone {uptimeRobotAlert.AlertTypeFriendlyName.ToUpper()}\n";
                }
                // Up
                else if (uptimeRobotAlert.AlertType == UptimeRobotAlert.AlertTypes.Up)
                {
                    tweet = $"{uptimeRobotAlert.FriendlyName} is now {uptimeRobotAlert.AlertTypeFriendlyName.ToUpper()}\nDown for: {uptimeRobotAlert.AlertDuration} seconds\n";
                }

                tweet += (!string.IsNullOrEmpty(uptimeRobotAlert.Url)) ? $"Target: {uptimeRobotAlert.Url}\n" : "";
                tweet += (!string.IsNullOrEmpty(uptimeRobotAlert.AlertDetails)) ? $"Details: {uptimeRobotAlert.AlertDetails}\n" : "";

                var publishedTweet = Tweet.PublishTweet(tweet);
                return Task.FromResult(publishedTweet?.Id != null && publishedTweet?.Id != default);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Got an exception while sending UptimeRobot alert to Twitter");
                return Task.FromResult(false);
            }
        }
    }
}
