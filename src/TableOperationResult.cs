using System.Net;
using Microsoft.Azure.Cosmos.Table;

namespace JosephGuadagno.AzureHelpers.Cosmos
{
    /// <summary>
    /// Represents the results of a table operation
    /// </summary>
    /// <remarks>This class is based off of the TableResult class in Microsoft.Azure.Cosmos.Table. For more info on the class check out https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.table.tableresult?view=azure-dotnet</remarks>
    public class TableOperationResult
    {
        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        /// <param name="tableResult">The TableResult to use</param>
        /// <remarks>For more info on the class check out https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.table.tableresult?view=azure-dotnet</remarks>
        public TableOperationResult(TableResult tableResult)
        {
            if (tableResult == null)
            {
                return;
            }

            ActivityId = tableResult.ActivityId;
            ETag = tableResult.Etag;
            HttpStatusCode = (HttpStatusCode) tableResult.HttpStatusCode;
            RequestChange = tableResult.RequestCharge;
            Result = tableResult.Result;
            SessionToken = tableResult.SessionToken;
        }

        /// <summary>
        /// Gets the activity id
        /// </summary>
        public string ActivityId { get; }

        /// <summary>
        /// Gets the ETag
        /// </summary>
        public string ETag { get; }

        /// <summary>
        /// Get the HTTP status code from the result of the operation
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; }

        /// <summary>
        /// Gets the request change for the operation
        /// </summary>
        public double? RequestChange { get; }

        /// <summary>
        /// Gets the result of the operation
        /// </summary>
        public object Result { get; }

        /// <summary>
        /// Gets the session token returned from results
        /// </summary>
        public string SessionToken { get; }

        /// <summary>
        /// Indicates whether the call was successful
        /// </summary>
        /// <remarks>This is a translation of the <see cref="HttpStatusCode"/> code property based on the operation requested.</remarks>
        public bool WasSuccessful { get; internal set; }
    }
}