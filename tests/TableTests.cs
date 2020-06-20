using System;
using System.Threading.Tasks;
using JosephGuadagno.AzureHelpers.Cosmos.Tests.Models;
using Microsoft.Azure.Cosmos.Table;
using Xunit;

namespace JosephGuadagno.AzureHelpers.Cosmos.Tests
{
    public class TableTests
    {

        // Constructor1 
        //     string, string
        [Fact]
        public void Constructor1_WithNullStorageConnectionString_ShouldThrowArgumentNullException()
        {
            // Arrange
            
            // Act
            void Action()
            {
                var unused = new Table(null, null);
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Action);
            Assert.Equal("storageConnectionString", exception.ParamName);
            Assert.StartsWith("The storage connection string cannot be null or empty", exception.Message);
        }
        
        [Fact]
        public void Constructor1_WithNullTableName_ShouldThrowArgumentNullException()
        {
            // Arrange
            
            // Act
            void Action()
            {
                var unused = new Table(TableTestsHelper.DevelopmentConnectionString,null);
            }
            

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Action);
            Assert.Equal("tableName", exception.ParamName);
            Assert.StartsWith("The table name cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Constructor1_WithValidStorageConnectionStringAndTableName_ShouldSetTheCloudTable()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            
            // Act
            var table = new Table(TableTestsHelper.DevelopmentConnectionString, tableName);

            // Assert
            Assert.NotNull(table);
            Assert.NotNull(table.CloudTable);
            Assert.Equal(tableName, table.CloudTable.Name);
        }
        
        // Constructor2
        //    cloudTable
        [Fact]
        public void Constructor2_WithNullCloudTable_ShouldThrowArgumentNullException()
        {
            // Arrange
            
            // Act
            void Action()
            {
                var unused = new Table(null);
            }
            

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Action);
            Assert.Equal("cloudTable", exception.ParamName);
            Assert.StartsWith("The cloud table cannot be null", exception.Message);
        }
        
        [Fact]
        public void Constructor2_WithValidCloudTable_ShouldSetTheCloudTable()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var cloudTable = TableTestsHelper.GetCloudTable(tableName);
            
            // Act
            var table = new Table(cloudTable);

