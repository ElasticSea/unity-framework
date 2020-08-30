using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Audio;
using _Framework.Scripts.Extensions;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace _Framework.Scripts.Util
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

        public static Color HSV(
            float H, // hue shift (0 - 1)
            float S, // saturation multiplier (0 - 1)
            float V // value multiplier (0 - 1)
        )
        {
            var VSU = V * S * Math.Cos(H * Mathf.PI * 2);
            var VSW = V * S * Math.Sin(H * Mathf.PI * 2);

            return new Color
            {
                r = (float)((.299 * V + .701 * VSU + .168 * VSW)
                            + (.587 * V - .587 * VSU + .330 * VSW)
                            + (.114 * V - .114 * VSU - .497 * VSW)),
                g = (float)((.299 * V - .299 * VSU - .328 * VSW)
                            + (.587 * V + .413 * VSU + .035 * VSW)
                            + (.114 * V - .114 * VSU + .292 * VSW)),
                b = (float)((.299 * V - .3 * VSU + 1.25 * VSW)
                            + (.587 * V - .588 * VSU - 1.05 * VSW)
                            + (.114 * V + .886 * VSU - .203 * VSW)),
                a = 1
            };
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

        [ObsoleteAttribute("This method has been deprecated.")]
        public static string AlternativeFileName(string file, string newFileName)
        {
            var fileInfo = new FileInfo(file);
            
            var existingNames = new DirectoryInfo(fileInfo.DirectoryName)
                .EnumerateFiles()
                .Select(d => d.Name)
                .ToSet();
            
            return Path.Combine(fileInfo.DirectoryName, AlternativeName(existingNames, newFileName));
        }

        public static string AlternativeFileName(string filePath)
        {
            var directory = new FileInfo(filePath).DirectoryName;
            var extension = Path.GetExtension(filePath);
            var fileWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            
            var existingNamesWithoutExtension = new DirectoryInfo(directory)
                .EnumerateFiles()
                .Select(d =>  Path.GetFileNameWithoutExtension(d.Name))
                .ToSet();

            return Path.Combine(directory, AlternativeName(existingNamesWithoutExtension, fileWithoutExtension) + extension);
        }

        public static string AlternativeName(IEnumerable<string> takenNames, string name)
        {
            var nameSet = takenNames.Select(n => n.ToLower()).ToSet();
            return AlternativeName(s => nameSet.Contains(s), name);
        }
        
        public static string AlternativeName(Predicate<string> isTaken, string name)
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
        
        public static string GetRandomHexNumber(int digits)
        {
            var buffer = new byte[digits / 2];
            Rng.NextBytes(buffer);
            var result = string.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result.ToLower();
            return (result + Rng.Next(16).ToString("X")).ToLower();
        }

        public static void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public static bool IsStringValidFileName(this string fileName)
        {
            return fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
        }
    }
}