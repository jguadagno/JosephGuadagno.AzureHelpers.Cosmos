using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace JosephGuadagno.AzureHelpers.Cosmos
{
    // TODO: Add/Update Xml documentation
    
    public class Tables
    {
        public CloudTableClient CloudTableClient { get; }
        
        /// <summary>
        /// Creates an instance of the <see cref="Tables"/> using the supplied <paramref name="storageConnectionString">Storage Account Url</paramref>
        /// </summary>
        /// <param name="storageConnectionString">A url to the cloud storage account to use.</param>
        public Tables(string storageConnectionString)
        {
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new ArgumentNullException(nameof(storageConnectionString), "The storage connection string cannot be null or empty");
            }
            
            var cloudStorageAccount = CloudStorageAccountHelper.CreateStorageAccountFromConnectionString(storageConnectionString);
            CloudTableClient = cloudStorageAccount.CreateCloudTableClient();
        }

        /// <summary>
        /// Creates an instance of the <see cref="Tables"/> using the supplied <paramref name="cloudStorageAccount">CloudStorageAccount</paramref>
        /// </summary>
        /// <param name="cloudStorageAccount"></param>
        public Tables(CloudStorageAccount cloudStorageAccount)
        {
            if (cloudStorageAccount == null)
            {
                throw new ArgumentNullException(nameof(cloudStorageAccount), "The cloud storage account cannot be null or empty");
            }
            CloudTableClient = cloudStorageAccount.CreateCloudTableClient();
        }
        
        // TODO: Implement in the future
        // ListTablesSegmented
        
        // GetCloudTable
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
            return  cloudTable;
        }

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

        public async Task<CloudTable> CreateCloudTableAsync(CloudTable cloudTable)
        {
            if (cloudTable == null)
            {
                throw new ArgumentNullException(nameof(cloudTable), "The cloud table cannot be null");
            }
            
            await cloudTable.CreateAsync();
            
            return cloudTable;
        }

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

        // ExistsAsync
        public async Task<bool> DoesCloudTableExistsAsync(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName), "The table name cannot be null or empty");
            }

            return await CloudTableClient.GetTableReference(tableName).ExistsAsync();
        }
        
        // Exists
        public bool DoesCloudTableExists(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName), "The table name cannot be null or empty");
            }

            return CloudTableClient.GetTableReference(tableName).Exists();
        }
        
        //ListTables
        public async Task<IEnumerable<CloudTable>> GetListOfTablesAsync(string tableStartsWith = null)
        {
            return await Task.Run (function: () => CloudTableClient.ListTables(tableStartsWith));
        }
    }
}