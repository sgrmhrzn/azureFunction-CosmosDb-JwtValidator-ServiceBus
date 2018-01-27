using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionAppCosmos
{
    public static class CosmosDBUpsertFunction
    {
        static CosmosDBUpsertFunction()
        {

        }
        [FunctionName("CosmosDBUpsertFunction")]
        public static object Run(
            [HttpTrigger(WebHookType = "genericJson")]
            HttpRequestMessage req,
            TraceWriter log,
            [DocumentDB("cosmosdbt", "MyCollection",
                        CreateIfNotExists =true,
                        ConnectionStringSetting = "myCosmosDBConnection")]
                        out dynamic document)
        {
            log.Info($"Webhook was triggered!");
            var task = req.Content.ReadAsStringAsync();
            task.Wait();
            string jsonContent = task.Result;
            dynamic data = JsonConvert.DeserializeObject(jsonContent);

            document = data;
            if (data != null)
            {
                return req.CreateResponse(HttpStatusCode.OK, new
                {
                    greeting = $"Will upsert document!"
                });
            }
            else return req.CreateResponse(HttpStatusCode.BadRequest, new { error = "Document was empty!" });
        }
    }
}
