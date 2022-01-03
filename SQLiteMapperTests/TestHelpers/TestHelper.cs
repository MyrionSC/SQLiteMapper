using NUnit.Framework;

namespace SQLiteMapperTests.TestHelpers
{
    public static class TestHelper
    {
        public static void AssertEqualNoWhitepace(string expected, string actual)
        {
            string expectedClean = expected.Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
            string actualClean = actual.Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
            Assert.AreEqual(expectedClean, actualClean);
        }
    }
}