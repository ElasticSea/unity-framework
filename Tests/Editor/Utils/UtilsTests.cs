using ElasticSea.Framework.Util;
using NUnit.Framework;
using UnityEngine;

namespace ElasticSea.Framework.Tests.Extensions
{
    public class UtilsTests
    {
        [Test]
        public void AverageRotation()
        {
            var rotations = new[] {new Vector3(100, 90, 80)};
            var average = Utils.AverageRotation(rotations);
            Assert.AreEqual(new Vector3(100, 90, 80), average);
        }
        
        [Test]
        public void AverageRotation2()
        {
            var rotations = new[]
            {
                new Vector3(180, 0, 0),
                new Vector3(-180, 0, 0),
                new Vector3(180, 0, 0),
            };
            var average = Utils.AverageRotation(rotations);
            Assert.AreEqual(new Vector3(180, 0, 0), average);
        }
    }
}