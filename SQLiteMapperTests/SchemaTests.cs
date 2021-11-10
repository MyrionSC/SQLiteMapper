using business_layer_test;
using Newtonsoft.Json;
using NUnit.Framework;
using SQLiteMapper;
using SQLiteMapper.Exceptions;

namespace SQLiteMapperTests
{
    public class SchemaTests
    {
        
        [Test]
        public void BadSchema_BadValue()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/schema_bad_value_input.json"); 
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            Assert.Throws<SchemaBadValueException>(() => SqLiteMapper.GenerateTableAndInsertStatements(inputParsed));
        }
        
        [Test]
        public void BadSchema_NoCorrespondingData()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/schema_bad_value_input.json"); 
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            Assert.Throws<SchemaNoCorrespondingDataException>(() => SqLiteMapper.GenerateTableAndInsertStatements(inputParsed));
        }
        
        
        [Test]
        public void CorrectSchema_AllGood_Gen()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/schema_correct_input.json"); 
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            var result = SqLiteMapper.GenerateTableAndInsertStatements(inputParsed);
            var expected = System.IO.File.ReadAllText(@"testfiles/schema_correct_output.sql");
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
        
        [Test]
        public void CorrectSchema_AllGood_Query()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/schema_correct_input.json"); 
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            var result = SqLiteMapper.GenerateTableAndInsertStatements(inputParsed);
            var expected = @"[{""name"": ""Martin"", ""age"": null}]";
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
        
        [Test]
        public void CorrectSchema_EmptyData()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/schema_empty_data_input.json"); 
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            var result = SqLiteMapper.GenerateTableAndInsertStatements(inputParsed);
            var expected = System.IO.File.ReadAllText(@"testfiles/users_output.sql");
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
        
        
    }
}