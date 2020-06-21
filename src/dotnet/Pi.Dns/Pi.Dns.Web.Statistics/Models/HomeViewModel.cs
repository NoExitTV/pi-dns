using Pi.Dns.Common.Models;
using Pi.Dns.Web.Statistics.Settings;
using System.Collections.Generic;

namespace Pi.Dns.Web.Statistics.Models
{
    public class HomeViewModel
    {
        public HomeViewModel(DisplayableDnsServerSettings displayableDnsServerSettings)
        {
            DisplayableDnsServerSettings = displayableDnsServerSettings;
        }

        public string ServerName { get; set; }
        public DisplayableDnsServerSettings DisplayableDnsServerSettings { get; set; }
        public IEnumerable<DnsServerStatistics> ExtendedStatistics { get; set; }

        public IEnumerable<string> QueryTypeDimensions { get; set; }
        public IEnumerable<int> QueryTypeValues { get; set; }

        public IEnumerable<string> AnswerTypeDimensions { get; set; }
        public IEnumerable<int> AnswerTypeValues { get; set; }
    }
}
