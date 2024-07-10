using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Xunit;

namespace JosephGuadagno.AzureHelpers.Cosmos.Tests
{
    public class TablesTests
    {

        // Constructor1
        //     string
        [Fact]
        public void Constructor1_WithNullStorageConnectionString_ShouldThrowArgumentNullException()
        {
            // Arrange

            // Act
            void Action()
            {
                var unused = new Tables((string) null);
            }

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Action);
            Assert.Equal("storageConnectionString", exception.ParamName);
            Assert.StartsWith("The storage connection string cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Constructor1_WithValidStorageConnectionString_ShouldSetTableClient()
        {
            // Arrange

            // Act
            var tables = new Tables(CloudStorageAccountHelper.CreateStorageAccountFromConnectionString(TableTestsHelper.DevelopmentConnectionString));
            
            // Assert
            Assert.NotNull(tables);
            Assert.NotNull(tables.CloudTableClient);
        }
        
        // Constructor2
        //    CloudStorageAccount
        [Fact]
        public void Constructor2_WithNullCloudStorageAccount_ShouldThrowArgumentNullException()
        {
            // Arrange
            
            // Act
            void Action()
            {
                var unused = new Tables((CloudStorageAccount) null);
            }
            
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Action);
            Assert.Equal("cloudStorageAccount", exception.ParamName);
            Assert.StartsWith("The cloud storage account cannot be null", exception.Message);
        }
        
        [Fact]
        public void Constructor1_WithValidCloudStorageAccount_ShouldSetTableClient()
        {
            // Arrange
            
            // Act
            var tables = new Tables(CloudStorageAccount.DevelopmentStorageAccount);
            
            // Assert
            Assert.NotNull(tables);
            Assert.NotNull(tables.CloudTableClient);
        }
        
        // GetCloudTableAsync
        // Missing scenario GetCloudTable: Table does not exist, Create=True,the call errors, It should return null.
        // This scenario is going to be hard to prove
        [Fact]
        public async Task GetCloudTableAsync_WithNullTableName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>tables.GetCloudTableAsync(null));

