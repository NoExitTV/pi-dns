using Destructurama.Attributed;

namespace Pi.Dns.Function.Triggers.Settings
{
    public class PiDnsWebApiSettings
    {
        public const string ConfigSectionName = "PiDnsWebApiSettings";
        public string Url { get; set; }

        [LogMasked(ShowFirst = 3, PreserveLength = false)]
        public string ApiKey { get; set; }
    }
}
