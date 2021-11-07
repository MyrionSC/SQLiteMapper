using business_layer_test;
using Newtonsoft.Json;
using NUnit.Framework;
using SQLiteMapper;

namespace SQLiteMapperTests
{
    public class ExecuteTests
    {
        [Test]
        public void GetSingle()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/users_input.json");
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            var result = SqLiteMapper.ExecuteQuery(inputParsed);
            var expected = @"[{""name"": ""Martin"", ""age"": 42}]";
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
    }
}