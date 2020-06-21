namespace Pi.Dns.Function.Notifications.Settings
{
    public class TelegramSettings
    {
        public const string SectionName = "TelegramSettings";

        public string Token { get; set; }
        public string TelegramUrl { get; set; }
        public string TelegramChannel { get; set; }
    }
}
