using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using Pi.Dns.Common.Models;
using System;

namespace Pi.Dns.Common.Stores.Storage
{
    public class DnsServerStatisticsEntity : TableEntity
    {
        [JsonIgnore]
        public DnsServerStatistics DnsServerStatistics { get; set; }

        public string DnsServerStatisticsJson
        {
            get { return (DnsServerStatistics != null) ? JsonConvert.SerializeObject(DnsServerStatistics, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None }) : null; }
            set { DnsServerStatistics = (value != null) ? JsonConvert.DeserializeObject<DnsServerStatistics>(value) : null; }
        }

        public DateTime CreatedDate { get; set; }

        public DnsServerStatisticsEntity()
        {
        }

        public DnsServerStatisticsEntity(DnsServerStatistics dnsServerStatistics) : base($"{dnsServerStatistics.ServerName}:{dnsServerStatistics.CreatedDate:yyyy-MM}", $"{dnsServerStatistics.CreatedDate:o}")
        {
            DnsServerStatistics = dnsServerStatistics;
            CreatedDate = DnsServerStatistics.CreatedDate;
        }
    }
}
