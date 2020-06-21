using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace JosephGuadagno.AzureHelpers.Cosmos
{
    /// <summary>
    /// Provides methods for interacting with Tables in Azure Storage Tables or Azure CosmosDB Tables
    /// </summary>
    public class Tables
    {
        /// <summary>
        /// The reference to the current CloudTableClient being used
        /// </summary>
        public CloudTableClient CloudTableClient { get; }

        /// <summary>
        /// Creates an instance of the Tables
        /// </summary>
        /// <param name="storageConnectionString">The storage connection string to use</param>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="storageConnectionString"/> is null or empty</exception>
        public Tables(string storageConnectionString)
        {
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new ArgumentNullException(nameof(storageConnectionString),
                    "The storage connection string cannot be null or empty");
            }

            var cloudStorageAccount =
                CloudStorageAccountHelper.CreateStorageAccountFromConnectionString(storageConnectionString);
            CloudTableClient = cloudStorageAccount.CreateCloudTableClient();
        }

        /// <summary>
        /// Creates an instance of the Tables
        /// </summary>
        /// <param name="cloudStorageAccount">The CloudStorageAccount to use</param>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="cloudStorageAccount"/> is null</exception>
        /// <remarks>You can get a reference to a <see cref="CloudStorageAccount"/> by using <see cref="CloudStorageAccountHelper.CreateStorageAccountFromConnectionString"/></remarks>
        public Tables(CloudStorageAccount cloudStorageAccount)
        {
            if (cloudStorageAccount == null)
            {
                throw new ArgumentNullException(nameof(cloudStorageAccount),
                    "The cloud storage account cannot be null or empty");
            }

            CloudTableClient = cloudStorageAccount.CreateCloudTableClient();
        }

        // TODO: Implement in the future
        // ListTablesSegmented

        /// <summary>
        /// Gets a reference to the CloudTable
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="createIfNeeded">Indicates if you want to create the table if it does not already exists. The default is false.</param>
        /// <returns>A reference to the CloudTable</returns>
        /// <exception cref="ArgumentNullException">Throws if the the <see cref="tableName"/> is null or empty</exception>
        /// <remarks>This can be used to create an instance of the <see cref="Table"/> class</remarks>
        public async Task<CloudTable> GetCloudTableAsync(string tableName, bool createIfNeeded = false)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName), "The table name cannot be null or empty");
            }

            var cloudTable = CloudTableClient.GetTableReference(tableName);

            if (await cloudTable.ExistsAsync() == false)
            {
                if (createIfNeeded)
                {
                    var created = await cloudTable.CreateIfNotExistsAsync();
                    if (!created)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }

            return cloudTable;
        }

        /// <summary>
        /// Creates a new table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <returns>A reference to the CloudTable</returns>
        /// <exception cref="ArgumentNullException">Throws if the the <see cref="tableName"/> is null or empty</exception>
        /// <remarks>This can be used to create an instance of the <see cref="Table"/> class</remarks>
        // CreateAsync (CreateIfNotExists)
        public async Task<CloudTable> CreateCloudTableAsync(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName), "The table name cannot be null or empty");
            }

            var cloudTable = CloudTableClient.GetTableReference(tableName);
            return await CreateCloudTableAsync(cloudTable);
        }

        /// <summary>
        /// Creates a new table
        /// </summary>
        /// <param name="cloudTable">The table</param>
        /// <returns>A reference to the CloudTable</returns>
        /// <exception cref="ArgumentNullException">Throws if the the <see cref="cloudTable"/> is null</exception>
        /// <remarks>This can be used to create an instance of the <see cref="Table"/> class</remarks>
        public async Task<CloudTable> CreateCloudTableAsync(CloudTable cloudTable)
        {
            if (cloudTable == null)
            {
                throw new ArgumentNullException(nameof(cloudTable), "The cloud table cannot be null");
            }

            await cloudTable.CreateAsync();

            return cloudTable;
        }

        /// <summary>
        /// Deletes a table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <returns>True, if successful, otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Throws if the the <see cref="tableName"/> is null or empty</exception>
        // Delete
        public async Task<bool> DeleteCloudTableAsync(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName), "The table name cannot be null or empty");
            }

            var cloudTable = await GetCloudTableAsync(tableName);
            if (cloudTable == null)
            {
                return false;
            }

            return await DeleteCloudTableAsync(cloudTable);
        }

        /// <summary>
        /// Deletes a table
        /// </summary>
        /// <param name="cloudTable">The table</param>
        /// <returns>True, if successful, otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Throws if the the <see cref="cloudTable"/> is null</exception>
        public async Task<bool> DeleteCloudTableAsync(CloudTable cloudTable)
        {
            if (cloudTable == null)
            {
                throw new ArgumentNullException(nameof(cloudTable), "The cloud table cannot be null");
            }

            if (await cloudTable.ExistsAsync())
            {
                await cloudTable.DeleteIfExistsAsync();
            }
            else
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Checks if the table exists
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <returns>True, if the table exists, otherwise, false</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="tableName"/> is null or empty</exception>
        // ExistsAsync
        public async Task<bool> DoesCloudTableExistsAsync(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName), "The table name cannot be null or empty");
            }

            return await CloudTableClient.GetTableReference(tableName).ExistsAsync();
        }

        /// <summary>
        /// Checks if the table exists
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <returns>True, if the table exists, otherwise, false</returns>
        /// <exception cref="ArgumentNullException">Throws if the <see cref="tableName"/> is null or empty</exception>
        // Exists
        public bool DoesCloudTableExists(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName), "The table name cannot be null or empty");
            }

            return CloudTableClient.GetTableReference(tableName).Exists();
        }

        /// <summary>
        /// Gets a list of all of the tables in current Azure Account
        /// </summary>
        /// <param name="tableStartsWith">Only get tables that start with this string. Default is null.</param>
        /// <returns>A List of CloudTable's</returns>
        /// <remarks>An item in this list can be used to create an instance of <see cref="Table"/></remarks>
        //ListTables
        public async Task<IEnumerable<CloudTable>> GetListOfTablesAsync(string tableStartsWith = null)
        {
            return await Task.Run(function: () => CloudTableClient.ListTables(tableStartsWith));
        }
    }
}