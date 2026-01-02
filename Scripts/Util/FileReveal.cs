using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public static class FileReveal
    {
        /// <summary>
        /// Opens the OS file manager.
        /// If 'path' is a file, it opens the folder and selects the file.
        /// If 'path' is a folder, it just opens that folder.
        /// </summary>
        public static void Reveal(string path)
        {
            // 1. Sanitize the path to prevent errors
            if (string.IsNullOrEmpty(path)) return;
        
            // Normalize path to system defaults to avoid confusion
            path = Path.GetFullPath(path);

            // 2. Check if path exists
            bool isFile = File.Exists(path);
            bool isDirectory = Directory.Exists(path);

            if (!isFile && !isDirectory)
            {
                UnityEngine.Debug.LogError($"[FileReveal] Path not found: {path}");
                return;
            }

            // 3. Platform-specific execution
#if UNITY_EDITOR
            // The simple built-in Unity way (Editor only)
            UnityEditor.EditorUtility.RevealInFinder(path);
#elif UNITY_STANDALONE_WIN
            RevealWindows(path, isFile);
#elif UNITY_STANDALONE_OSX
            RevealMac(path);
#elif UNITY_STANDALONE_LINUX
            RevealLinux(path, isFile);
#else
            // Fallback for other platforms (e.g. Android/iOS just try OpenURL)
            RevealFallback(path, isFile);
#endif
        }

        // --- Windows Implementation ---
        private static void RevealWindows(string path, bool isFile)
        {
            // Windows Explorer requires backslashes
            path = path.Replace("/", "\\");

            try
            {
                // If it's a file, we use the /select argument to highlight it
                if (isFile)
                {
                    Process.Start("explorer.exe", $"/select,\"{path}\"");
                }
                else
                {
                    Process.Start("explorer.exe", $"\"{path}\"");
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"[FileReveal] Windows Error: {e.Message}");
            }
        }

        // --- macOS Implementation ---
        private static void RevealMac(string path)
        {
            // macOS 'open' command with -R reveals the file in Finder
            // It handles both files and folders gracefully
            try
            {
                // Ensure strictly valid quotes for shell
                string arguments = $"-R \"{path}\"";
                Process.Start("open", arguments);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"[FileReveal] Mac Error: {e.Message}");
            }
        }

        // --- Linux Implementation ---
        private static void RevealLinux(string path, bool isFile)
        {
            // Linux is fragmented (Nautilus, Dolphin, Thunar, etc.)
            // We try the safest approach: xdg-open.
            // xdg-open generally opens folders well, but DOES NOT support file selection standardly.
        
            try
            {
                // If it's a file, we usually just open the parent folder because 
                // selecting a specific file varies wildly between distros.
                string folderToOpen = isFile ? Path.GetDirectoryName(path) : path;
            
                // "xdg-open" is the standard command to open the default file manager
                Process.Start("xdg-open", $"\"{folderToOpen}\"");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"[FileReveal] Linux Error: {e.Message}");
            }
        }

        // --- Fallback ---
        private static void RevealFallback(string path, bool isFile)
        {
            // Just try to open the folder URL if all else fails
            string folder = isFile ? Path.GetDirectoryName(path) : path;
            Application.OpenURL($"file://{folder}");
        }
    }
}