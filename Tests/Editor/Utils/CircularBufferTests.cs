using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ElasticSea.Framework.Util;

namespace ElasticSea.Framework.Tests.Extensions
{
    public class CircularBufferTests
    {
        [Test]
        public void TestHalfFilled()
        {
            var circularBuffer = new CircularBuffer<int>(5) {1, 2, 3};
            Assert.AreEqual(circularBuffer.Count, 3);
            Assert.AreEqual(circularBuffer.Capacity, 5);
        }
        
        [Test]
        public void TestHalfFilledEnumeration()
        {
            var circularBuffer = new CircularBuffer<int>(5) {1, 2, 3};
            CollectionAssert.AreEqual(new List<int> {1, 2, 3}, circularBuffer.ToList());
        }

        [Test]
        public void TestOverFilled()
        {
            var circularBuffer = new CircularBuffer<int>(5) {1, 2, 3, 4, 5, 6, 7};
            Assert.AreEqual(circularBuffer.Count, 5);
            Assert.AreEqual(circularBuffer.Capacity, 5);
        }

        [Test]
        public void TestOverFilledEnumeration()
        {
            var circularBuffer = new CircularBuffer<int>(5) {1, 2, 3, 4, 5, 6, 7};
            CollectionAssert.AreEqual(new List<int> {3, 4, 5, 6, 7}, circularBuffer.ToList());
        }
    }
}