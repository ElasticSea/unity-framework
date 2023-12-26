using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ElasticSea.Framework.Util.Editor
{
    public class Tools
    {
        [MenuItem("Tools/Find duplicate resources")]
        public static void FixResource()
        {
            var groupedPaths = Directory
                .EnumerateDirectories(Application.dataPath, "Resources", SearchOption.AllDirectories)
                .Select(path => path.ToLowerInvariant())
                .SelectMany(resourceDirPath => Directory.EnumerateFiles(resourceDirPath, "*", SearchOption.AllDirectories)
                    .Select(path => path.ToLowerInvariant())
                    .Where(file => Path.GetExtension(file) != ".meta")
                    .Select(file => (root:resourceDirPath, relative:file.Substring(resourceDirPath.Length + 1)))).GroupBy(tuple => tuple.Item2)
                .ToDictionary(g => g.Key, g => g.ToArray());

            var duplicateResourcePaths = groupedPaths.Where(a => a.Value.Length > 1);

            foreach (var (resourceDirPath, grouped) in duplicateResourcePaths)
            {
                Debug.Log($"{resourceDirPath} {grouped.Length}");
                for (var i = 0; i < grouped.Length; i++)
                {
                    Debug.Log($"{i} {grouped[i].root}");
                }
            }
        }
    }
}