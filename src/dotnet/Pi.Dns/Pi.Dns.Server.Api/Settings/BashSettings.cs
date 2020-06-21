namespace Pi.Dns.Server.Api.Settings
{
    public class BashSettings
    {
        public const string SectionName = "BashSettings";

        public string UnboundControlCmd { get; set; }
        public string DomainsOnBlocklistCmd { get; set; }
    }
}
