using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Core.Util
{
    public class MeshUtils
    {
        public static byte[] WriteMesh(Mesh mesh)
        {
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;
            var uv = mesh.uv;
            var normals = mesh.normals;
            var colors = mesh.colors;
            
            var bytes = new byte[
                ByteExtesnions.GetBytesInVector3Array(vertices.Length) +
                ByteExtesnions.GetBytesInIntArray(triangles.Length) +
                ByteExtesnions.GetBytesInVector2Array(uv.Length) +
                ByteExtesnions.GetBytesInVector3Array(normals.Length) +
                ByteExtesnions.GetBytesInColorArray(colors.Length)
            ];

            var offset = 0;
            offset += vertices.WriteBytes(bytes, offset);
            offset += triangles.WriteBytes(bytes, offset);
            offset += uv.WriteBytes(bytes, offset);
            offset += normals.WriteBytes(bytes, offset);
            offset += colors.WriteBytes(bytes, offset);

            return bytes;
        }
        
        public static Mesh ReadMesh(byte[] bytes)
        {
            var offset = 0;
            var vertices = bytes.ToVector3s(offset, out var read1);
            offset += read1;
            var triangles = bytes.ToInts(offset, out var read2);
            offset += read2;
            var uv = bytes.ToVector2s(offset, out var read3);
            offset += read3;
            var normals = bytes.ToVector3s(offset, out var read4);
            offset += read4;
            var colors = bytes.ToColors(offset, out var read5);
            offset += read5;

            return new Mesh()
            {
                vertices = vertices,
                triangles = triangles,
                uv = uv,
                normals = normals,
                colors = colors,
            };
        }
        
        public static void WriteToFile(Mesh mesh, string path)
        {
            File.WriteAllBytes(path, WriteMesh(mesh));
        }

        public static Mesh ReadFromFile(string path)
        {
            if(!File.Exists(path))
            {
                Debug.LogError("meshFile.dat file does not exist.");
                return default;
            }
            
            return ReadMesh(File.ReadAllBytes(path));
        }
    }
}