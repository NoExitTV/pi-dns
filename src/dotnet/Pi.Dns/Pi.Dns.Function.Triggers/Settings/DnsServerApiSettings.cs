using Destructurama.Attributed;
using System.Collections.Generic;

namespace Pi.Dns.Function.Triggers.Settings
{
    public class DnsServerApiSettings
    {
        public const string ConfigSectionName = "DnsServerApiSettings";
        public List<DnsServerSettingEntry> DnsServerApis { get; set; }
    }

    public class DnsServerSettingEntry
    {
        public string ServerName { get; set; }

        [LogMasked(ShowFirst = 3, PreserveLength = false)]
        public string ApiKey { get; set; }
    }
}
