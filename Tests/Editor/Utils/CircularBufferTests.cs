using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ElasticSea.Framework.Util;

namespace ElasticSea.Framework.Tests.Extensions
{
    public class CircularBufferTests
    {
        [Test]
        public void CircularBufferTest1()
        {
            var circularBuffer = new CircularBuffer<int>(5) {1, 2, 3};

            CollectionAssert.AreEqual(new List<int> {1, 2, 3}, circularBuffer.ToList());
        }

        [Test]
        public void CircularBufferTest2()
        {
            var circularBuffer = new CircularBuffer<int>(5) {1, 2, 3, 4, 5, 6, 7};

            CollectionAssert.AreEqual(new List<int> {3, 4, 5, 6, 7}, circularBuffer.ToList());
        }
    }
}