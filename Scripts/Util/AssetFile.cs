#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ElasticSea.Framework.Util
{
    // System.IO counterparts for tools that accept Unity asset paths. Directory
    // listings retain virtual paths so they can still be passed to AssetDatabase.
    public static class AssetFile
    {
        private static string Resolve(string path) => AssetPathUtility.ToPhysicalPath(path);
        public static bool Exists(string path) => File.Exists(Resolve(path));
        public static byte[] ReadAllBytes(string path) => File.ReadAllBytes(Resolve(path));
        public static string ReadAllText(string path) => File.ReadAllText(Resolve(path));
        public static void WriteAllBytes(string path, byte[] bytes) => File.WriteAllBytes(Resolve(path), bytes);
        public static void WriteAllText(string path, string text) => File.WriteAllText(Resolve(path), text);
        public static void WriteAllText(string path, string text, Encoding encoding) => File.WriteAllText(Resolve(path), text, encoding);
        public static void Copy(string source, string destination, bool overwrite = false) => File.Copy(Resolve(source), Resolve(destination), overwrite);
    }

    public static class AssetDirectory
    {
        private static string Resolve(string path) => AssetPathUtility.ToPhysicalPath(path);
        public static bool Exists(string path) => Directory.Exists(Resolve(path));
        public static DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(Resolve(path));
        public static void Delete(string path, bool recursive = false) => Directory.Delete(Resolve(path), recursive);
        public static string GetCurrentDirectory() => Directory.GetCurrentDirectory();
        public static IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption option = SearchOption.TopDirectoryOnly)
        {
            var physical = Resolve(path);
            return Directory.EnumerateFiles(physical, pattern, option)
                .Select(file => physical == path ? file : path.TrimEnd('/') + file.Substring(physical.TrimEnd('/').Length).Replace('\\', '/'));
        }
        public static string[] GetFiles(string path, string pattern = "*", SearchOption option = SearchOption.TopDirectoryOnly) => EnumerateFiles(path, pattern, option).ToArray();
    }
}
#endif
