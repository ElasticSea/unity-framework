using System;
using UnityEngine;
using static _Framework.Scripts.Util.Conversions.EndianBitConverter;

namespace Core.Util
{
    public static class ByteUtils
    {
        public static void Write(int value, byte[] bytes, ref int offset)
        {
            var valueBytes = Little.GetBytes(value);
            Array.Copy(valueBytes, 0, bytes, offset, 4);
            offset += 4;
        }
        
        public static void Write(int[] ints, byte[] bytes, ref int offset)
        {
            var lengthBytes = Little.GetBytes(ints.Length);
            Array.Copy(lengthBytes, 0, bytes, offset, 4);
            offset += 4;
            for (var i = 0; i < ints.Length; i++)
            {
                var intBytes = Little.GetBytes(ints[i]);
                Array.Copy(intBytes, 0, bytes, offset, 4);
                offset += 4;
            }
        }
        
        public static void Write(this Vector3[] vectors, byte[] bytes, ref int offset)
        {
            var lengthBytes = Little.GetBytes(vectors.Length);
            Array.Copy(lengthBytes, 0, bytes, offset, 4);
            offset += 4;
            for (var i = 0; i < vectors.Length; i++)
            {
                var vector = vectors[i];
                var x = Little.GetBytes(vector.x);
                var y = Little.GetBytes(vector.y);
                var z = Little.GetBytes(vector.z);
                Array.Copy(x, 0, bytes, offset, 4);
                offset += 4;
                Array.Copy(y, 0, bytes, offset, 4);
                offset += 4;
                Array.Copy(z, 0, bytes, offset, 4);
                offset += 4;
            }
        }
        
        public static void Write(this Vector2[] vectors, byte[] bytes, ref int offset)
        {
            var lengthBytes = Little.GetBytes(vectors.Length);
            Array.Copy(lengthBytes, 0, bytes, offset, 4);
            offset += 4;
            for (var i = 0; i < vectors.Length; i++)
            {
                var vector = vectors[i];
                var x = Little.GetBytes(vector.x);
                var y = Little.GetBytes(vector.y);
                Array.Copy(x, 0, bytes, offset, 4);
                offset += 4;
                Array.Copy(y, 0, bytes, offset, 4);
                offset += 4;
            }
        }
        
        public static void Write(this Color[] colors, byte[] bytes, ref int offset)
        {
            var lengthBytes = Little.GetBytes(colors.Length);
            Array.Copy(lengthBytes, 0, bytes, offset, 4);
            offset += 4;
            for (var i = 0; i < colors.Length; i++)
            {
                var colorBytes = ColorToBytes(colors[i]);
                Array.Copy(colorBytes, 0, bytes, offset, 4);
                offset += 4;
            }
        }
        
        public static int ReadInt(this byte[] bytes, ref int offset)
        {
            var value = Little.ToInt32(bytes, offset);
            offset += 4;
            return value;
        }
        
        public static int[] ReadInts(this byte[] bytes, ref int offset)
        {
            var length = Little.ToInt32(bytes, offset);
            offset += 4;

            var ints = new int[length];
            
            for (var i = 0; i < ints.Length; i++)
            {
                ints[i] = Little.ToInt32(bytes, offset);
                offset += 4;
            }

            return ints;
        }
        
        public static Vector3[] ReadVector3s(this byte[] bytes, ref int offset)
        {
            var length = Little.ToInt32(bytes, offset);
            offset += 4;

            var vector3s = new Vector3[length];
            
            for (var i = 0; i < vector3s.Length; i++)
            {
                var x = Little.ToSingle(bytes, offset);
                offset += 4;
                var y = Little.ToSingle(bytes, offset);
                offset += 4;
                var z = Little.ToSingle(bytes, offset);
                offset += 4;
                vector3s[i] = new Vector3(x, y, z);
            }

            return vector3s;
        }
        
        public static Vector2[] ReadVector2s(this byte[] bytes, ref int offset)
        {
            var length = Little.ToInt32(bytes, offset);
            offset += 4;

            var vector2s = new Vector2[length];
            
            for (var i = 0; i < vector2s.Length; i++)
            {
                var x = Little.ToSingle(bytes, offset);
                offset += 4;
                var y = Little.ToSingle(bytes, offset);
                offset += 4;
                vector2s[i] = new Vector2(x, y);
            }

            return vector2s;
        }
        
        public static Color[] ReadColors(this byte[] bytes, ref int offset)
        {
            var length = Little.ToInt32(bytes, offset);
            offset += 4;

            var colors = new Color[length];
            
            for (var i = 0; i < colors.Length; i++)
            {
                colors[i] = BytesToColor(bytes, offset);
                offset += 4;
            }

            return colors;
        }

        public static int GetBytesInVector3Array(int count) => 4 + (count * 3 * 4);
        public static int GetBytesInVector2Array(int count) => 4 + (count * 2 * 4);
        public static int GetBytesInIntArray(int count) => 4 + (count * 4);
        public static int GetBytesInColorArray(int count) => 4 + (count * 4);
        
        private static byte[] ColorToBytes(Color c)
        {
            return new[]
            {
                (byte) (c.r * 256),
                (byte) (c.g * 256),
                (byte) (c.b * 256),
                (byte) (c.a * 256)
            };
        }

        private static Color BytesToColor(byte[] bytes, int offset = 0)
        {
            return new Color(
                bytes[offset + 0] / 256f,
                bytes[offset + 1] / 256f,
                bytes[offset + 2] / 256f,
                bytes[offset + 3] / 256f
            );
        }
    }
}