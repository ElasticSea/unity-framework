using System;
using ElasticSea.Framework.Util;
using NUnit.Framework;

namespace ElasticSea.Framework.Tests.Editor.Utils
{
    [TestFixture]
    public class OrderedIndexPoolTests
    {
        private OrderedIndexPool pool;

        [SetUp]
        public void Setup()
        {
            pool = new OrderedIndexPool();
        }

        [Test]
        public void Get_FirstCall_ReturnsZero()
        {
            Assert.AreEqual(0, pool.Get());
        }

        [Test]
        public void Get_MultipleCalls_ReturnsSequentialIndices()
        {
            Assert.AreEqual(0, pool.Get());
            Assert.AreEqual(1, pool.Get());
            Assert.AreEqual(2, pool.Get());
        }

        [Test]
        public void Put_FreesIndex_CanBeReusedByGet()
        {
            var id0 = pool.Get();
            pool.Get();

            pool.Put(id0);

            Assert.AreEqual(0, pool.Get());
        }

        [Test]
        public void Put_IndexHigherThanCount_ThrowsException()
        {
            Assert.Throws<Exception>(() => pool.Put(100));
        }

        [Test]
        public void Put_AlreadyFreeIndex_ThrowsException()
        {
            int idx = pool.Get();
            pool.Put(idx);
            Assert.Throws<Exception>(() => pool.Put(idx)); // double free
        }

        [Test]
        public void Get_AfterPutReusesLowestFreeIndex()
        {
            pool.Get(); // 0
            pool.Get(); // 1
            pool.Get(); // 2

            pool.Put(1);
            pool.Put(0);

            Assert.AreEqual(0, pool.Get());
            Assert.AreEqual(1, pool.Get());
            Assert.AreEqual(3, pool.Get());
        }
    }
}
