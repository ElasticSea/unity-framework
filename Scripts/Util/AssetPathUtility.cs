#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor.PackageManager;

namespace ElasticSea.Framework.Util
{
    /// <summary>Resolve Unity virtual package paths before using System.IO.</summary>
    public static class AssetPathUtility
    {
        public static string ToPhysicalPath(string path)
        {
            if (string.IsNullOrEmpty(path) || !path.StartsWith("Packages/", StringComparison.Ordinal))
                return path;
            var package = PackageInfo.FindForAssetPath(path);
            if (package == null)
                throw new ArgumentException("Package is not registered: " + path, nameof(path));
            return Path.Combine(package.resolvedPath, path.Substring(package.assetPath.Length).TrimStart('/'));
        }
    }
}
#endif
