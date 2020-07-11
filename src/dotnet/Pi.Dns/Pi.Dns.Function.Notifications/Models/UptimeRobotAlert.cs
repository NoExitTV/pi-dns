using Pi.Dns.Common.Models;

namespace Pi.Dns.Function.Notifications.Models
{
    public class UptimeRobotAlert
    {
        public string Id { get; set; }
        public string FriendlyName { get; set; }
        public string Url { get; set; }
        public int AlertType { get; set; }
        public string AlertTypeFriendlyName { get; set; }
        public string AlertDetails { get; set; }
        public int AlertDuration { get; set; }

        public static class AlertTypes
        {
            public static readonly int Down = 1;
            public static readonly int Up = 2;
            public static readonly int SSLExpiry = 3;
        }
    }
}
