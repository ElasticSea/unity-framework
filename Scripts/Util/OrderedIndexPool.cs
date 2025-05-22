using System;

namespace ElasticSea.Framework.Util
{
    public class OrderedIndexPool 
    {
        private bool[] occupied = new bool[1];
        private int count = 1;
        private int firstFreeIndex = 0;
        
        public int Get()
        {
            var freeIndex = firstFreeIndex;

            do
            {
                firstFreeIndex++;
                if (firstFreeIndex >= occupied.Length)
                {
                    count *= 2;
                    occupied = occupied.EnsureArray(count);
                }
            } while (occupied[firstFreeIndex]);
            
            
            occupied[freeIndex] = true;
            
            return freeIndex;
        }

        public void Put(int index)
        {
            if (index >= count)
                throw new Exception("index does not belong to this pool");
            
            if (!occupied[index])
                throw new Exception("Index is already in the pool");
            
            occupied[index] = false;

            if (index < firstFreeIndex)
                firstFreeIndex = index;
        }
    }
}