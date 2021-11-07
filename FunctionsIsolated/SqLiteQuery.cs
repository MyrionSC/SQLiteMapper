using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using SQLiteMapper;

namespace FunctionsIsolated
{
    public static class SqLiteQuery
    {
        [Function("SQLiteQuery")]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var mapperInput = JsonConvert.DeserializeObject<SqLiteMapperInput>(requestBody);
            var result = SqLiteMapper.ExecuteQuery(mapperInput);
            
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await response.WriteStringAsync(result);
            return response;
        }
    }
}
