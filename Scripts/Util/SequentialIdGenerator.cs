namespace ElasticSea.Framework.Util
{
    public class SequentialIdGenerator
    {
        private readonly PriorityPool<int> pool;

        public SequentialIdGenerator()
        {
            var id = 0;
            pool = new PriorityPool<int>(0, () =>
            {
                var generatedId = id;
                id++;
                return generatedId;
            });
        }
        
        public int GenerateId() => pool.Get();
        public void ReturnId(int id) => pool.Put(id);
    }
}