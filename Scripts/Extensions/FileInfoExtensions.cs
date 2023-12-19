using System.IO;

namespace ElasticSea.Framework.Extensions
{
    public static class FileInfoExtensions
    {
        public static byte[] ToBytes(this FileInfo fileInfo)
        {
            return File.ReadAllBytes(fileInfo.FullName);
        }
    }
}