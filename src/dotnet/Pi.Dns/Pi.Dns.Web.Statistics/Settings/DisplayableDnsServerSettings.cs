using System.Collections.Generic;

namespace Pi.Dns.Web.Statistics.Settings
{
    public class DisplayableDnsServerSettings
    {
        public const string ConfigSectionName = "DisplayableDnsServerSettings";

        public List<DisplayableDnsServerObject> DisplayableDnsServers { get; set; }
    }

    public class DisplayableDnsServerObject
    {
        public string ServerName { get; set; }
        public string DisplayName { get; set; }
    }
}
