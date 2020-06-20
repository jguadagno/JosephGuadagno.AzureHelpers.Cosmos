using System.Net;
using Microsoft.Azure.Cosmos.Table;

namespace JosephGuadagno.AzureHelpers.Cosmos
{
    // TODO: Add Xml Documentation
    public class TableOperationResult  
    {
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

        public string ActivityId { get; }
        public string ETag { get; }
        public HttpStatusCode HttpStatusCode { get; }
        public double? RequestChange { get; }
        public object Result { get; }
        public string SessionToken { get; }
        
        /// <summary>
        /// Indicates whether the call was successful
        /// </summary>
        /// <remarks>This is a translation of the <see cref="HttpStatusCode"/> code property based on the operation requested.</remarks>
        public bool WasSuccessful { get; internal set; }
        
    }
}