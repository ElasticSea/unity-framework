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

        [Test]
        public void AlternativeNameNoBrackets_NothingTaken()
        {
            var alternative = Utils.AlternativeNameNoBrackets(new string[0], "name");
            Assert.AreEqual("name", alternative);
        }

        [Test]
        public void AlternativeNameNoBrackets_SameName()
        {
            var alternative = Utils.AlternativeNameNoBrackets(new[] {"name"}, "name");
            Assert.AreEqual("name 1", alternative);
        }

        [Test]
        public void AlternativeNameNoBrackets_NumberWithoutSpace()
        {
            var alternative = Utils.AlternativeNameNoBrackets(new[] {"name1"}, "name1");
            Assert.AreEqual("name1 1", alternative);
        }

        [Test]
        public void AlternativeNameNoBrackets_SameNameWithDigit()
        {
            var alternative = Utils.AlternativeNameNoBrackets(new[] {"name"}, "name");
            Assert.AreEqual("name 1", alternative);
        }

        [Test]
        public void AlternativeNameNoBrackets_DifferentName()
        {
            var alternative = Utils.AlternativeNameNoBrackets(new[] {"name"}, "name 1");
            Assert.AreEqual("name 1", alternative);
        }

        [Test]
        public void AlternativeNameNoBrackets_MultipleSame()
        {
            var alternative = Utils.AlternativeNameNoBrackets(new[] {"name", "name 1", "name 2"}, "name");
            Assert.AreEqual("name 3", alternative);
        }

        [Test]
        public void AlternativeNameNoBrackets_SimilarButDifferent()
        {
            var alternative = Utils.AlternativeNameNoBrackets(new[] {"name", "name1"}, "name");
            Assert.AreEqual("name 1", alternative);
        }
    }
}