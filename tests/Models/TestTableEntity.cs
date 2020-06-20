using System;
using Microsoft.Azure.Cosmos.Table;

namespace JosephGuadagno.AzureHelpers.Cosmos.Tests.Models
{
    public class TestTableEntity: TableEntity
    {
        public TestTableEntity()
        {

        }

        public TestTableEntity(string partitionKey, string rowKey): this()
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public DateTime CreatedAt { get; set; }
        public string Property1 { get; set; }
    }
}
