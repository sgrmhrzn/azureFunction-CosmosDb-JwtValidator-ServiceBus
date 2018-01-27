using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace CosmosDbTrigger
{
    public static class DbTrigger
    {
        [FunctionName("DbTrigger")]
        public static void Run([CosmosDBTrigger(
            databaseName: "cosmosCsharpDb",
            collectionName: "TaskCollection",
            ConnectionStringSetting = "myCosmosDBConnection",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input, TraceWriter log)
        {
            if (input != null && input.Count > 0)
            {
                log.Verbose("Documents modified " + input.Count);
                log.Verbose("First document Id " + input[0].Id);
            }
        }
    }
}
