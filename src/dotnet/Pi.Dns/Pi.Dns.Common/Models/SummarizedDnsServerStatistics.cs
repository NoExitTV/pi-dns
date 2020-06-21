using System;
using System.Collections.Generic;
using System.Linq;

namespace Pi.Dns.Common.Models
{
    public class SummarizedDnsServerStatistics
    {
        public string ServerName { get; set; }
        public DateTime CreatedDate { get; set; }

        // Total
        public int TotalNumQueries { get; set; }
        public int TotalNumCacheHits { get; set; }
        public int TotalNumCacheMiss { get; set; }
        public double TotalRecursionTimeAvg { get; set; }

        // Num query
        public int NumQueryTypeA { get; set; }
        public int NumQueryTypeSOA { get; set; }
        public int NumQueryTypeNull { get; set; }
        public int NumQueryTypeTXT { get; set; }
        public int NumQueryTypeAAA { get; set; }
        public int NumQueryTypeSRV { get; set; }
        public int NumQueryTypeDNSKEY { get; set; }
        public int NumQueryTypeAny { get; set; }

        // Num answer
        public int NumAnswerNOERROR { get; set; }
        public int NumAnswerFORMERR { get; set; }
        public int NumAnswerSERVFAIL { get; set; }
        public int NumAnswerNXDOMAIN { get; set; }
        public int NumAnswerNOTIMPL { get; set; }
        public int NumAnswerREFUSED { get; set; }
        public int NumAnswerNODATA { get; set; }

        // Extra
        public double QueriesPerSecondAvg { get; set; }
        public int DomainsOnBlocklist { get; set; }
        public int DataPoints { get; set; }

        public SummarizedDnsServerStatistics()
        {

        }

        public SummarizedDnsServerStatistics(IEnumerable<DnsServerStatistics> dnsServerStatistics)
        {
            var numberOfDataPoints = dnsServerStatistics.Count();

            ServerName = dnsServerStatistics.First().ServerName;
            CreatedDate = DateTime.UtcNow;

            // Total
            TotalNumQueries = dnsServerStatistics.Sum(s => s.TotalNumQueries);
            TotalNumCacheHits = dnsServerStatistics.Sum(s => s.TotalNumCacheHits);
            TotalNumCacheMiss = dnsServerStatistics.Sum(s => s.TotalNumCacheMiss);
            TotalRecursionTimeAvg = dnsServerStatistics.Sum(s => s.TotalRecursionTimeAvg) / numberOfDataPoints;

            // Num query
            NumQueryTypeA = dnsServerStatistics.Sum(s => s.NumQueryTypeA);
            NumQueryTypeSOA = dnsServerStatistics.Sum(s => s.NumQueryTypeSOA);
            NumQueryTypeNull = dnsServerStatistics.Sum(s => s.NumQueryTypeNull);
            NumQueryTypeTXT = dnsServerStatistics.Sum(s => s.NumQueryTypeTXT);
            NumQueryTypeAAA = dnsServerStatistics.Sum(s => s.NumQueryTypeAAA);
            NumQueryTypeSRV = dnsServerStatistics.Sum(s => s.NumQueryTypeSRV);
            NumQueryTypeDNSKEY = dnsServerStatistics.Sum(s => s.NumQueryTypeDNSKEY);
            NumQueryTypeAny = dnsServerStatistics.Sum(s => s.NumQueryTypeAny);

            // Num answer
            NumAnswerNOERROR = dnsServerStatistics.Sum(s => s.NumAnswerNOERROR);
            NumAnswerFORMERR = dnsServerStatistics.Sum(s => s.NumAnswerFORMERR);
            NumAnswerSERVFAIL = dnsServerStatistics.Sum(s => s.NumAnswerSERVFAIL);
            NumAnswerNXDOMAIN = dnsServerStatistics.Sum(s => s.NumAnswerNXDOMAIN);
            NumAnswerNOTIMPL = dnsServerStatistics.Sum(s => s.NumAnswerNOTIMPL);
            NumAnswerREFUSED = dnsServerStatistics.Sum(s => s.NumAnswerREFUSED);
            NumAnswerNODATA = dnsServerStatistics.Sum(s => s.NumAnswerNODATA);

            // Extra
            QueriesPerSecondAvg = dnsServerStatistics.Sum(s => s.QueriesPerSecond) / numberOfDataPoints;
            DomainsOnBlocklist = dnsServerStatistics.Last().DomainsOnBlocklist;
            DataPoints = numberOfDataPoints;
        }
    }
}
