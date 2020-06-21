namespace Pi.Dns.Function.Notifications.Settings
{
    public class TwitterSettings
    {
        public const string SectionName = "TwitterSettings";

        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
    }
}
