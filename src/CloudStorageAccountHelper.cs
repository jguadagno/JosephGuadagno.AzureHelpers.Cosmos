using System;
using Microsoft.Azure.Cosmos.Table;

namespace JosephGuadagno.AzureHelpers.Cosmos
{
    public static class CloudStorageAccountHelper
    {
        /// <summary>
        /// Gets a reference to a storage account from a Url
        /// </summary>
        /// <param name="storageConnectionString">The Url to the storage account</param>
        /// <returns>A CloudStorageAccount</returns>
        /// <exception cref="FormatException">Throws if the format of the <see cref="storageConnectionString"/> is invalid</exception>
        /// <exception cref="ArgumentException">Throws if the <see cref="storageConnectionString"/> that was passed is missing some required elements</exception>
        public static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine(
                    "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine(
                    "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }
    }
}