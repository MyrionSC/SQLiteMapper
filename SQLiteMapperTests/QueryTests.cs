using business_layer_test;
using Newtonsoft.Json;
using NUnit.Framework;
using SQLiteMapper;

namespace SQLiteMapperTests
{
    public class QueryTests
    {
        [Test]
        public void GetUser()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/users_input.json");
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            var result = SqLiteMapper.ExecuteQuery(inputParsed);
            var expected = @"[{""name"": ""Martin"", ""age"": 42}]";
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
        
        [Test]
        public void GetUserNested()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/users_nested_input.json");
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            var result = SqLiteMapper.ExecuteQuery(inputParsed);
            var expected = @"[{""name"": ""Martin"", ""age"": 42, ""address_city"": ""Aalborg"", ""address_street"": ""Søndergade 58 2th""}]";
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
        
        [Test]
        public void GetUserBool()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/users_bool_input.json");
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            var result = SqLiteMapper.ExecuteQuery(inputParsed);
            var expected = @"[{""name"":""Martin"",""age"":42,""isAlive"":1},{""name"":""Casper"",""age"":2,""isAlive"":1}]";
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
        
        [Test]
        public void GetUsersCompanies()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/users_companies_input.json");
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            var result = SqLiteMapper.ExecuteQuery(inputParsed);
            var expected = @"[{""name"":""Casper"",""worksForCompany"":""Commentor""},{""name"":""Martin"",""worksForCompany"":""Commentor""}]";
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
    }
}