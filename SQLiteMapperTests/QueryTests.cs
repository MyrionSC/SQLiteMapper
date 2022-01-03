using Newtonsoft.Json;
using NUnit.Framework;
using SQLiteMapper;
using SQLiteMapperTests.TestHelpers;

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
        
        [Test]
        public void RemoveSymbols()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/remove_symbols_input.json"); 
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            var result = SqLiteMapper.ExecuteQuery(inputParsed);
            var expected = @"[{""odata_etag"":""@123@"", ""name"": ""Martin"", ""age"": 42}]";
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
        
        [Test]
        public void SingleQuoteInContentEscape()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/single_quote_in_content_escape_input.json");
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            var result = SqLiteMapper.ExecuteQuery(inputParsed);
            var expected = "[{\"odata_etag\": \"W/\\\"datetime'2021-12-15T15%3A55%3A13.9148147Z'\\\"\"}]";
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
    }
}