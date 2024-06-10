using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Blocks.Meshbakers;
using DG.Tweening.Core.Easing;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Scripts.Extensions;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace ElasticSea.Framework.Util
{
    public static class Utils
    {
        private static readonly Random Rng = new Random();

        public static bool Probability(float value) => Probability(value, Rng);

        public static bool Probability(float value, Random rng) => rng.NextDouble() <= value;

        public static bool RollDice(float sides) => RollDice(sides, Rng);

        public static bool RollDice(float sides, Random rng) => rng.NextDouble() * sides < 1;

        public static IEnumerable<T> GetEnumValues<T>() => Enum.GetValues(typeof(T)).Cast<T>();

        public static void MoveDirectory(string source, string target)
        {
            var sourcePath = source.NormalizePath();
            var targetPath = target.NormalizePath();
            
            var files = Directory
                .EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                .GroupBy(Path.GetDirectoryName);
            
            foreach (var folder in files)
            {
                var targetFolder = folder.Key.Replace(sourcePath, targetPath);
                Directory.CreateDirectory(targetFolder);
                foreach (var file in folder)
                {
                    var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                    if (File.Exists(targetFile)) File.Delete(targetFile);
                    File.Move(file, targetFile);
                }
            }
            Directory.Delete(source, true);
        }

        public static T FromXml<T>(string xml)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml ?? ""));
            var serializer = new XmlSerializer(typeof(T));
            var reader = new StreamReader(stream);
            var deserialized = (T)serializer.Deserialize(reader);
            reader.Close();
            return deserialized;
        }

        public static float InverseLerp(float a, float b, float t)
        {
            return (t - a) / (b - a);
        }

        public static float valueToDb(float value)
        {
            return value != 0f ? 20.0f * Mathf.Log10(value) : -144f;
        }

        public static AudioSource PlayClipAtPoint3D(AudioClip clip, Vector3 pos, float volume = 1f, AudioMixerGroup audioMixerGroup = null, float pitch = 1, bool loop = false, float min = 1, float max = 500, float delay = 0)
        {
            var go = new GameObject("Play One Shot");
            go.transform.position = pos;
            var source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = loop;
            source.pitch = pitch;
            source.volume = volume;
            source.spatialize = true;
            source.spatialBlend = 1;
            source.minDistance = min;
            source.maxDistance = max;
            source.outputAudioMixerGroup = audioMixerGroup;
            
            if (delay == 0)
            {
                source.Play();
            }
            else
            {
                source.PlayDelayed(delay);
            }
            
            if (loop == false)
                Object.Destroy(go, clip.length + delay);
            
            return source;
        }

        public static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 pos, float volume = 1f, AudioMixerGroup audioMixerGroup = null, float pitch = 1, bool loop = false)
        {
            var go = new GameObject("Play One Shot");
            go.transform.position = pos;
            var source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = loop;
            source.pitch = pitch;
            source.volume = volume;
            source.outputAudioMixerGroup = audioMixerGroup;
            source.Play();
            
            if (loop == false)
                Object.Destroy(go, clip.length);
            
            return source;
        }

        public static float Ease(float t, float exponent)
        {
            return Mathf.Pow(Mathf.Abs(t), exponent) / (Mathf.Pow(Mathf.Abs(t), exponent) + Mathf.Pow((1 - Mathf.Abs(t)), exponent)) * Mathf.Sign(t);
        }

        public static DirectoryInfo CreateDir(params string[] pathFragments)
        {
            var userLevelDir = new DirectoryInfo(Path.Combine(pathFragments));
            if (userLevelDir.Exists == false) userLevelDir.Create();
            return userLevelDir;
        }

        public static bool IntersectsCollider(Collider collider, Vector3 from, Vector3 to)
        {
            if (collider == false) return false;
            var layerIndex = collider.gameObject.layer;
            var oneWay = Physics.Linecast(from, to, 1 << layerIndex, QueryTriggerInteraction.Collide);
            var otherWay = Physics.Linecast(to, from, 1 << layerIndex, QueryTriggerInteraction.Collide);
            return oneWay || otherWay;
        }

        public static Texture2D BytesToTexture(byte[] bytes)
        {
            var tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            return tex;
        }

        [Obsolete("This method has been deprecated.")]
        public static string AlternativeFileName(string file, string newFileName)
        {
            var fileInfo = new FileInfo(file);
            
            var existingNames = new DirectoryInfo(fileInfo.DirectoryName)
                .EnumerateFiles()
                .Select(d => d.Name.ToLower())
                .ToSet();
            
            return Path.Combine(fileInfo.DirectoryName, AlternativeNameBraces(existingNames, newFileName));
        }

        public static string AlternativeFileName(string filePath)
        {
            var directory = new FileInfo(filePath).DirectoryName;
            var extension = Path.GetExtension(filePath);
            var fileWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            
            var existingNamesWithoutExtension = new DirectoryInfo(directory)
                .EnumerateFiles()
                .Select(d =>  Path.GetFileNameWithoutExtension(d.Name).ToLower())
                .ToSet();

            return Path.Combine(directory, AlternativeNameBraces(existingNamesWithoutExtension, fileWithoutExtension) + extension);
        }

        public static string AlternativeDirectoryName(string directoryPath)
        {
            var directory = new DirectoryInfo(directoryPath);
            if (directory.Exists == false)
            {
                return directory.FullName;
            }

            var existingNames = directory.Parent
                .EnumerateDirectories()
                .Select(d => d.Name.ToLower())
                .ToSet();

            var alternativeDirectoryName = AlternativeNameBraces(existingNames, directory.Name);
            return Path.Combine(directory.Parent.FullName, alternativeDirectoryName);
        }

        public static string AlternativeNameBraces(IEnumerable<string> takenNames, string name)
        {
            var nameSet = takenNames.Select(n => n.ToLower()).ToSet();
            return AlternativeNameBraces(s => nameSet.Contains(s), name);
        }
        
        public static string AlternativeNameBraces(Predicate<string> isTaken, string name)
        {
            while (isTaken(name.ToLower()))
            {
                var firstBrace = name.LastIndexOf("(", StringComparison.Ordinal);
                var lastBrace = name.LastIndexOf(")", StringComparison.Ordinal);

                if (firstBrace != -1 && lastBrace != -1 && firstBrace < lastBrace)
                {
                    var insideBraces = name.Substring(firstBrace + 1, lastBrace-firstBrace- 1);
                    var num = insideBraces.ToInt(0);
                    if (num >= 1)
                    {
                        name = name.Substring(0, firstBrace) + "(" + (num+1) + ")";
                        continue;
                    }
                }

                name = name + " (" + 1 + ")";
            }

            return name;
        }

        public static string AlternativeNameNoBrackets(IEnumerable<string> takenNames, string name)
        {
            var nameSet = takenNames.ToSet();
            return AlternativeNameNoBrackets(s => nameSet.Contains(s, StringComparer.InvariantCultureIgnoreCase), name);
        }
        
        public static string AlternativeNameNoBrackets(Predicate<string> isTaken, string name)
        {
            while (isTaken(name))
            {
                var replace = false;
                var lastIndex = -1;
                var firstIndex = -1;
                for (var i = name.Length - 1; i >= 0; i--)
                {
                    var ch = name[i];
                    var isDigit = char.IsDigit(ch);
                    if (lastIndex == -1)
                    {
                        if (isDigit)
                        {
                            lastIndex = i + 1;
                        }
                    }
                    else
                    {
                        if (firstIndex == -1)
                        {
                            if (isDigit == false)
                            {
                                firstIndex = i + 1;
                                if (char.IsWhiteSpace(ch))
                                {
                                    replace = true;
                                }
                                break;
                            }
                        }
                    }
                }

                if (replace)
                {
                    var prefix = name.Substring(0, firstIndex);
                    var number = name.Substring(firstIndex, lastIndex - firstIndex);
                    var suffix = name.Substring(lastIndex, name.Length - lastIndex);

                    name = prefix + (number.ToInt() + 1) + suffix;
                }
                else
                {
                    name = $"{name} 1";
                }
            }

            return name;
        }
        
        public static void CopyDirectory(string sourceDirectory, string targetDirectory, Predicate<FileSystemInfo> filter = null)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyDirectory(diSource, diTarget, filter);
        }

        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo target, Predicate<FileSystemInfo> filter = null)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (var fi in source.GetFiles().Where(f => filter == null || filter(f)))
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories().Where(f => filter == null || filter(f)))
            {
                var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectory(diSourceSubDir, nextTargetSubDir, filter);
            }
        }
        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fromPath"/> or <paramref name="toPath"/> is <c>null</c>.</exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static string GetRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath))
            {
                throw new ArgumentNullException("fromPath");
            }

            if (string.IsNullOrEmpty(toPath))
            {
                throw new ArgumentNullException("toPath");
            }

            Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
            Uri toUri = new Uri(AppendDirectorySeparatorChar(toPath));

            if (fromUri.Scheme != toUri.Scheme)
            {
                return toPath;
            }

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        private static string AppendDirectorySeparatorChar(string path)
        {
            // Append a slash only if the path is a directory and does not have a slash.
            if (!Path.HasExtension(path) &&
                !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                return path + Path.DirectorySeparatorChar;
            }

            return path;
        }
        
        public static Task<DateTime> GetNetworkTime()
        {
            return Task.Run(() =>
            {
                //default Windows time server
                const string ntpServer = "time.windows.com";

                // NTP message size - 16 bytes of the digest (RFC 2030)
                var ntpData = new byte[48];

                //Setting the Leap Indicator, Version Number and Mode values
                ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

                var addresses = Dns.GetHostEntry(ntpServer).AddressList;

                //The UDP port number assigned to NTP is 123
                var ipEndPoint = new IPEndPoint(addresses[0], 123);
                //NTP uses UDP

                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    socket.Connect(ipEndPoint);

                    //Stops code hang if NTP is blocked
                    socket.ReceiveTimeout = 1000;

                    socket.Send(ntpData);
                    socket.Receive(ntpData);
                }

                //Offset to get to the "Transmit Timestamp" field (time at which the reply 
                //departed the server for the client, in 64-bit timestamp format."
                const byte serverReplyTime = 40;

                //Get the seconds part
                ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

                //Get the seconds fraction
                ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

                //Convert From big-endian to little-endian
                intPart = SwapEndianness(intPart);
                fractPart = SwapEndianness(fractPart);

                var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

                //**UTC** time
                return new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((long) milliseconds);
            });
        }

        // stackoverflow.com/a/3294698/162671
        static uint SwapEndianness(ulong x)
        {
            return (uint) (((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

        public static int[] FlipIndices(int[] indices)
        {
            var newIndices = new int[indices.Length];
            for (var i = 0; i < indices.Length; i += 3)
            {
                newIndices[i + 0] = indices[i + 0];
                newIndices[i + 1] = indices[i + 2];
                newIndices[i + 2] =  indices[i + 1];
            }

            return newIndices;
        }

        public static Vector3[] FlipNormals(Vector3[] normals)
        {
            var newNormals = new Vector3[normals.Length];
            for (var i = 0; i < normals.Length; i++)
            {
                newNormals[i] = normals[i].Flip();
            }

            return newNormals;
        }

        public static Color RandomColor()
        {
            return new Color(
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                1f
            );
        }
        
        public static int GetUniqueInt(ISet<int> taken)
        {
            int candidate;
            do
            {
                candidate = Rng.Next();
            } while (taken.Contains(candidate));

            return candidate;
        }
        
        public static string GetUniqueHexNumber(ISet<string> taken, int digits)
        {
            string candidate;
            do
            {
                candidate = GetRandomHexNumber(digits);
            } while (taken.Contains(candidate));

            return candidate;
        }
        
        public static string GetRandomHexNumber(int digits)
        {
            return GetRandomHexNumber(Rng, digits);
        }

        public static string GetRandomHexNumber(Random rng, int digits)
        {
            var bytes = new byte[digits / 2];
            rng.NextBytes(bytes);

            var buffer = new char[bytes.Length * 2];

            int startingIndex = 0;
            var bytesLength = bytes.Length;
            for (int index = 0; index < bytesLength; ++index)
            {
                ToCharsBuffer(bytes[index], buffer, startingIndex, 8224);
                startingIndex += 2;
            }

            return new string(buffer);
        }
        
        private static void ToCharsBuffer(
            byte value,
            char[] buffer,
            int startingIndex = 0,
            int casing = 0)
        {
            uint num1 = (uint) ((((int) value & 240) << 4) + ((int) value & 15) - 35209);
            uint num2 = (uint) ((int) ((int) ((uint) (-(int) num1 & 28784) >> 4) + (int) num1 + 47545) | casing);
            buffer[startingIndex + 1] = (char) (num2 & (uint) byte.MaxValue);
            buffer[startingIndex] = (char) (num2 >> 8);
        }

        public static void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public static bool IsStringValidFileName(this string fileName)
        {
            return fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
        }

        public static void OpenInExplorer(string path)
        {
            Application.OpenURL($"file://{new DirectoryInfo(path).FullName}");
        }

        public static Texture2D CreateColorWheelTexture(int width, int height, Color? outsideColor = null, TextureFormat textureFormat = TextureFormat.RGBA32, int mipCount = -1, bool linear = false)
        {
            var texture = new Texture2D(width, height, textureFormat, mipCount, linear);
            
            var center = new Vector2(width / 2f, height / 2f);

            var outside = outsideColor ?? Color.black;
            
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var ang = Mathf.Atan2(center.y - y, center.x - x);
                    var angNormalized = (ang / (Mathf.PI * 2) + 1) % 1;

                    var normalizedX = (x - center.x) / width * 2;
                    var normalizedY = (y - center.y) / height * 2;
                    var distNormalized = new Vector2(normalizedX, normalizedY).magnitude;
                    
                    var wheelColor = Color.HSVToRGB(angNormalized , distNormalized, 1);
                    var pixelColor = distNormalized < 1 ? wheelColor : outside;
                    texture.SetPixel(x, y, pixelColor);
                }
            }
            
            texture.Apply();

            return texture;
        }
        
        public static void EnsureDirectory(string path, bool clear = false)
        {
            if (Directory.Exists(path))
            {
                if (clear)
                {
                    Directory.Delete(path, true);
                    Directory.CreateDirectory(path);
                }
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }
        
        public static Vector3[] PointsOnSphere(int count)
        {
            Vector3[] upts = new Vector3[count];
            float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
            float off = 2.0f / count;
            float x = 0;
            float y = 0;
            float z = 0;
            float r = 0;
            float phi = 0;
       
            for (var k = 0; k < count; k++){
                y = k * off - 1 + (off /2);
                r = Mathf.Sqrt(1 - y * y);
                phi = k * inc;
                x = Mathf.Cos(phi) * r;
                z = Mathf.Sin(phi) * r;
           
                upts[k] = new Vector3(x, y, z);
            }
            
            return upts;
        }
        
        public static Vector3[] PointsOnSphereSegment(int count, float angle, Vector3 direction)
        {
            var rotation = Quaternion.FromToRotation(Vector3.up, direction);
            
            var cnt = count;
            var ll = (1-Mathf.Sin((90-angle) * Mathf.Deg2Rad)) / 2;

            count = (int) (count / ll);
            Vector3[] upts = new Vector3[cnt];
            float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
            float off = 2.0f / count;
            float x = 0;
            float y = 0;
            float z = 0;
            float r = 0;
            float phi = 0;
       
            for (var k = 0; k < cnt; k++){
                y = k * off - 1 + (off /2);
                r = Mathf.Sqrt(1 - y * y);
                phi = k * inc;
                x = Mathf.Cos(phi) * r;
                z = Mathf.Sin(phi) * r;
                
                upts[k] = rotation * new Vector3(x, -y, z);
            }
            
            return upts;
        }
        
        public static string ToRoman(int number)
        {
            var romanNumerals = new[]
            {
                new[] {"", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"}, // ones
                new[] {"", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC"}, // tens
                new[] {"", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM"}, // hundreds
                new[] {"", "M", "MM", "MMM"} // thousands
            };

            // split integer string into array and reverse array
            var intArr = number.ToString().Reverse().ToArray();
            var len = intArr.Length;
            var romanNumeral = "";
            var i = len;

            // starting with the highest place (for 3046, it would be the thousands
            // place, or 3), get the roman numeral representation for that place
            // and add it to the final roman numeral string
            while (i-- > 0)
            {
                romanNumeral += romanNumerals[i][int.Parse(intArr[i].ToString())];
            }

            return romanNumeral;
        }

        public static string BytesToString(long bytes)
        {
            double partial = bytes;
            if (partial < 1024) return $"{partial:G3}B";
            partial /= 1024;
            if (partial < 1024) return $"{partial:G3}KB";
            partial /= 1024;
            if (partial < 1024) return $"{partial:G3}MB";
            partial /= 1024;
            if (partial < 1024) return $"{partial:G3}GB";
            partial /= 1024;
            if (partial < 1024) return $"{partial:G3}TB";
            partial /= 1024;
            if (partial < 1024) return $"{partial:G3}PB";
            partial /= 1024;
            if (partial < 1024) return $"{partial:G3}EB";
            partial /= 1024;
            if (partial < 1024) return $"{partial:G3}ZB";
            partial /= 1024;
            return $"{partial:G3}YB";
        }
        
        public static bool IsPrefabModeIsActive()
        {
#if UNITY_EDITOR
#if UNITY_2021_1_OR_NEWER
            return UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
#else
            return UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
#endif
#endif
            return false;
        }
        
        public static IEnumerable<T> Find<T>()
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);

                foreach (var root in scene.GetRootGameObjects())
                {
                    foreach (var childInterface in root.GetComponentsInChildren<T>())
                    {
                        yield return childInterface;
                    }
                }
            }
        }

        public static Vector3 AverageRotation(Vector3[] rotations)
        {
            float Clamp(float previous, float current)
            {
                if (previous - current > 180)
                {
                    return current + 360;
                }
            
                if (previous - current < -180)
                {
                    return current - 360;
                }

                return current;
            }
            
            var initial = rotations[0];
            var last = initial;
            for (var i = 1; i < rotations.Length; i++)
            {
                var rr = rotations[i];

                var x = Clamp(last.x, rr.x);
                var y = Clamp(last.y, rr.y);
                var z = Clamp(last.z, rr.z);
                var newRot = new Vector3(x, y, z);
                initial +=  newRot;
                last = newRot;
            }
            initial /= rotations.Length;

            return initial;
        }

        public static (bool found, string[] values) CommandLineArgValues(string key)
        {
            return CommandLineArgValues(System.Environment.GetCommandLineArgs(), key);
        }

        // format: application.exe -key1 arg1 arg2 -key2 arg1
        public static (bool found, string[] values) CommandLineArgValues(string[] args, string key)
        {
            for (var i = 1; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.Equals("-" + key, StringComparison.InvariantCultureIgnoreCase))
                {
                    var listValues = new List<string>();
                    for (var j = i + 1; j < args.Length; j++)
                    {
                        var value = args[j];
                        if (value.StartsWith("-") == false)
                        {
                            listValues.Add(value);
                        }
                        else
                        {
                            break;
                        }
                    }

                    return (true, listValues.ToArray());
                }
            }

            return (false, new string[0]);
        }
        
        public static Vector3 GetBezierCurve(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
        {
            t = Mathf.Clamp01(t);

            var part1 = Mathf.Pow(1 - t, 3) * p1;
            var part2 = 3 * Mathf.Pow(1 - t, 2) * t * p2;
            var part3 = 3 * (1 - t) * Mathf.Pow(t, 2) * p3;
            var part4 = Mathf.Pow(t, 3) * p4;

            return part1 + part2 + part3 + part4;
        }

        public static object CallPrivateMethod(object instance, string methodName, params object[] parameters)
        {
            return instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(instance, parameters);
        }

        public static float Remap(float sourceStart, float sourceEnd, float destinationStart, float destinationEnd, float sourceValue)
        {
            var t = Mathf.InverseLerp(sourceStart, sourceEnd, sourceValue);
            return Mathf.Lerp(destinationStart, destinationEnd, t);
        }
        
        // public static async Task<byte[]> HttpDownloadFileAsync(string url) {
        //     using var httpClient = new HttpClient();
        //     using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        //     using var streamToReadFrom = await response.Content.ReadAsStreamAsync(); 
        //     return streamToReadFrom.ReadAllBytes();
        // }
        
        public static async Task<byte[]> HttpDownloadFileAsync(string url) {
            
            var myReq = WebRequest.Create(url);
            myReq.Proxy = null;
            var responseAsync = await myReq.GetResponseAsync();
            return await responseAsync.GetResponseStream().ReadAllBytesAsync();
        }
        
        public static (byte[] bytes, int width, int height) NearestNeighbourScaleDown(byte[] bytes, int bytesPerPixel, int width, int height, int skip)
        {
            var newWidth = width / (1 + skip);
            var newHeight = height / (1 + skip);

            var output = new byte[newWidth * newHeight * bytesPerPixel];

            for (var y = 0; y < newHeight; y++)
            {
                for (var x = 0; x < newWidth; x++)
                {
                    var prevX = (x * (1 + skip));
                    var prevY = (y * (1 + skip));
                    var prevPos = (prevX + prevY * width) * bytesPerPixel;
                    var newPos = (x + y * newWidth) * bytesPerPixel;

                    for (var k = 0; k < bytesPerPixel; k++)
                    {
                        output[newPos + k] = bytes[prevPos + k];
                    }
                }
            }

            return (output, newWidth, newHeight);
        }
        
        public static (byte[] bytes, int width, int height) NearestNeighbourScaleDownMT(byte[] bytes, int bytesPerPixel, int width, int height, int skip)
        {
            var newWidth = width / (1 + skip);
            var newHeight = height / (1 + skip);

            var output = new byte[newWidth * newHeight * bytesPerPixel];

            var chunks = System.Environment.ProcessorCount * 4;
            var jobs =
                Enumerable.Range(0, chunks).Select(i =>
                {
                    var step = Mathf.CeilToInt(newHeight / (float) chunks);
                    return (i * step, Mathf.Min((i + 1) * step, newHeight));
                });

            Parallel.ForEach(jobs, tuple =>
            {
                var start = tuple.Item1;
                var end = tuple.Item2;

                for (var y = start; y < end; y++)
                {
                    for (var x = 0; x < newWidth; x++)
                    {
                        var prevX = (x * (1 + skip));
                        var prevY = (y * (1 + skip));
                        var prevPos = (prevX + prevY * width) * bytesPerPixel;
                        var newPos = (x + y * newWidth) * bytesPerPixel;

                        for (var k = 0; k < bytesPerPixel; k++)
                        {
                            output[newPos + k] = bytes[prevPos + k];
                        }
                    }
                }
            });
            
            return (output, newWidth, newHeight);
        }

        public static Texture2D ResizeActual(this Texture2D texture2D, int width, int height)
        {
            var rt = new RenderTexture(width, height, 24);
            var prev = RenderTexture.active;
            RenderTexture.active = rt;
            Graphics.Blit(texture2D, rt);
            var result = new Texture2D(width, height);
            result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            result.Apply();
            RenderTexture.active = prev;
            Object.Destroy(rt);
            return result;
        }

        public static void CatchException(string message, Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                LogException(message, e);
            }
        }

        public static void LogException(string message, Exception e)
        {
            Debug.LogException(new Exception(message, e));
        }

        public static Bounds Bounds(Vector3 min, Vector3 max)
        {
            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }
        
        public static float DistanceToBounds(Bounds bounds, Vector3 point)
        {
            var min = bounds.min;
            var max = bounds.max;
            var dx = Mathf.Max(min.x - point.x, 0, point.x - max.x);
            var dy = Mathf.Max(min.y - point.y, 0, point.y - max.y);
            var dz = Mathf.Max(min.z - point.z, 0, point.z - max.z);
            return Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        
        public static float DistanceToBounds(FastBounds bounds, Vector3 point)
        {
            var min = bounds.Min;
            var max = bounds.Max;
            var dx = Mathf.Max(Mathf.Max(min.x - point.x, point.x - max.x), 0);
            var dy = Mathf.Max(Mathf.Max(min.y - point.y, point.y - max.y), 0);
            var dz = Mathf.Max(Mathf.Max(min.z - point.z, point.z - max.z), 0);
            return Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public static void WriteAllText(string path, string contents)
        {
            new FileInfo(path).EnsureDirectory();
            File.WriteAllText(path, contents);
        }

        public static void WriteAllBytes(string path, byte[] contents)
        {
            new FileInfo(path).EnsureDirectory();
            File.WriteAllBytes(path, contents);
        }

        public static void Time(string message, Action inner)
        {
            Debug.Log("Begin " + message);
            var sw = Stopwatch.StartNew();
            inner();
            Time(message, sw);
        }

        public static async Task Time(string message, Task inner)
        {
            Debug.Log("Begin " + message);
            var sw = Stopwatch.StartNew();
            await inner;
            Time(message, sw);
        }

        public static async Task<T> Time<T>(string message, Task<T> inner)
        {
            Debug.Log("Begin " + message);
            var sw = Stopwatch.StartNew();
            var result = await inner;
            Time(message, sw);
            return result;
        }

        public static void Time(string message, Stopwatch sw)
        {
            Debug.Log($"{message} took {sw.Elapsed}");
        }

        public static byte[] ReadAllBytes(string filePath)
        {
            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch (IOException e)
            {
                Debug.LogException(e);
                
                // In case another program already has write access to this file

                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return fs.ReadAllBytes();
                }
            }
        }

        public static PageElement<T>[][] CalculatePages<T>((T value, float size)[] elements, float maxSize, float leftNavigationSize, float rightNavigationSize)
        {
            // For 3 elements and less its always best to just make it one page
            if (elements.Length <= 3)
            {
                return new[]
                {
                    elements.Select(e =>
                    {
                        return new PageElement<T>
                        {
                            Size = e.size,
                            Element = e.value,
                            Type = ElementType.Element
                        };
                    }).ToArray()
                };
            }

            var pages = new List<List<PageElement<T>>>();
            var currentPage = new List<PageElement<T>>();
            pages.Add(currentPage);

            var buffer = maxSize;

            for (var i = 0; i < elements.Length; i++)
            {
                var element = elements[i];

                // First element of non first page is always left navigation
                if (pages.Count > 1 && currentPage.Count == 0)
                {
                    currentPage.Add(new PageElement<T>
                    {
                        Size = leftNavigationSize,
                        Type = ElementType.LeftNav
                    });
                    buffer -= leftNavigationSize;

                    i--;
                    // Skip this iteration
                    continue;
                }

                // Element and nav size does not fit on the page, create new page and continue with that one
                var bufferTooSmall = buffer < element.size + rightNavigationSize;
                var notEmpty = currentPage.Count >= 2;
                var isNotLast = i < elements.Length - 1;
                var biggerThanNav = element.size > rightNavigationSize;
                if (bufferTooSmall && (isNotLast || biggerThanNav) && notEmpty)
                {
                    currentPage.Add(new PageElement<T>
                    {
                        Size = rightNavigationSize,
                        Type = ElementType.RightNav
                    });
                    currentPage = new List<PageElement<T>>();
                    pages.Add(currentPage);
                    buffer = maxSize;

                    i--;
                    // Skip this iteration
                    continue;
                }

                currentPage.Add(new PageElement<T>
                {
                    Size = element.size,
                    Element = element.value,
                    Type = ElementType.Element
                });
                buffer -= element.size;
            }

            return pages.Select(p => p.ToArray()).ToArray();
        }

        public static float Ease(float t, Easing easing, float overshootOrAmplitude = 1)
        {
            var duration = 1f;
            switch (easing)
            {
                case Easing.Linear:
                    return t;
                case Easing.InSine:
                    return (-Mathf.Cos(t * 1.5707963705062866f) + 1f);
                case Easing.OutSine:
                    return Mathf.Sin(t * 1.5707963705062866f);
                case Easing.InOutSine:
                    return (-0.5f * (Mathf.Cos(3.1415927410125732f * t) - 1f));
                case Easing.InQuad:
                    return t * t;
                case Easing.OutQuad:
                    return (-(t) * (t - 2f));
                case Easing.InOutQuad:
                    return (t /= 0.5f) < 1.0f ? 0.5f * t * t : -0.5f * (--t * (t - 2.0f) - 1.0f);
                case Easing.InCubic:
                    return t * t * t;
                case Easing.OutCubic:
                    return ((t = (t - 1f)) * t * t + 1f);
                case Easing.InOutCubic:
                    return (t * 0.5f) < 1f ? 0.5f * t * t * t : (0.5f * ((t -= 2f) * t * t + 2f));
                case Easing.InQuart:
                    return t * t * t * t;
                case Easing.OutQuart:
                    return -((t = (t - 1f)) * t * t * t - 1f);
                case Easing.InOutQuart:
                    return (t * 0.5f) < 1f ? 0.5f * t * t * t * t : (-0.5f * ((t -= 2f) * t * t * t - 2f));
                case Easing.InQuint:
                    return t * t * t * t * t;
                case Easing.OutQuint:
                    return ((t = (t - 1f)) * t * t * t * t + 1f);
                case Easing.InOutQuint:
                    return (t * 0.5f) < 1f ? 0.5f * t * t * t * t * t : (0.5f * ((t -= 2f) * t * t * t * t + 2f));
                case Easing.InExpo:
                    return t != 0f ? Mathf.Pow(2f, 10f * (t - 1f)) : 0.0f;
                case Easing.OutExpo:
                    return t == duration ? 1f : (-Mathf.Pow(2f, -10f * t) + 1f);
                case Easing.InOutExpo:
                    if (t == 0.0)
                        return 0.0f;
                    if (t == duration)
                        return 1f;
                    return (t * 0.5f) < 1f ? 0.5f * Mathf.Pow(2f, 10f * (t - 1f)) : (0.5f * (-Mathf.Pow(2f, -10f * --t) + 2f));
                case Easing.InCirc:
                    return -(Mathf.Sqrt(1f - (t) * t) - 1f);
                case Easing.OutCirc:
                    return Mathf.Sqrt(1f - (t = (t - 1f)) * t);
                case Easing.InOutCirc:
                    return (t * 0.5f) < 1f ? (-0.5f * (Mathf.Sqrt(1f - t * t) - 1f)) : (0.5f * (Mathf.Sqrt(1f - (t -= 2f) * t) + 1f));
                // case Easing.InElastic:
                //   if ( t == 0.0)
                //     return 0.0f;
                //   if ( (t) == 1f)
                //     return 1f;
                //   if ( period == 0.0)
                //     period = duration * 0.3f;
                //   float num1;
                //   if ( overshootOrAmplitude < 1f)
                //   {
                //     overshootOrAmplitude = 1f;
                //     num1 = period / 4f;
                //   }
                //   else
                //     num1 = period / 6.2831855f *  Math.Asin(1f /  overshootOrAmplitude);
                //   return  -( overshootOrAmplitude * Math.Pow(2f, 10.0 *  --t) * Math.Sin(( t *  duration -  num1) * 6.2831854820251465 ));
                // case Easing.OutElastic:
                //   if ( t == 0.0)
                //     return 0.0f;
                //   if ( (t) == 1f)
                //     return 1f;
                //   if ( period == 0.0)
                //     period = duration * 0.3f;
                //   float num2;
                //   if ( overshootOrAmplitude < 1f)
                //   {
                //     overshootOrAmplitude = 1f;
                //     num2 = period / 4f;
                //   }
                //   else
                //     num2 = period / 6.2831855f *  Math.Asin(1f /  overshootOrAmplitude);
                //   return  ( overshootOrAmplitude * Math.Pow(2f, -10.0 *  t) * Math.Sin(( t *  duration -  num2) * 6.2831854820251465 ) + 1f);
                // case Easing.InOutElastic:
                //   if ( t == 0.0)
                //     return 0.0f;
                //   if ( (t * 0.5f) == 2f)
                //     return 1f;
                //   if ( period == 0.0)
                //     period = duration * 0.45000002f;
                //   float num3;
                //   if ( overshootOrAmplitude < 1f)
                //   {
                //     overshootOrAmplitude = 1f;
                //     num3 = period / 4f;
                //   }
                //   else
                //     num3 = period / 6.2831855f *  Math.Asin(1f /  overshootOrAmplitude);
                //   return  t < 1f ?  (-0.5 * ( overshootOrAmplitude * Math.Pow(2f, 10.0 *  --t) * Math.Sin(( t *  duration -  num3) * 6.2831854820251465))) :  ( overshootOrAmplitude * Math.Pow(2f, -10.0 *  --t) * Math.Sin(( t *  duration -  num3) * 6.2831854820251465 /  period) * 0.5 + 1f);
                case Easing.InBack:
                    return (t) * t * ((overshootOrAmplitude + 1f) * t - overshootOrAmplitude);
                case Easing.OutBack:
                    return (t = (t - 1f)) * t * ((overshootOrAmplitude + 1f) * t + overshootOrAmplitude) + 1f;
                case Easing.InOutBack:
                    return t * 0.5f < 1f
                        ? 0.5f * (t * t * (((overshootOrAmplitude *= 1.525f) + 1f) * t - overshootOrAmplitude))
                        : 0.5f * ((t -= 2f) * t * (((overshootOrAmplitude *= 1.525f) + 1f) * t + overshootOrAmplitude) + 2f);
                case Easing.InBounce:
                  return Bounce.EaseIn(t, duration, 0, 0);
                case Easing.OutBounce:
                  return Bounce.EaseOut(t, duration, 0, 0);
                case Easing.InOutBounce:
                  return Bounce.EaseInOut(t, duration, 0, 0);
                // case Easing.Flash:
                //   return Flash.Easing(t, duration, overshootOrAmplitude, period);
                // case Easing.InFlash:
                //   return Flash.EasingIn(t, duration, overshootOrAmplitude, period);
                // case Easing.OutFlash:
                //   return Flash.EasingOut(t, duration, overshootOrAmplitude, period);
                // case Easing.InOutFlash:
                //   return Flash.EasingInOut(t, duration, overshootOrAmplitude, period);
                default:
                    throw new Exception($"Easing [{easing}] not suported");
            }
        }
        
        public static float[] CreateAnimationTimeSlots(float t, int count)
        {
            var slots = new float[count];
            var segment = 1f / slots.Length;
            for (var i = 0; i < slots.Length; i++)
            {
                var start = (i + 0) * segment;
                var end = (i + 1) * segment;
                slots[i] = Mathf.Clamp01(Mathf.InverseLerp(start, end, t));
            }

            return slots;
        }
        
        public static (float first, float second) SplitTime(float t, float split)
        {
            var first = Mathf.Clamp01(Mathf.InverseLerp(0, split, t));
            var second = Mathf.Clamp01(Mathf.InverseLerp(split, 1, t));
            return (first, second);
        }

        public static Vector3 CircleXZ(float delta)
        {
            var circle = Mathf.PI * 2;
            var angle = delta * circle;
            var x = Mathf.Cos(angle);
            var z = MathF.Sin(angle);
            return new Vector3(x, 0, z);
        }

        public static (Rect rect, Vector2Int dimensions) GetGridDimensions(Rect availableRect, Vector2 cellSize, float spacing, int cellWidthLimit = -1, int cellHeightLimit = -1, int cellWidthMin = -1, int cellHeightMin = -1)
        {
            var rectSize = availableRect.size;
            var (occupiedRect, dimensions) = GetGridDimensions(rectSize, cellSize, spacing, cellWidthLimit, cellHeightLimit, cellWidthMin, cellHeightMin);
            occupiedRect.position += availableRect.min;
            return (occupiedRect, dimensions);
        }

        public static (Rect rect, Vector2Int dimensions) GetGridDimensions(Vector2 gridSize, Vector2 cellSize, float spacing, int cellWidthLimit = -1, int cellHeightLimit = -1, int cellWidthMin = -1, int cellHeightMin = -1)
        {
            return GetGridDimensions(gridSize.x, gridSize.y, cellSize.x, cellSize.y, spacing, spacing, cellWidthLimit, cellHeightLimit, cellWidthMin, cellHeightMin);
        }

        public static (Rect rect, Vector2Int dimensions) GetGridDimensions(Vector2 gridSize, Vector2 cellSize, Vector2 spacing, int cellWidthLimit = -1, int cellHeightLimit = -1, int cellWidthMin = -1, int cellHeightMin = -1)
        {
            return GetGridDimensions(gridSize.x, gridSize.y, cellSize.x, cellSize.y, spacing.x, spacing.y, cellWidthLimit, cellHeightLimit, cellWidthMin, cellHeightMin);
        }

        public static (Rect rect, Vector2Int dimensions) GetGridDimensions(float gridWidth, float gridHeight, float cellWidth, float cellHeight, float xSpacing, float ySpacing, int cellWidthLimit = -1, int cellHeightLimit = -1, int cellWidthMin = -1, int cellHeightMin = -1)
        {
            var xCellCount = Mathf.FloorToInt((gridWidth - xSpacing) / (cellWidth + xSpacing));
            var yCellCount = Mathf.FloorToInt((gridHeight - ySpacing) / (cellHeight + ySpacing));

            if (cellWidthLimit != -1) xCellCount = Mathf.Min(xCellCount, cellWidthLimit);
            if (cellHeightLimit != -1) yCellCount = Mathf.Min(yCellCount, cellHeightLimit);

            if (cellWidthMin != -1) xCellCount = Mathf.Max(xCellCount, cellWidthMin);
            if (cellHeightMin != -1) yCellCount = Mathf.Max(yCellCount, cellHeightMin);

            var finalWidth = (xCellCount - 1) * xSpacing + xCellCount * cellWidth;
            var finalHeight = (yCellCount - 1) * ySpacing + yCellCount * cellHeight;

            var dimensions = new Vector2Int(xCellCount, yCellCount);
            var offset = new Vector2((gridWidth - finalWidth) / 2, (gridHeight - finalHeight) / 2);

            var rect = Rect.MinMaxRect(offset.x, offset.y, offset.x + finalWidth, offset.y + finalHeight);
            return (rect, dimensions);
        }
        
        public static Collider[] OverlapBox(Transform transform, Bounds bounds, Vector3? offset = null, int layermask = -1)
        {
            var off = offset ?? Vector3.zero;
            var worldCenter = transform.TransformPoint(bounds.center);
            var worldExtents = transform.lossyScale.Multiply(bounds.size) / 2 + off;
            var orientation = transform.rotation;

            return Physics.OverlapBox(worldCenter, worldExtents, orientation, layermask);
        }
        
        public static int OverlapBoxNonAlloc(Transform transform, Bounds bounds, Collider[] results, int layermask = -1, float offset = 0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            var worldCenter = transform.TransformPoint(bounds.center);
            var worldExtents = transform.lossyScale.Multiply(bounds.size) / 2 + new Vector3(offset, offset, offset);
            var orientation = transform.rotation;

            return Physics.OverlapBoxNonAlloc(worldCenter, worldExtents, results, orientation, layermask, queryTriggerInteraction);
        }

        public static T[] Grow<T>(this T[] array, int length)
        {
            if (length > array.Length)
            {
                var newArray = new T[length];
                Array.Copy(array, newArray, array.Length);
                return newArray;
            }

            return array;
        }
    }
    
    public struct PageElement<T>
    {
        public float Size;
        public ElementType Type;
        public T Element;
    }
    
    public enum ElementType
    {
        LeftNav, Element, RightNav
    }
}