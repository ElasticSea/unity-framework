using ElasticSea.Framework.Scripts.Util;
using ElasticSea.Framework.Util;
using NUnit.Framework;
using UnityEngine;

namespace ElasticSea.Framework.Tests.Extensions
{
    public class PhysicsUtilsTests
    {
        [Test]
        public void BoxTriangleIntersectorOldTest1()
        {
            var box = Utils.Bounds(Vector3.zero, Vector3.one * 10);
            var triangle = new[] { new Vector3(12, 9, 9), new Vector3(9, 12, 9), new Vector3(19, 19, 20) };
             
            var intersect = BoxTriangleIntersectorOld.IsIntersecting(box, triangle[0], triangle[1], triangle[2]);
            Assert.AreEqual(false, intersect);
        }
        
        [Test]
        public void BoxTriangleIntersectorTest1()
        {
            var box = Utils.Bounds(Vector3.zero, Vector3.one * 10);
            var triangle = new[] { new Vector3(12, 9, 9), new Vector3(9, 12, 9), new Vector3(19, 19, 20) };
             
            var intersect = BoxTriangleIntersector.IsIntersecting(box, triangle[0], triangle[1], triangle[2]);
            Assert.AreEqual(false, intersect);
        }
    }
}