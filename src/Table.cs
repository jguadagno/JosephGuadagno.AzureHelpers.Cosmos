using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace JosephGuadagno.AzureHelpers.Cosmos
{
    // TODO: Implement IDE Suggestions
    // TODO: Add/Update Xml documentation
    
    public class Table
    {
        public CloudTable CloudTable { get; }

        public Table(string storageConnectionString, string tableName)
        {
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new ArgumentNullException(nameof(storageConnectionString), "The storage connection string cannot be null or empty");
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName), "The table name cannot be null or empty");
            }

            var cloudStorageAccount =
                CloudStorageAccountHelper.CreateStorageAccountFromConnectionString(storageConnectionString);
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable = cloudTableClient.GetTableReference(tableName);
        }

        public Table(CloudTable cloudTable)
        {
            if (cloudTable == null)
            {
                throw new ArgumentNullException(nameof(cloudTable), "The cloud table cannot be null");
            }

            CloudTable = cloudTable;
        }
        
        // TODO: Implement in the future
        // ExecuteBatchAsync
        // ExecuteQueryAsync

        public async Task<TableOperationResult> InsertEntityAsync(TableEntity tableEntity)
        {
            if (tableEntity == null)
            {
                throw new ArgumentNullException(nameof(tableEntity), "The table entity cannot be null");
            }

            var insertTableOperation = TableOperation.Insert(tableEntity);
            var tableResult = await CloudTable.ExecuteAsync(insertTableOperation);

            var result = new TableOperationResult(tableResult)
            {
                WasSuccessful = tableResult.HttpStatusCode == (int) HttpStatusCode.NoContent
            };
            return result;
        }

        public async Task<TableOperationResult> InsertOrMergeEntityAsync(TableEntity tableEntity)
        {
            if (tableEntity == null)
            {
                throw new ArgumentNullException(nameof(tableEntity), "The table entity cannot be null");
            }

            var insertOrMergeOperation = TableOperation.InsertOrMerge(tableEntity);
            var tableResult = await CloudTable.ExecuteAsync(insertOrMergeOperation);

            var result = new TableOperationResult(tableResult)
            {
                WasSuccessful = tableResult.HttpStatusCode == (int) HttpStatusCode.NoContent
            };
            return result;
        }

        public async Task<TableOperationResult> InsertOrReplaceEntityAsync(TableEntity tableEntity)
        {
            if (tableEntity == null)
            {
                throw new ArgumentNullException(nameof(tableEntity), "The table entity cannot be null");
            }

            var insertOrReplaceOperation = TableOperation.InsertOrReplace(tableEntity);
            var tableResult = await CloudTable.ExecuteAsync(insertOrReplaceOperation);

            var result = new TableOperationResult(tableResult)
            {
                WasSuccessful = tableResult.HttpStatusCode == (int) HttpStatusCode.NoContent
            };
            return result;
        }

        public async Task<TableOperationResult> MergeEntityAsync(TableEntity tableEntity)
        {
            if (tableEntity == null)
            {
                throw new ArgumentNullException(nameof(tableEntity), "The table entity cannot be null");
            }

            if (tableEntity.ETag == null)
            {
                tableEntity.ETag = "*";
            }
            
            var mergeOperation = TableOperation.Merge(tableEntity);
            var tableResult = await CloudTable.ExecuteAsync(mergeOperation);

            var result = new TableOperationResult(tableResult)
            {
                WasSuccessful = tableResult.HttpStatusCode == (int) HttpStatusCode.NoContent
            };
            return result;
        }

        public async Task<TableOperationResult> ReplaceEntityAsync(TableEntity tableEntity)
        {
            if (tableEntity == null)
            {
                throw new ArgumentNullException(nameof(tableEntity), "The table entity cannot be null");
            }
            
            if (tableEntity.ETag == null)
            {
                tableEntity.ETag = "*";
            }

            var replaceTableOperation = TableOperation.Replace(tableEntity);
            var tableResult = await CloudTable.ExecuteAsync(replaceTableOperation);

            var result = new TableOperationResult(tableResult)
            {
                WasSuccessful = tableResult.HttpStatusCode == (int) HttpStatusCode.NoContent
            };
            return result;
        }

        public async Task<TableOperationResult> DeleteEntityAsync(TableEntity tableEntity)
        {
            if (tableEntity == null)
            {
                throw new ArgumentNullException(nameof(tableEntity), "The table entity cannot be null");
            }
            
            if (tableEntity.ETag == null)
            {
                tableEntity.ETag = "*";
            }

            var deleteTableOperation = TableOperation.Delete(tableEntity);
            var tableResult = await CloudTable.ExecuteAsync(deleteTableOperation);

            var result = new TableOperationResult(tableResult)
            {
                WasSuccessful = tableResult.HttpStatusCode == (int) HttpStatusCode.NoContent
            };
            return result;
        }
        
        public async Task<T> GetTableEntityAsync<T>(string partitionKey, string rowKey)
            where T : class, ITableEntity
        {
            if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey))
                return null;

            var doesExists = await CloudTable.ExistsAsync();
            if (!doesExists) return null;

            var retrieveTableOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = await CloudTable.ExecuteAsync(retrieveTableOperation);
            return result?.Result as T;
        }
    }
}