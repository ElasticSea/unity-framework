using System;
using System.Diagnostics;
using ElasticSea.Framework.Util;
using NUnit.Framework;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

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
        
        [Test]
        public void CommandLineArgValues_NotFound()
        {
            var (found, values) = Utils.CommandLineArgValues("application.exe -key1 a b -key2 a".Split(' '), "missingKey");
            Assert.AreEqual(false, found);
            CollectionAssert.AreEqual(new string[0], values);
        }
        
        [Test]
        public void CommandLineArgValues_Found()
        {
            var (found, values) = Utils.CommandLineArgValues("application.exe -key1 a b -key2 c".Split(' '), "key2");
            Assert.AreEqual(true, found);
            CollectionAssert.AreEqual(new []{"c"}, values);
        }
        
        [Test]
        public void HexNumber()
        {
            var rng = new Random(249047250);
            var hex = Utils.GetRandomHexNumber(rng, 20);
            Assert.AreEqual("0c9e010455fdeb6fd3d6", hex);
        }
        
        [Test]
        public void NearestNeighbourScaleDown()
        {
            var original = new byte[]
            {
                1, 0, 1, 0,
                0, 0, 0, 0,
                1, 0, 1, 0,
                0, 0, 0, 0,
            };
            
            var expected = new byte[]
            {
                1, 1,
                1, 1,
            };

            var actual = Utils.NearestNeighbourScaleDown(original, 1, 4, 4, 1);
            
            CollectionAssert.AreEqual(expected, actual.bytes);
        }
        
        [Test]
        public void NearestNeighbour24BitScaleDown()
        {
            var original = new byte[]
            {
                1,2,3, 0,0,0, 1,2,3, 0,0,0,
                0,0,0, 0,0,0, 0,0,0, 0,0,0,
                1,2,3, 0,0,0, 1,2,3, 0,0,0,
                0,0,0, 0,0,0, 0,0,0, 0,0,0,
            };
            
            var expected = new byte[]
            {
                1,2,3, 1,2,3,
                1,2,3, 1,2,3,
            };

            var actual = Utils.NearestNeighbourScaleDown(original, 3, 4, 4, 1);
            
            CollectionAssert.AreEqual(expected, actual.bytes);
        }
        
        [Test]
        public void NearestNeighbourScaleDown_MT()
        {
            var original = new byte[]
            {
                1, 0, 1, 0,
                0, 0, 0, 0,
                1, 0, 1, 0,
                0, 0, 0, 0,
            };
            
            var expected = new byte[]
            {
                1, 1,
                1, 1,
            };

            var actual = Utils.NearestNeighbourScaleDownMT(original, 1, 4, 4, 1);
            
            CollectionAssert.AreEqual(expected, actual.bytes);
        }
        
        [Test]
        public void NearestNeighbour24BitScaleDown_MT()
        {
            var original = new byte[]
            {
                1,2,3, 0,0,0, 1,2,3, 0,0,0,
                0,0,0, 0,0,0, 0,0,0, 0,0,0,
                1,2,3, 0,0,0, 1,2,3, 0,0,0,
                0,0,0, 0,0,0, 0,0,0, 0,0,0,
            };
            
            var expected = new byte[]
            {
                1,2,3, 1,2,3,
                1,2,3, 1,2,3,
            };

            var actual = Utils.NearestNeighbourScaleDownMT(original, 3, 4, 4, 1);
            
            CollectionAssert.AreEqual(expected, actual.bytes);
        }
    }
}