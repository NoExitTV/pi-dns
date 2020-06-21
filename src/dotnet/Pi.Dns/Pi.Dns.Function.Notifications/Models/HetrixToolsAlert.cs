using Newtonsoft.Json;

namespace Pi.Dns.Function.Notifications.Models
{
    public class HetrixToolsAlert
    {
        [JsonProperty("monitor_id")]
        public string Monitor_id { get; set; }

        [JsonProperty("monitor_name")]
        public string Monitor_name { get; set; }

        [JsonProperty("monitor_target")]
        public string Monitor_target { get; set; }

        [JsonProperty("monitor_type")]
        public string Monitor_type { get; set; }

        [JsonProperty("monitor_category")]
        public string Monitor_category { get; set; }

        [JsonProperty("monitor_status")]
        public string Monitor_status { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("monitor_errors")]
        public MonitorErrors Monitor_errors { get; set; }

        public static class MonitorStatus
        {
            public static readonly string Online = "online";
            public static readonly string Offline = "offline";
        }
    }

    public class MonitorErrors
    {
        [JsonProperty("New York")]
        public string New_York { get; set; }

        [JsonProperty("San Francisco")]
        public string San_Francisco { get; set; }

        [JsonProperty("Dallas")]
        public string Dallas { get; set; }

        [JsonProperty("Amsterdam")]
        public string Amsterdam { get; set; }

        [JsonProperty("London")]
        public string London { get; set; }

        [JsonProperty("Frankfurt")]
        public string Frankfurt { get; set; }

        [JsonProperty("Singapore")]
        public string Singapore { get; set; }

        [JsonProperty("Sydney")]
        public string Sydney { get; set; }

        [JsonProperty("Sao Paulo")]
        public string Sao_Paulo { get; set; }

        [JsonProperty("Tokyo")]
        public string Tokyo { get; set; }

        [JsonProperty("Mumbai")]
        public string Mumbai { get; set; }

        [JsonProperty("Moscow")]
        public string Moscow { get; set; }
    }
}
