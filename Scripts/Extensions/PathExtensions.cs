using System.IO;

namespace ElasticSea.Framework.Scripts.Extensions
{
    public static class PathExtensions
    {
        public static void EnsureDirectory(this FileInfo fileInfo)
        {
            fileInfo.Directory.EnsureDirectory();
        }
        
        public static void EnsureDirectory(this DirectoryInfo directoryInfo)
        {
            if (directoryInfo.Exists == false)
            {
                directoryInfo.Create();
            }
        }
    }
}