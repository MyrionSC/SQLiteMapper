using Newtonsoft.Json;
using NUnit.Framework;
using SQLiteMapper;
using SQLiteMapper.Exceptions;
using SQLiteMapperTests.TestHelpers;

namespace SQLiteMapperTests
{
    public class GenerateTableAndInsertStatementsTests
    {
        [Test]
        public void GenerateUsers()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/users_input.json"); 
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            var result = SqLiteMapper.GenerateTableAndInsertStatements(inputParsed);
            var expected = System.IO.File.ReadAllText(@"testfiles/users_output.sql");
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
        
        [Test]
        public void GenerateUsersCompanies()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/users_companies_input.json"); 
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            var result = SqLiteMapper.GenerateTableAndInsertStatements(inputParsed);
            var expected = System.IO.File.ReadAllText(@"testfiles/users_companies_output.sql");
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
        
        [Test]
        public void DefaultToTextWhenAllNull()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/users_input_null_field.json"); 
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            var result = SqLiteMapper.GenerateTableAndInsertStatements(inputParsed);
            var expected = System.IO.File.ReadAllText(@"testfiles/users_input_null_field.sql");
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
        
        [Test]
        public void RemoveSymbolsUsers()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/remove_symbols_input.json"); 
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            var result = SqLiteMapper.GenerateTableAndInsertStatements(inputParsed);
            var expected = System.IO.File.ReadAllText(@"testfiles/remove_symbols_output.sql");
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
        
        [Test]
        public void ThrowExceptionOnEmptyData()
        {
            var inputStr = System.IO.File.ReadAllText(@"testfiles/users_empty_companies_input.json"); 
            var inputParsed = JsonConvert.DeserializeObject<SqLiteMapperInput>(inputStr);
            Assert.Throws<DataEmptyException>(() => SqLiteMapper.GenerateTableAndInsertStatements(inputParsed)) ;
        }
    }
}