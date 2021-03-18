using NUnit.Framework;
using ElasticSea.Framework.Extensions;
using Assert = UnityEngine.Assertions.Assert;

namespace ElasticSea.Framework.Tests.Extensions
{
    public class FloatExtensionsTests
    {
        [TestCase(-0.06f, 0.02f, -0.06f)]
        [TestCase(-0.058f, 0.02f, -0.06f)]
        [TestCase(-0.062f, 0.02f, -0.06f)]
        public void TestApprox2(float input, float roundTo, float expected)
        {
            TestContext.AddFormatter<float>(val => ((float)val).ToString("F6"));
            
            var output = input.RoundTo(roundTo);
            
            Assert.AreEqual(expected, output);
        }
    }
}