            // Assert
            Assert.NotNull(table);
            Assert.NotNull(table.CloudTable);
            Assert.Equal(tableName, table.CloudTable.Name);
        }
        
        // InsertEntityAsync
        [Fact]
        public async Task InsertEntityAsync_WithNullTableEntity_ShouldThrowArgumentNullException()
        {
            // Arrange
            var table = new Table(TableTestsHelper.DevelopmentConnectionString, "tableName");
            
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => table.InsertEntityAsync(null));
            
            // Assert
            Assert.Equal("tableEntity", exception.ParamName);
            Assert.StartsWith("The table entity cannot be null", exception.Message);
        }

        [Fact]
        public async Task InsertEntityAsync_WithEntity_ShouldInsertEntityAndReturnTrue()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            var table = new Table(temporaryTable);
            var temporaryObject = TableTestsHelper.GetTestObject();
            
            // Act
            var result = await table.InsertEntityAsync(temporaryObject);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.WasSuccessful);
            // Get the item from the table
            var searchedItem =
                TableTestsHelper.GetEntity<TestTableEntity>(tableName, temporaryObject.PartitionKey,
                    temporaryObject.RowKey);
            Assert.NotNull(searchedItem);
            Assert.Equal(temporaryObject.CreatedAt.ToUniversalTime(), searchedItem.CreatedAt.ToUniversalTime());
            Assert.Equal(temporaryObject.Property1, searchedItem.Property1);
                
            // Cleanup
            TableTestsHelper.DeleteTable(temporaryTable);
        }
        
        // InsertOrMergeEntityAsync
        [Fact]
        public async Task InsertOrMergeEntityAsync_WithNullTableEntity_ShouldThrowArgumentNullException()
        {
            // Arrange
            var table = new Table(TableTestsHelper.DevelopmentConnectionString, "tableName");
            
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => table.InsertOrMergeEntityAsync(null));
            
            // Assert
            Assert.Equal("tableEntity", exception.ParamName);
            Assert.StartsWith("The table entity cannot be null", exception.Message);
        }

        [Fact]
        public async Task InsertOrMergeEntityAsync_WithEntityThatDoesNotExists_ShouldInsertEntityAndReturnTrue()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            var table = new Table(temporaryTable);
            var temporaryObject = TableTestsHelper.GetTestObject();
            
            // Act
            var result = await table.InsertOrMergeEntityAsync(temporaryObject);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.WasSuccessful);
            // Get the item from the table
            var searchedItem =
                TableTestsHelper.GetEntity<TestTableEntity>(tableName, temporaryObject.PartitionKey,
                    temporaryObject.RowKey);
            Assert.NotNull(searchedItem);
            Assert.Equal(temporaryObject.CreatedAt.ToUniversalTime(), searchedItem.CreatedAt.ToUniversalTime());
            Assert.Equal(temporaryObject.Property1, searchedItem.Property1);
                
            // Cleanup
            TableTestsHelper.DeleteTable(temporaryTable);
        }

        [Fact]
        public async Task InsertOrMergeEntityAsync_WithEntityThatDoesExist_ShouldMergeTheEntityAndReturnTrue()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            var table = new Table(temporaryTable);
            var temporaryObject = TableTestsHelper.GetTestObject();
            TableTestsHelper.InsertEntity(tableName, temporaryObject);
            var newProperty1Field = TableTestsHelper.GetTemporaryName();
            temporaryObject.Property1 = newProperty1Field;
            
            // Act
            var result = await table.InsertOrMergeEntityAsync(temporaryObject);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.WasSuccessful);
            // Get the item from the table
            var searchedItem =
                TableTestsHelper.GetEntity<TestTableEntity>(tableName, temporaryObject.PartitionKey,
                    temporaryObject.RowKey);
            Assert.NotNull(searchedItem);
            Assert.Equal(temporaryObject.CreatedAt.ToUniversalTime(), searchedItem.CreatedAt.ToUniversalTime());
            Assert.Equal(temporaryObject.Property1, searchedItem.Property1);
                
            // Cleanup
            TableTestsHelper.DeleteTable(temporaryTable);
        }

        // InsertOrReplaceEntityAsync
        [Fact]
        public async Task InsertOrReplaceEntityAsync_WithNullTableEntity_ShouldThrowArgumentNullException()
        {
            // Arrange
            var table = new Table(TableTestsHelper.DevelopmentConnectionString, "tableName");
            
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => table.InsertOrReplaceEntityAsync(null));
            
            // Assert
            Assert.Equal("tableEntity", exception.ParamName);
            Assert.StartsWith("The table entity cannot be null", exception.Message);
        }
                
        [Fact]
        public async Task InsertOrReplaceEntityAsync_WithEntityThatDoesNotExists_ShouldInsertEntityAndReturnTrue()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            var table = new Table(temporaryTable);
            var temporaryObject = TableTestsHelper.GetTestObject();
            
            // Act
            var result = await table.InsertOrReplaceEntityAsync(temporaryObject);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.WasSuccessful);
            // Get the item from the table
            var searchedItem =
                TableTestsHelper.GetEntity<TestTableEntity>(tableName, temporaryObject.PartitionKey,
                    temporaryObject.RowKey);
            Assert.NotNull(searchedItem);
            Assert.Equal(temporaryObject.CreatedAt.ToUniversalTime(), searchedItem.CreatedAt.ToUniversalTime());
            Assert.Equal(temporaryObject.Property1, searchedItem.Property1);
                
            // Cleanup
            TableTestsHelper.DeleteTable(temporaryTable);
        }

        [Fact]
        public async Task InsertOrReplaceEntityAsync_WithEntityThatDoesExist_ShouldReplaceTheEntityAndReturnTrue()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            var table = new Table(temporaryTable);
            var temporaryObject = TableTestsHelper.GetTestObject();
            TableTestsHelper.InsertEntity(tableName, temporaryObject);
            var newProperty1Field = TableTestsHelper.GetTemporaryName();
            temporaryObject.Property1 = newProperty1Field;
            
            // Act
            var result = await table.InsertOrReplaceEntityAsync(temporaryObject);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.WasSuccessful);
            // Get the item from the table
            var searchedItem =
                TableTestsHelper.GetEntity<TestTableEntity>(tableName, temporaryObject.PartitionKey,
                    temporaryObject.RowKey);
            Assert.NotNull(searchedItem);
            Assert.Equal(temporaryObject.CreatedAt.ToUniversalTime(), searchedItem.CreatedAt.ToUniversalTime());
            Assert.Equal(temporaryObject.Property1, searchedItem.Property1);
                
            // Cleanup
            TableTestsHelper.DeleteTable(temporaryTable);
        }
        
        // MergeEntityAsync
        [Fact]
        public async Task MergeEntityAsync_WithNullTableEntity_ShouldThrowArgumentNullException()
        {
            // Arrange
            var table = new Table(TableTestsHelper.DevelopmentConnectionString, "tableName");
            
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => table.MergeEntityAsync(null));
            
            // Assert
            Assert.Equal("tableEntity", exception.ParamName);
            Assert.StartsWith("The table entity cannot be null", exception.Message);
        }
        
        [Fact]
        public async Task MergeEntityAsync_WithEntityThatDoesNotExists_ShouldThrowStorageException()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            var table = new Table(temporaryTable);
            var temporaryObject = TableTestsHelper.GetTestObject();
            
            // Act
            var exception = await Assert.ThrowsAsync<StorageException>(() => table.MergeEntityAsync(temporaryObject));

            // Assert
            Assert.Equal("Not Found", exception.Message);
                
            // Cleanup
            TableTestsHelper.DeleteTable(temporaryTable);
        }

        [Fact]
        public async Task MergeEntityAsync_WithEntityThatDoesExist_ShouldMergeTheEntityAndReturnTrue()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            var table = new Table(temporaryTable);
            var temporaryObject = TableTestsHelper.GetTestObject();
            TableTestsHelper.InsertEntity(tableName, temporaryObject);
            var newProperty1Field = TableTestsHelper.GetTemporaryName();
            temporaryObject.Property1 = newProperty1Field;
            
            // Act
            var result = await table.MergeEntityAsync(temporaryObject);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.WasSuccessful);
            // Get the item from the table
            var searchedItem =
                TableTestsHelper.GetEntity<TestTableEntity>(tableName, temporaryObject.PartitionKey,
                    temporaryObject.RowKey);
            Assert.NotNull(searchedItem);
            Assert.Equal(temporaryObject.CreatedAt.ToUniversalTime(), searchedItem.CreatedAt.ToUniversalTime());
            Assert.Equal(temporaryObject.Property1, searchedItem.Property1);
                
            // Cleanup
            TableTestsHelper.DeleteTable(temporaryTable);
        }
        
        // ReplaceEntityAsync
        [Fact]
        public async Task ReplaceEntityAsync_WithNullTableEntity_ShouldThrowArgumentNullException()
        {
            // Arrange
            var table = new Table(TableTestsHelper.DevelopmentConnectionString, "tableName");
            
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => table.ReplaceEntityAsync(null));
            
            // Assert
            Assert.Equal("tableEntity", exception.ParamName);
            Assert.StartsWith("The table entity cannot be null", exception.Message);
        }
        
        [Fact]
        public async Task ReplaceEntityAsync_WithEntityThatDoesNotExists_ShouldThrowStorageException()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            var table = new Table(temporaryTable);
            var temporaryObject = TableTestsHelper.GetTestObject();
            
            // Act
            var exception = await Assert.ThrowsAsync<StorageException>(() => table.ReplaceEntityAsync(temporaryObject));

            // Assert
            Assert.Equal("Not Found", exception.Message);
                
            // Cleanup
            TableTestsHelper.DeleteTable(temporaryTable);
        }

        [Fact]
        public async Task ReplaceEntityAsync_WithEntityThatDoesExist_ShouldReplaceTheEntityAndReturnTrue()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            var table = new Table(temporaryTable);
            var temporaryObject = TableTestsHelper.GetTestObject();
            TableTestsHelper.InsertEntity(tableName, temporaryObject);
            var newProperty1Field = TableTestsHelper.GetTemporaryName();
            temporaryObject.Property1 = newProperty1Field;
            
            // Act
            var result = await table.ReplaceEntityAsync(temporaryObject);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.WasSuccessful);
            // Get the item from the table
            var searchedItem =
                TableTestsHelper.GetEntity<TestTableEntity>(tableName, temporaryObject.PartitionKey,
                    temporaryObject.RowKey);
            Assert.NotNull(searchedItem);
            Assert.Equal(temporaryObject.CreatedAt.ToUniversalTime(), searchedItem.CreatedAt.ToUniversalTime());
            Assert.Equal(temporaryObject.Property1, searchedItem.Property1);
                
            // Cleanup
            TableTestsHelper.DeleteTable(temporaryTable);
        }
        
        // DeleteEntityAsync
        [Fact]
        public async Task DeleteEntityAsync_WithNullTableEntity_ShouldThrowArgumentNullException()
        {
            // Arrange
            var table = new Table(TableTestsHelper.DevelopmentConnectionString, "tableName");
            
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => table.DeleteEntityAsync(null));
            
            // Assert
            Assert.Equal("tableEntity", exception.ParamName);
            Assert.StartsWith("The table entity cannot be null", exception.Message);
        }
        
        [Fact]
        public async Task DeleteEntityAsync_WithEntityThatDoesNotExists_ShouldThrowStorageException()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            var table = new Table(temporaryTable);
            var temporaryObject = TableTestsHelper.GetTestObject();
            
            // Act
            var exception = await Assert.ThrowsAsync<StorageException>(() => table.DeleteEntityAsync(temporaryObject));

            // Assert
            Assert.Equal("Not Found", exception.Message);
            
            // Cleanup
            TableTestsHelper.DeleteTable(temporaryTable);
        }

        [Fact]
        public async Task DeleteEntityAsync_WithEntityThatDoesExist_ShouldDeleteTheEntityAndReturnTrue()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            var table = new Table(temporaryTable);
            var temporaryObject = TableTestsHelper.GetTestObject();
            TableTestsHelper.InsertEntity(tableName, temporaryObject);
            
            // Act
            var result = await table.DeleteEntityAsync(temporaryObject);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.WasSuccessful);
            // Get the item from the table
            var searchedItem =
                TableTestsHelper.GetEntity<TestTableEntity>(tableName, temporaryObject.PartitionKey,
                    temporaryObject.RowKey);
            Assert.Null(searchedItem);
                
            // Cleanup
            TableTestsHelper.DeleteTable(temporaryTable);
        }
        
        // GetTableEntityAsync
        [Fact]
        public async Task GetTableEntityAsync_WithPartitionKeyNull_ShouldReturnNull()
        {
            // Arrange
            var table = new Table(TableTestsHelper.DevelopmentConnectionString, "tableName");   
            
            // Act
            var result = await table.GetTableEntityAsync<TestTableEntity>(null, "rowKey");
            
            // Assert
            Assert.Null(result);
        }
        [Fact]
        public async Task GetTableEntityAsync_WithRowKeyNull_ShouldReturnNull()
        {
            // Arrange
            var table = new Table(TableTestsHelper.DevelopmentConnectionString, "tableName");   
            
            // Act
            var result = await table.GetTableEntityAsync<TestTableEntity>("partitionKey", null);
            
            // Assert
            Assert.Null(result);
        }
        [Fact]
        public async Task GetTableEntityAsync_WithPartitionKeyAndRowKeyNull_ShouldReturnNull()
        {
            // Arrange
            var table = new Table(TableTestsHelper.DevelopmentConnectionString, "tableName");   
            
            // Act
            var result = await table.GetTableEntityAsync<TestTableEntity>(null, null);
            
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetTableEntityAsync_WithCloudTableThatDoesNotExists_ShouldReturnNull()
        {
            // Arrange
            var table = new Table(TableTestsHelper.DevelopmentConnectionString, "tableName");   
            
            // Act
            var result = await table.GetTableEntityAsync<TestTableEntity>("partitionKey", "rowKey");
            
            // Assert
            Assert.Null(result);
        }
        // If partitionKey and rowKey exists, return it
        // If partitionKey and or rowKey don't , return null
        // if PartitionKey and rowKey exists but cannot be cast into T, return null
        
    }
}