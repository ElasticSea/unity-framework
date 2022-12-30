using System.Collections.Generic;
using ElasticSea.Framework.Util;

namespace ElasticSea.Framework.Scripts.Util
{
    public class GuidGenerator
    {
        private readonly int length;
        private readonly HashSet<string> taken;

        public GuidGenerator(int length)
        {
            this.length = length;
            this.taken = new HashSet<string>();
        }

        public string Next()
        {
            return Utils.GetUniqueHexNumber(taken, length);
        }
    }
}