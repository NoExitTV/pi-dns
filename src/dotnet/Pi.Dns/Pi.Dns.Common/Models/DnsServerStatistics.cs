using System;

namespace Pi.Dns.Common.Models
{
    public class DnsServerStatistics : UnboundControlStats
    {
        public string ServerName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
