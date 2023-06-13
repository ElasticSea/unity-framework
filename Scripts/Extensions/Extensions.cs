using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class Extensions
    {
        public static string GetBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
        
        public static string GetUtf8String(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        
        public static byte[] GetUtf8Bytes(this string utf8String)
        {
            return Encoding.UTF8.GetBytes(utf8String);
        }
        
        public static void Write(this FileStream fs, byte[] bytes)
        {
            fs.Write(bytes, 0, bytes.Length);
        }

        public static byte[] GetBytes(this string str)
        {
            return Convert.FromBase64String(str);
        }
        public static T CheckReference<T>(this T component) where T : UnityEngine.Object
        {
            if (component)
            {
                return component;
            }
            return default(T);
        }

        public static float? ToNullableFloat(this string s, float? def = (float?)null)
        {
            return float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var i) ? i : def;
        }

        public static bool? ToNullableBool(this string s, bool? def = (bool?)null)
        {
            return bool.TryParse(s, out var i) ? i : def;
        }

        public static int? ToNullableInt(this string s, int? def = (int?)null)
        {
            return int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var i) ? i : def;
        }

        public static float ToFloat(this string s, float def = 0)
        {
            return float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var i) ? i : def;
        }

        public static bool ToBool(this string s, bool def = false)
        {
            return bool.TryParse(s, out var i) ? i : def;
        }

        public static int ToInt(this string s, int def = 0)
        {
            return int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var i) ? i : def;
        }

        public static T? ToEnum<T>(this string s, T? def) where T : struct
        {
            return Enum.TryParse<T>(s, true, out var i) ? i : def;
        }

        public static Color SetAlpha(this Color color, float value)
        {
            return new Color(color.r, color.g, color.b, value);
        }

        public static void SetAlpha(this Material material, float value)
        {
            material.color = new Color(material.color.r, material.color.g, material.color.b, value);
        }

        public static Color ToColor(this int rgb)
        {
            return ((uint)(rgb << 8) | 0xff).ToColor();
        }

        public static Color ToColor(this uint rgba)
        {
            return new Color32(
                (byte)((rgba & 0xff000000) >> 24),
                (byte)((rgba & 0xff0000) >> 16),
                (byte)((rgba & 0xff00) >> 8),
                (byte)((rgba & 0xff) >> 0)
            );
        }

        public static uint ToUint(this Color color)
        {
            return (uint)(
                ((byte)(color.a * 255) << 24) |
                ((byte)(color.b * 255) << 16) |
                ((byte)(color.g * 255) << 8) |
                ((byte)(color.r * 255) << 0)
            );
        }

        public static int ToInt(this Color color)
        {
            return ((byte)(color.b * 255) << 16) |
                   ((byte)(color.g * 255) << 8) |
                   ((byte)(color.r * 255) << 0);
        }

        public static string colorToHex(this Color color) =>
            "0x" +
            ((int)(color.r * 255)).ToString("X2") +
            ((int)(color.g * 255)).ToString("X2") +
            ((int)(color.b * 255)).ToString("X2") +
            ((int)(color.a * 255)).ToString("X2");

        public static Color hexToColor(this string hex)
        {
            if (string.IsNullOrEmpty(hex)) return Color.black;

            hex = hex.Replace("0x", ""); //in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", ""); //in case the string is formatted #FFFFFF
            
            if (hex.Length == 6)
            {
                byte a = 255; //assume fully visible unless specified in hex
                byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                return new Color32(r, g, b, a);
            }
            
            if (hex.Length == 8)
            {
                byte a = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
                byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                return new Color32(r, g, b, a);
            }
            
            return Color.black;
        }

        public static Color TransformHSV(
            this Color color, // color to transform
            float H, // hue shift (in degrees)
            float S, // saturation multiplier (scalar)
            float V // value multiplier (scalar)
        )
        {
            var VSU = V * S * Math.Cos(H * Mathf.PI / 180);
            var VSW = V * S * Math.Sin(H * Mathf.PI / 180);

            return new Color
            {
                r = (float)((.299 * V + .701 * VSU + .168 * VSW) * color.r
                            + (.587 * V - .587 * VSU + .330 * VSW) * color.g
                            + (.114 * V - .114 * VSU - .497 * VSW) * color.b),
                g = (float)((.299 * V - .299 * VSU - .328 * VSW) * color.r
                            + (.587 * V + .413 * VSU + .035 * VSW) * color.g
                            + (.114 * V - .114 * VSU + .292 * VSW) * color.b),
                b = (float)((.299 * V - .3 * VSU + 1.25 * VSW) * color.r
                            + (.587 * V - .588 * VSU - 1.05 * VSW) * color.g
                            + (.114 * V + .886 * VSU - .203 * VSW) * color.b),
                a = 1
            };
        }

        public static int getHue(this Color color)
        {
            int r = (int)(color.r * 256);
            int g = (int)(color.g * 256);
            int b = (int)(color.b * 256);

            float min = Math.Min(Math.Min(r, g), b);
            float max = Math.Max(Math.Max(r, g), b);

            float hue;
            if (max == r)
            {
                hue = (g - b) / (max - min);

            }
            else if (max == g)
            {
                hue = 2f + (b - r) / (max - min);

            }
            else
            {
                hue = 4f + (r - g) / (max - min);
            }

            hue = hue * 60;
            if (hue < 0) hue = hue + 360;

            return (int)Mathf.Round(hue);
        }


        public static Vector3? PointOnGround(this Camera camera, Vector2 screenPosition,
            Plane ground)
        {
            var ray = camera.ScreenPointToRay(screenPosition);
            float distance;
            if (ground.Raycast(ray, out distance))
            {
                return ray.GetPoint(distance);
            }
            return null;
        }

        public static byte[] ReadAllBytes(this Stream stream)
        {
            if (stream is MemoryStream memoryStream1)
                return memoryStream1.ToArray();

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}