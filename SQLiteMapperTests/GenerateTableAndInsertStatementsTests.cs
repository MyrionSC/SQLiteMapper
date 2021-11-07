using business_layer_test;
using Newtonsoft.Json;
using NUnit.Framework;
using SQLiteMapper;

namespace SQLiteMapperTests
{
    public class GenerateTableAndInsertStatementsTests
    {
        [Test]
        public void CreateSingleTable()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/users_input.json"); 
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            var result = SqLiteMapper.GenerateTableAndInsertStatements(inputParsed);
            var expected = System.IO.File.ReadAllText(@"testfiles/users_export_output.sql");
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
    }
}