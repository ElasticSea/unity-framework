using System;
using System.IO;
using UnityEngine;

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
            dir.Refresh();
            
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
        
        public static byte[] ReadBytes(this FileInfo file)
        {
            return System.IO.File.ReadAllBytes(file.FullName);
        }

        public static void ForceDelete(this DirectoryInfo dir, bool silent = false)
        {
            try
            {
                if (dir.Exists)
                    dir.Delete(true);
            }
            catch (Exception e)
            {
                if (silent)
                    Debug.LogException(new IOException($"Failed to delete {dir.FullName}", e));
                else
                    throw;
            }
        }
    }
}