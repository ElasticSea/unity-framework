using System.IO;

namespace ElasticSea.Framework.Scripts.Extensions
{
    public static class PathExtensions
    {
        public static void EnsureDirectory(this FileInfo fileInfo)
        {
            fileInfo.Directory.EnsureDirectory();
        }
        
        public static void EnsureDirectory(this DirectoryInfo dir, bool clear = false)
        {
            if (dir.Exists)
            {
                if (clear)
                {
                    dir.Delete(true);
                    dir.Create();
                }
            }
            else
            {
                dir.Create();
            }
        }
        
        public static FileInfo File(this DirectoryInfo dir, string path)
        {
            return new FileInfo(Path.Combine(dir.FullName, path));
        }
        
        public static void Write(this FileInfo file, byte[] bytes)
        {
            System.IO.File.WriteAllBytes(file.FullName, bytes);
        }
        
        public static void Write(this FileInfo file, string text)
        {
            System.IO.File.WriteAllText(file.FullName, text);
        }
    }
}