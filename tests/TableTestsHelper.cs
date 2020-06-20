using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using JosephGuadagno.AzureHelpers.Cosmos.Tests.Models;
using Microsoft.Azure.Cosmos.Table;

namespace JosephGuadagno.AzureHelpers.Cosmos.Tests
{
    public static class TableTestsHelper
    {
        public const string DevelopmentConnectionString = "UseDevelopmentStorage=true";
        
        public static string GetTemporaryName()
        {
            var dateString = DateTime.Now.ToString("s")
                .Replace("/", "")
                .Replace(":", "")
                .Replace("T", "")
                .Replace("-", "");
            var randomNumber = new Random().Next(1, 1000).ToString()
                .PadLeft(4, '0');
            return $"test{dateString}{randomNumber}";
        }

        public static CloudTable CreateTable(string tableName)
        {
            var cloudTable = GetCloudTable(tableName);
            cloudTable.CreateIfNotExists();
            return cloudTable;
        }

        public static bool DeleteTable(string tableName)
        {
            return DeleteTable(GetCloudTable(tableName));
        }

        public static bool DeleteTable(CloudTable cloudTable)
        {
            return cloudTable.DeleteIfExists();
        }

        public static bool DoesTableExists(string tableName)
        {
            return GetCloudTable(tableName).Exists();
        }

        public static CloudTable GetCloudTable(string tableName)
        {
            var cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            return cloudTableClient.GetTableReference(tableName);
        }
        
        // Table Helpers
        public static bool InsertEntity(string tableName, TableEntity entity)
        {
            var cloudTable = GetCloudTable(tableName);
            var tableAction = TableOperation.Insert(entity);
            var results = cloudTable.Execute(tableAction);
            return results.HttpStatusCode == (int) HttpStatusCode.Created;
        }
        
        public static bool DeleteEntity(string tableName, TableEntity entity)
        {
            var cloudTable = GetCloudTable(tableName);
            var tableAction = TableOperation.Delete(entity);
            var results = cloudTable.Execute(tableAction);
            return results.HttpStatusCode == (int) HttpStatusCode.NoContent;
        }

        public static T GetEntity<T>(string tableName, string partitionKey, string rowKey)
            where T : class, ITableEntity
        {
            var cloudTable = GetCloudTable(tableName);
            var tableOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = cloudTable.Execute(tableOperation);
            var resultingItem = result.Result as T;
            return resultingItem;
        }

        public static TestTableEntity GetTestObject()
        {
            return new TestTableEntity
            {
                PartitionKey = GetTemporaryName(),
                RowKey = GetTemporaryName(),
                CreatedAt = DateTime.Now, //.ToString("s"),
                Property1 = "JosephGuadagno"
            };
        }
    }
}