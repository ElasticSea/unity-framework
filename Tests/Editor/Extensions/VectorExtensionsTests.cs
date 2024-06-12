using ElasticSea.Framework.Extensions;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace ElasticSea.Framework.Tests.Extensions
{
    public class VectorExtensionsTests
    {
        [TestCase]
        public void TestInverseLerpMiddle()
        {
            var min = new Vector3(0, 0, 0);
            var max = new Vector3(1, 1, 1);
            var value = new Vector3(0.5f, 0.5f, 0.5f);
            var t = min.InverseLerpWeird(max, value);
            Assert.AreEqual(0.5f, t);
        }
        
        [TestCase]
        public void TestInverseLerpFrom()
        {
            var min = new Vector3(0, 0, 0);
            var max = new Vector3(1, 1, 1);
            var value = min;
            var t = min.InverseLerpWeird(max, value);
            Assert.AreEqual(0, t);
        }
        
        [TestCase]
        public void TestInverseLerpTo()
        {
            var min = new Vector3(0, 0, 0);
            var max = new Vector3(1, 1, 1);
            var value = max;
            var t = min.InverseLerpWeird(max, value);
            Assert.AreEqual(1, t);
        }
        
        [TestCase]
        public void TestInverseLerpOverflowBottom()
        {
            var min = new Vector3(0, 0, 0);
            var max = new Vector3(1, 1, 1);
            var value = new Vector3(-1, -1, -1);
            var t = min.InverseLerpWeird(max, value);
            Assert.AreEqual(-1, t);
        }
        
        [TestCase]
        public void TestInverseLerpOverflowTop()
        {
            var min = new Vector3(0, 0, 0);
            var max = new Vector3(1, 1, 1);
            var value = new Vector3(2, 2, 2);
            var t = min.InverseLerpWeird(max, value);
            Assert.AreEqual(2, t);
        }
    }
}