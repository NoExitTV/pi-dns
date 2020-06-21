using Microsoft.Azure.Cosmos.Table;
using Pi.Dns.Common.Exceptions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FO.Payment.Services.Common.Stores.Storage
{
    public class TableStorage : ITableStorage
    {
        public async Task<TableQuerySegment<T>> GetTableData<T>(CloudTable table, string filter, TableContinuationToken continuationToken = null) where T : ITableEntity, new()
        {
            var query = new TableQuery<T>().Where(filter);
            try
            {
                try
                {
                    return await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                }
                catch (StorageException se) when (se?.RequestInformation?.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    if (await table.CreateIfNotExistsAsync())
                    {
                        return await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (StorageException se)
            {
                throw ToDatastoreException(se);
            }
        }


        public async Task InsertTableRecordAsync<T>(CloudTable table, T data) where T : ITableEntity, new()
        {
            var tableOperation = TableOperation.Insert(data);
            await ExecuteAsync(table, tableOperation);
        }

        //public async Task<T> GetTableRecord<T>(CloudTable table, string filter) where T : ITableEntity, new()
        //{
        //    var tableData = await GetTableData<T>(table, filter);
        //    return tableData.Results.FirstOrDefault();
        //}

        //public async Task RemoveTableRecord<T>(CloudTable table, T data) where T : ITableEntity, new()
        //{
        //    var tableOperation = TableOperation.Delete(data);
        //    await ExecuteAsync(table, tableOperation);
        //}

        //public async Task UpdateTableRecord<T>(CloudTable table, T data) where T : ITableEntity, new()
        //{
        //    var tableOperation = TableOperation.Replace(data);
        //    await ExecuteAsync(table, tableOperation);
        //}

        //public async Task<T> RetrieveTableRecord<T>(CloudTable table, string partitionKey, string rowKey) where T : ITableEntity, new()
        //{
        //    var retrieve = TableOperation.Retrieve<T>(partitionKey, rowKey);
        //    var tableResult = await ExecuteAsync(table, retrieve);

        //    return (T)tableResult.Result;
        //}

        //public async Task InsertOrMergeTableRecordAsync<T>(CloudTable table, T data) where T : ITableEntity, new()
        //{
        //    var tableOperation = TableOperation.InsertOrMerge(data);
        //    await ExecuteAsync(table, tableOperation);
        //}

        private async Task<TableResult> ExecuteAsync(CloudTable table, TableOperation operation)
        {
            TableResult executionResult;
            try
            {
                try
                {
                    executionResult = await table.ExecuteAsync(operation);
                }
                catch (StorageException se) when (se?.RequestInformation?.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    if (await table.CreateIfNotExistsAsync())
                    {
                        executionResult = await table.ExecuteAsync(operation);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (StorageException se)
            {
                throw ToDatastoreException(se);
            }

            return executionResult;
        }

        public DatastoreException ToDatastoreException(StorageException se)
        {
            var message = se?.RequestInformation?.ExtendedErrorInformation?.ErrorMessage ?? "Unspecified datastore error";
            var retryAt = TimeSpan.Zero;
            if (Enum.TryParse(se?.RequestInformation?.HttpStatusCode.ToString() ?? null, out HttpStatusCode httpStatusCode))
            {
                switch (httpStatusCode)
                {
                    case HttpStatusCode.NotFound:
                        message = "The specified entity does not exist";
                        break;
                    case HttpStatusCode.Conflict:
                        message = "The specified entity already exists";
                        break;
                    case HttpStatusCode.PreconditionFailed:
                        message = "The specified entity has been changed on the server since it was retrieved";
                        break;
                    case HttpStatusCode.ServiceUnavailable:
                        httpStatusCode = HttpStatusCode.ServiceUnavailable;
                        retryAt = TimeSpan.FromMilliseconds(500);
                        break;
                }
            }
            else
            {
                httpStatusCode = HttpStatusCode.InternalServerError;
            }

            return new DatastoreException(httpStatusCode, message, retryAt, se);
        }
    }
}