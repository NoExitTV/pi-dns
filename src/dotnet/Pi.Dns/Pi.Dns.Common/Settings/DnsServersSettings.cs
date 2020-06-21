using System.Collections.Generic;

namespace Pi.Dns.Common.Settings
{
    public class DnsServersSettings
    {
        public const string ConfigSectionName = "DnsServersSettings";

        public List<DnsServerSetting> DnsServers { get; set; }
    }

    public class DnsServerSetting
    {
        public string ServerName { get; set; }
    }
}
