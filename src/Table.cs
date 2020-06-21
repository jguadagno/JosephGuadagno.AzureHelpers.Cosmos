using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace JosephGuadagno.AzureHelpers.Cosmos
{
    /// <summary>
    /// Provides methods to work with objects in a table in Azure Storage or Azure CosmosDb.
    /// </summary>
    /// <remarks>To add, remove, lists, etc. with Tables, please refer to <see cref="Tables"/></remarks>
    public class Table
    {
        /// <summary>
        /// A reference to the table being used
        /// </summary>
        public CloudTable CloudTable { get; }

        /// <summary>
        /// Creates an instance of the table
        /// </summary>
        /// <param name="storageConnectionString">The connection string to the storage</param>
        /// <param name="tableName">The name of the table</param>
        /// <exception cref="ArgumentNullException">Throws if either the <see cref="storageConnectionString"/> or <see cref="tableName"/> are null or empty</exception>
        public Table(string storageConnectionString, string tableName)
        {
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new ArgumentNullException(nameof(storageConnectionString),
                    "The storage connection string cannot be null or empty");
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

        /// <summary>
        /// Creates an instance of the table
        /// </summary>
        /// <param name="cloudTable">The cloud table</param>
        /// <exception cref="ArgumentNullException">Throws if <see cref="cloudTable"/>is null</exception>
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

        /// <summary>
        /// Inserts the entity
        /// </summary>
        /// <param name="tableEntity">An object that inherits from TableEntity</param>
        /// <returns>A <see cref="TableOperationResult"/> with the results of the operation</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="tableEntity"/> is null</exception>
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

        /// <summary>
        /// Insert the entity if it doesn't exists, otherwise merges this entity with the existing one
        /// </summary>
        /// <param name="tableEntity">An object that inherits from TableEntity</param>
        /// <returns>A <see cref="TableOperationResult"/> with the results of the operation</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="tableEntity"/> is null</exception>
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

        /// <summary>
        /// Insert the entity if it doesn't exists, otherwise replace the existing one with this one
        /// </summary>
        /// <param name="tableEntity">An object that inherits from TableEntity</param>
        /// <returns>A <see cref="TableOperationResult"/> with the results of the operation</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="tableEntity"/> is null</exception>
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

        /// <summary>
        /// Merge the entity with an existing one
        /// </summary>
        /// <param name="tableEntity">An object that inherits from TableEntity</param>
        /// <returns>A <see cref="TableOperationResult"/> with the results of the operation</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="tableEntity"/> is null</exception>
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

        /// <summary>
        /// Replaces an existing entity with the existing one
        /// </summary>
        /// <param name="tableEntity">An object that inherits from TableEntity</param>
        /// <returns>A <see cref="TableOperationResult"/> with the results of the operation</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="tableEntity"/> is null</exception>
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

        /// <summary>
        /// Deletes an existing entity
        /// </summary>
        /// <param name="tableEntity">An object that inherits from TableEntity</param>
        /// <returns>A <see cref="TableOperationResult"/> with the results of the operation</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="tableEntity"/> is null</exception>
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

        /// <summary>
        /// Gets a entity from the table
        /// </summary>
        /// <param name="partitionKey">The partition key</param>
        /// <param name="rowKey">The row key</param>
        /// <typeparam name="T">The object to deserialize the results into</typeparam>
        /// <returns>The object if found, otherwise, null</returns>
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