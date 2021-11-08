using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using SQLiteMapper;

namespace Functions
{
    public static class SQLiteGenerateTableInsertStatements
    {
        [Function("SQLiteGenerateTableInsertStatements")]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var mapperInput = JsonConvert.DeserializeObject<SqLiteMapperInput>(requestBody);
            var result = SqLiteMapper.GenerateTableAndInsertStatements(mapperInput);
            
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "plain/text; charset=utf-8");
            await response.WriteStringAsync(result);
            return response;
        }
    }
}
