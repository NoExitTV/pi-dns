using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace FO.Payment.Services.Common.Stores.Storage
{
    /// <summary>
    /// Allows to make access to any table storage by specifing existing CloudTable
    /// </summary>
    public interface ITableStorage
    {
        /// <summary>
        /// Gets table storage rows by filter
        /// </summary>
        Task<TableQuerySegment<T>> GetTableData<T>(CloudTable table, string filter, TableContinuationToken continuationToken = null) where T : ITableEntity, new();

        /// <summary>
        /// Gets single table storage row by filter
        /// </summary>
        //Task<T> GetTableRecord<T>(CloudTable table, string filter) where T : ITableEntity, new();

        /// <summary>
        /// Inserts new record into Table Storage
        /// </summary>
        Task InsertTableRecordAsync<T>(CloudTable table, T data) where T : ITableEntity, new();

        /// <summary>
        /// Removes record from Table Storage
        /// </summary>
        //Task RemoveTableRecord<T>(CloudTable table, T data) where T : ITableEntity, new();

        /// <summary>
        /// Gets table record from Table Storage by partition key and row key
        /// </summary>
        //Task<T> RetrieveTableRecord<T>(CloudTable table, string partitionKey, string rowKey) where T : ITableEntity, new();

        /// <summary>
        /// Updates table record with new data
        /// </summary>
        //Task UpdateTableRecord<T>(CloudTable table, T data) where T : ITableEntity, new();

        /// <summary>
        /// Insert or merge table record to Table Storage
        /// </summary>
        //Task InsertOrMergeTableRecordAsync<T>(CloudTable table, T data) where T : ITableEntity, new();
    }
}