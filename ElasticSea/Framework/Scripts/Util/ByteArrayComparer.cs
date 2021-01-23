using System;
using System.Collections.Generic;

namespace ElasticSea.Framework.Util
{
    public class ByteArrayComparer : EqualityComparer<byte[]>
    {
        public override bool Equals(byte[] first, byte[] second)
        {
            if (first == null || second == null) {
                // null == null returns true.
                // non-null == null returns false.
                return first == second;
            }
            if (ReferenceEquals(first, second)) {
                return true;
            }
            if (first.Length != second.Length) {
                return false;
            }

            for (var i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }

            return true;
        }
        
        public override int GetHashCode(byte[] obj)
        {
            if (obj == null) {
                throw new ArgumentNullException("obj");
            }
            // quick and dirty, instantly identifies obviously different
            // arrays as being different
            return obj.Length;
        }
    }
}