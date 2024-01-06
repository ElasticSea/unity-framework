using ElasticSea.Framework.Util;
using NUnit.Framework;

namespace ElasticSea.Framework.Tests.Util
{
    public class PriorityPoolTests
    {
        [Test]
        public void PriorityPool()
        {
            var id = 0;
            var pool = new PriorityPool<int>(0, () =>
            {
                var generatedId = id;
                id++;
                return generatedId;
            });
            
            Assert.AreEqual(0, pool.Get());
            Assert.AreEqual(1, pool.Get());
            Assert.AreEqual(2, pool.Get());
            Assert.AreEqual(3, pool.Get());
            Assert.AreEqual(4, pool.Get());
            Assert.AreEqual(5, pool.Get());
            
            pool.Put(3);
            pool.Put(4);
            pool.Put(1);
            
            Assert.AreEqual(1, pool.Get());
            Assert.AreEqual(3, pool.Get());
            Assert.AreEqual(4, pool.Get());
        }
    }
}