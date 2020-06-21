using Pi.Dns.Common.Extensions;
using System.Collections.Generic;

namespace Pi.Dns.Common.Models
{
    public class UnboundControlStats
    {
        // Total
        public int TotalNumQueries { get; set; }
        public int TotalNumCacheHits { get; set; }
        public int TotalNumCacheMiss { get; set; }
        public double TotalRecursionTimeAvg { get; set; }
        public double TotalRecursionTimeMedian { get; set; }

        // Time
        public double TimeUp { get; set; }
        public double TimeElapsed { get; set; }

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
        public double QueriesPerSecond { get; set; }
        public int DomainsOnBlocklist { get; set; }

        /// <summary>
        /// Empty constructor to make serializable
        /// </summary>
        public UnboundControlStats()
        {

        }

        /// <summary>
        /// Populate object from a enumerable of lines
        /// where lines is the direct output from unbound-control stats command
        /// </summary>
        /// <param name="lines"></param>
        public UnboundControlStats(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                var splitLine = line?.Trim()?.Split('=');

                if (splitLine?.Length != 2)
                    continue;

                var key = splitLine[0];
                var value = splitLine[1];

                switch (key)
                {
                    // Total
                    case "total.num.queries":
                        TotalNumQueries = value.AsInt();
                        break;
                    case "total.num.cachehits":
                        TotalNumCacheHits = value.AsInt();
                        break;
                    case "total.num.cachemiss":
                        TotalNumCacheMiss = value.AsInt();
                        break;
                    case "total.recursion.time.avg":
                        TotalRecursionTimeAvg = value.AsDouble();
                        break;
                    case "total.recursion.time.median":
                        TotalRecursionTimeMedian = value.AsDouble();
                        break;

                    // Time
                    case "time.up":
                        TimeUp = value.AsDouble();
                        break;
                    case "time.elapsed":
                        TimeElapsed = value.AsDouble();
                        break;

                    // Num query
                    case "num.query.type.A":
                        NumQueryTypeA = value.AsInt();
                        break;
                    case "num.query.type.SOA":
                        NumQueryTypeSOA = value.AsInt();
                        break;
                    case "num.query.type.NULL":
                        NumQueryTypeNull = value.AsInt();
                        break;
                    case "num.query.type.TXT":
                        NumQueryTypeTXT = value.AsInt();
                        break;
                    case "num.query.type.AAAA":
                        NumQueryTypeAAA = value.AsInt();
                        break;
                    case "num.query.type.SRV":
                        NumQueryTypeSRV = value.AsInt();
                        break;
                    case "num.query.type.DNSKEY":
                        NumQueryTypeDNSKEY = value.AsInt();
                        break;
                    case "num.query.type.ANY":
                        NumQueryTypeAny = value.AsInt();
                        break;

                    // Num answer
                    case "num.answer.rcode.NOERROR":
                        NumAnswerNOERROR = value.AsInt();
                        break;
                    case "num.answer.rcode.FORMERR":
                        NumAnswerFORMERR = value.AsInt();
                        break;
                    case "num.answer.rcode.SERVFAIL":
                        NumAnswerSERVFAIL = value.AsInt();
                        break;
                    case "num.answer.rcode.NXDOMAIN":
                        NumAnswerNXDOMAIN = value.AsInt();
                        break;
                    case "num.answer.rcode.NOTIMPL":
                        NumAnswerNOTIMPL = value.AsInt();
                        break;
                    case "num.answer.rcode.REFUSED":
                        NumAnswerREFUSED = value.AsInt();
                        break;
                    case "num.answer.rcode.nodata":
                        NumAnswerNODATA = value.AsInt();
                        break;

                    // Extra
                    case "domains.on.blocklist":
                        DomainsOnBlocklist = value.AsInt(); ;
                        break;
                }
            }

            QueriesPerSecond = TotalNumQueries / TimeElapsed;
        }
    }
}
