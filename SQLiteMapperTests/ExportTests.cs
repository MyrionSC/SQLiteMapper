using business_layer_test;
using NUnit.Framework;
using SQLiteMapper;

namespace SQLiteMapperTests
{
    public class ExportTests
    {
        [Test]
        public void Users_Export()
        {
            var input = System.IO.File.ReadAllText(@"testfiles/users_input.json"); 
            var result = SqLiteMapper.Export(input);
            var expected = System.IO.File.ReadAllText(@"testfiles/users_export_output.sql");
            TestHelper.AssertEqualNoWhitepace(expected, result);
        }
    }
}