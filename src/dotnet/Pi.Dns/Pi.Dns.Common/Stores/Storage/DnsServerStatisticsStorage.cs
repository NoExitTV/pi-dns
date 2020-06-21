using FO.Payment.Services.Common.Stores.Storage;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;
using Pi.Dns.Common.Models;
using Pi.Dns.Common.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pi.Dns.Common.Stores.Storage
{
    public class DnsServerStatisticsStorage : TableStorage, IDnsServerStatisticsStore
    {
        public readonly CloudTable _cloudTable;

        public DnsServerStatisticsStorage(IOptions<DnsServerStatisticsStoreSettings> settings)
        {
            _cloudTable = CloudStorageAccount.Parse(settings.Value.ConnectionString).CreateCloudTableClient().GetTableReference(settings.Value.TableName);
        }

        /// <summary>
        /// Add a DNS statistics entity to table storage
        /// </summary>
        /// <param name="dnsServerStatistics"></param>
        /// <returns></returns>
        public async Task Add(DnsServerStatistics dnsServerStatistics)
        {
            await InsertTableRecordAsync(_cloudTable, new DnsServerStatisticsEntity(dnsServerStatistics));
        }

        /// <summary>
        /// Get all DNS server statistics for a given server newer than a specific date
        /// </summary>
        /// <param name="server"></param>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        public async Task<IOrderedEnumerable<DnsServerStatistics>> GetServerStatisticsFromDate(string server, DateTime fromDate)
        {
            if (fromDate > DateTime.UtcNow)
                throw new ArgumentOutOfRangeException("From DateTime can not be greater than current DateTime. Can not query into the future");

            string partitionKeyFilter;

            // Partition key is server:year-month
            // If year or month is different from now, then we have to search in a range of partitions, for an example in partitions in range server:2020-01 - server:2020-03
            // This have been manually tested and seems to work :)
            if (fromDate.Year != DateTime.UtcNow.Year || fromDate.Month != DateTime.UtcNow.Month)
            {
                partitionKeyFilter = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, $"{server}:{fromDate:yyyy-MM}"),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.LessThanOrEqual, $"{server}:{DateTime.UtcNow:yyyy-MM}"));
            }
            else
            {
                partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, $"{server}:{fromDate:yyyy-MM}");
            }

            var createdDateFilter = TableQuery.GenerateFilterConditionForDate("CreatedDate", QueryComparisons.GreaterThanOrEqual, fromDate);
            string combinedFilter = TableQuery.CombineFilters(partitionKeyFilter, TableOperators.And, createdDateFilter);

            var entities = new List<DnsServerStatistics>();
            TableContinuationToken continuationToken = null;

            do
            {
                var page = await GetTableData<DnsServerStatisticsEntity>(_cloudTable, combinedFilter, continuationToken);
                continuationToken = page.ContinuationToken;
                entities.AddRange(page.Results.Select(entity => entity.DnsServerStatistics));
            }
            while (continuationToken != null);

            return entities.OrderBy(entity => entity.CreatedDate);
        }
    }
}
