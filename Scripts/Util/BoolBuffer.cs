namespace ElasticSea.Framework.Util
{
    public class BoolBuffer : CircularBuffer<bool>
    {
        public BoolBuffer(int capacity) : base(capacity)
        {
        }

        public bool Average()
        {
            var falseCount = 0;
            var trueCount = 0;
            var bufferLength = buffer.Length;
            
            for (var i = 0; i < bufferLength; i++)
            {
                if (buffer[i])
                {
                    trueCount++;
                }
                else
                {
                    falseCount++;
                }
            }

            return trueCount > falseCount;
        }

        public void Clear(bool value = false)
        {
            var bufferLength = buffer.Length;
            for (var i = 0; i < bufferLength; i++)
            {
                buffer[i] = value;
            }
        }
    }
}