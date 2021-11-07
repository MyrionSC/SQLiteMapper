using NUnit.Framework;
using SQLiteMapper;

namespace SQLiteMapperTests
{
    public class ExecuteTests
    {
        [Test]
        public void Test1()
        {
            var res = SqLiteMapper.Execute("");
            Assert.AreEqual("Martin", res);
        }
    }
}