            // Assert
            Assert.Equal("tableName", exception.ParamName);
            Assert.StartsWith("The table name cannot be null or empty", exception.Message);
        }
        
        [Fact]
        public async Task GetCloudTableAsync_WithValidTableNameAndTableDoesntExists_ShouldReturnNull()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            
            // Act
            var table = await tables.GetCloudTableAsync(tableName);

            // Assert
            Assert.Null(table);
            Assert.False(TableTestsHelper.DoesTableExists(tableName));
        }
        
        [Fact]
        public async Task GetCloudTableAsync_WithValidTableNameAndTableDoesntExistsAndCreateEqualsTrue_ShouldCreateTableAndReturnIt()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            
            // Act
            var table = await tables.GetCloudTableAsync(tableName,true);

            // Assert
            Assert.NotNull(table);
            Assert.True(TableTestsHelper.DoesTableExists(tableName));
            Assert.Equal(tableName, table.Name);
            
            // Cleanup 
            TableTestsHelper.DeleteTable(table);
        }
        
        [Fact]
        public async Task GetCloudTableAsync_WithTableThatDoesntExistAndCreateEqualsFalse_ShouldReturnNull()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            
            // Act
            var table = await tables.GetCloudTableAsync(tableName, false);

            // Assert
            Assert.Null(table);
            Assert.False(TableTestsHelper.DoesTableExists(tableName));
        }

        [Fact]
        public async Task GetCloudTableAsync_WithValidTableNameForTableThatDoesExists_ShouldReturnTable()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            
            // Act
            var table = await tables.GetCloudTableAsync(tableName);

            // Assert
            Assert.NotNull(table);
            Assert.True(TableTestsHelper.DoesTableExists(tableName));
            Assert.Equal(tableName, table.Name);
            
            // Cleanup 
            TableTestsHelper.DeleteTable(temporaryTable);
        }
        
        // CreateCloudTable1
        //    string
        [Fact]
        public async Task CreateCloudTable1_WithNullTableName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>tables.CreateCloudTableAsync((string)null));

            // Assert
            Assert.Equal("tableName", exception.ParamName);
            Assert.StartsWith("The table name cannot be null or empty", exception.Message);
            
        }
        
        [Fact]
        public async Task CreateCloudTable1_WithValidTableName_ShouldCreateTableAndReturnTable()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            
            // Act
            var table = await tables.CreateCloudTableAsync(tableName);

            // Assert
            Assert.NotNull(table);
            Assert.True(TableTestsHelper.DoesTableExists(tableName));
            Assert.Equal(tableName, table.Name);
            
            // Cleanup
            TableTestsHelper.DeleteTable(table);
        }
        
        // CreateCloudTable2
        //    cloudTable
        [Fact]
        public async Task CreateCloudTable2_WithNullCloudTable_ShouldThrowArgumentNullException()
        {
            // Arrange
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>tables.CreateCloudTableAsync((CloudTable)null));

            // Assert
            Assert.Equal("cloudTable", exception.ParamName);
            Assert.StartsWith("The cloud table cannot be null", exception.Message);
        }
        
        [Fact]
        public async Task CreateCloudTable2_WithCloudTable_ShouldCreateTableAndReturnTable()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            var temporaryTable = TableTestsHelper.GetCloudTable(tableName);
            
            // Act
            var table = await tables.CreateCloudTableAsync(temporaryTable);

            // Assert
            Assert.NotNull(table);
            Assert.True(TableTestsHelper.DoesTableExists(tableName));
            Assert.Equal(tableName, table.Name);

            // Cleanup
            TableTestsHelper.DeleteTable(table);
        }
        
        // DeleteCloudTable1
        //    string
        [Fact]
        public async Task DeleteCloudTableAsync_WithNullTableName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>tables.DeleteCloudTableAsync((string)null));

            // Assert
            Assert.Equal("tableName", exception.ParamName);
            Assert.StartsWith("The table name cannot be null or empty", exception.Message);
            
        }

        [Fact]
        public async Task DeleteCloudTableAsync1_WithTableNameThatExists_ShouldDeleteTableAndReturnTrue()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            
            // Act
            var wasDeleted = await tables.DeleteCloudTableAsync(tableName);

            // Assert
            Assert.True(wasDeleted);
            Assert.False(TableTestsHelper.DoesTableExists(temporaryTable.Name));
        }
        
        [Fact]
        public async Task DeleteCloudTableAsync1_WithTableNameThatDoesNotExists_ShouldDoNothingAndReturnFalse()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            
            // Act
            var wasDeleted = await tables.DeleteCloudTableAsync(tableName);

            // Assert
            Assert.False(wasDeleted);
            Assert.False(TableTestsHelper.DoesTableExists(tableName));
            
        }
             
        // DeleteCloudTable2
        //    cloudTable
        [Fact]
        public async Task DeleteCloudTable2_WithNullCloudTable_ShouldThrowArgumentNullException()
        {
            // Arrange
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>tables.DeleteCloudTableAsync((CloudTable)null));

            // Assert
            Assert.Equal("cloudTable", exception.ParamName);
            Assert.StartsWith("The cloud table cannot be null", exception.Message);
        }
        
        [Fact]
        public async Task DeleteCloudTableAsync2_WithCloudTableThatExists_ShouldDeleteTableAndReturnTrue()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            
            // Act
            var wasDeleted = await tables.DeleteCloudTableAsync(temporaryTable);

            // Assert
            Assert.True(wasDeleted);
            Assert.False(TableTestsHelper.DoesTableExists(temporaryTable.Name));
        }

        [Fact] public async Task DeleteCloudTableAsync2_WithCloudTableThatDoesNotExists_ShouldDoNothingAndReturnFalse()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            var temporaryTable = TableTestsHelper.GetCloudTable(tableName);
            
            // Act
            var wasDeleted = await tables.DeleteCloudTableAsync(temporaryTable);

            // Assert
            Assert.False(wasDeleted);
            Assert.False(TableTestsHelper.DoesTableExists(tableName));
            
        }
        
        // ExistsAsync
        [Fact] public async Task DoesCloudTableExistsAsync_WithNullTableName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>tables.DoesCloudTableExistsAsync((string) null));

            // Assert
            Assert.Equal("tableName", exception.ParamName);
            Assert.StartsWith("The table name cannot be null or empty", exception.Message);
            
        }
        
        [Fact]
        public async Task DoesCloudTableExistsAsync_WithTableThatExists_ShouldReturnTrue()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            
            // Act
            var doesTableExists = await tables.DoesCloudTableExistsAsync(tableName);

            // Assert
            Assert.True(doesTableExists);
            Assert.True(TableTestsHelper.DoesTableExists(tableName));
            
            // Cleanup
            TableTestsHelper.DeleteTable(temporaryTable);
        }

        [Fact]
        public async Task DoesCloudTableExistsAsync_WithTableThatDoesNotExists_ShouldReturnFalse()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            
            // Act
            var doesTableExists = await tables.DoesCloudTableExistsAsync(tableName);

            // Assert
            Assert.False(doesTableExists);
            Assert.False(TableTestsHelper.DoesTableExists(tableName));
        }

        // Exists
        [Fact]
        public void DoesCloudTableExists_WithNullTableName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>tables.DoesCloudTableExists((string)null));

            // Assert
            Assert.Equal("tableName", exception.ParamName);
            Assert.StartsWith("The table name cannot be null or empty", exception.Message);
            
        }

        [Fact]
        public void DoesCloudTableExists_WithTableThatExists_ShouldReturnTrue()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            var temporaryTable = TableTestsHelper.CreateTable(tableName);
            
            // Act
            var doesTableExists = tables.DoesCloudTableExists(tableName);

            // Assert
            Assert.True(doesTableExists);
            Assert.True(TableTestsHelper.DoesTableExists(tableName));
            
            // Cleanup
            TableTestsHelper.DeleteTable(temporaryTable);
        }

        [Fact]
        public void DoesCloudTableExists_WithTableThatDoesNotExists_ShouldReturnFalse()
        {
            // Arrange
            var tableName = TableTestsHelper.GetTemporaryName();
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            
            // Act
            var doesTableExists = tables.DoesCloudTableExists(tableName);

            // Assert
            Assert.False(doesTableExists);
            Assert.False(TableTestsHelper.DoesTableExists(tableName));
        }
        
        // ListTablesAsync
        [Fact]
        public async Task ListTablesAsync_WithNoParameter_ShouldReturnAListOfTables()
        {
            // Arrange
            var tables = new Tables(TableTestsHelper.DevelopmentConnectionString);
            const int numberOfTablesToCreate = 5;
            var temporaryTables = new string[numberOfTablesToCreate];
            for (var i = 0; i < numberOfTablesToCreate; i++)
            {
                var tableName = TableTestsHelper.GetTemporaryName();
                temporaryTables[i] = tableName;
                TableTestsHelper.CreateTable(tableName);
            }
            
            // Act
            var tableLists = await tables.GetListOfTablesAsync();

            // Assert
            Assert.NotNull(tableLists);
            
            var cloudTables = tableLists.ToList();
            Assert.True(cloudTables.Count() >= 5);
            foreach (var temporaryTable in temporaryTables)
            {
                Assert.True(cloudTables.Exists(table => table.Name == temporaryTable));
            }
            
            // Cleanup
            for (var i = 0; i < numberOfTablesToCreate; i++)
            {
                TableTestsHelper.DeleteTable(temporaryTables[i]);
            }
        }
    }
}