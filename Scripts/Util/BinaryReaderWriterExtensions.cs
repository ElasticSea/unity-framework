using System;
using System.IO;

namespace ElasticSea.Framework.Util
{
    public static class BinaryReaderWriterExtensions
    {
        public static void Write(this BinaryWriter writer, Version version)
        {
            writer.Write(version.Major);
            writer.Write(version.Minor);
            writer.Write(version.Build);
        }
        
        public static Version ReadVersion(this BinaryReader reader)
        {
            var major = reader.ReadInt32();
            var minor = reader.ReadInt32();
            var build = reader.ReadInt32();
            return new Version(major, minor, build);
        }
    }
}