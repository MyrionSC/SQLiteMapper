using System;
using NUnit.Framework;

namespace business_layer_test
{
    public static partial class TestHelper
    {
        public static void AssertEqualNoWhitepace(string expected, string actual)
        {
            string expectedClean = expected.Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
            string actualClean = actual.Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
            Assert.AreEqual(expectedClean, actualClean);
        }
    }
}