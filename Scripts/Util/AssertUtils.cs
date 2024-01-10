using UnityEngine;
using UnityEngine.Assertions;

namespace ElasticSea.Framework.Util
{
    public class AssertUtils
    {
        public static void AreApproximatelyEqual(Vector3 expected, Vector3 actual)
        {
            Assert.AreApproximatelyEqual(expected.x, actual.x);
            Assert.AreApproximatelyEqual(expected.y, actual.y);
            Assert.AreApproximatelyEqual(expected.z, actual.z);
        }
    }
}