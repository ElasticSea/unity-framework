using System.Collections.Generic;
using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util
{
	public static class MeshUtils
	{
		private static class IcosphereUtils
		{
			private struct TriangleIndices
			{
				public int v1;
				public int v2;
				public int v3;

				public TriangleIndices(int v1, int v2, int v3)
				{
					this.v1 = v1;
					this.v2 = v2;
					this.v3 = v3;
				}
			}

			// return index of point in the middle of p1 and p2
			private static int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache,
				float radius)
			{
				// first check if we have it already
				bool firstIsSmaller = p1 < p2;
				long smallerIndex = firstIsSmaller ? p1 : p2;
				long greaterIndex = firstIsSmaller ? p2 : p1;
				long key = (smallerIndex << 32) + greaterIndex;

				int ret;
				if (cache.TryGetValue(key, out ret))
				{
					return ret;
				}

				// not in cache, calculate it
				Vector3 point1 = vertices[p1];
				Vector3 point2 = vertices[p2];
				Vector3 middle = new Vector3(
					(point1.x + point2.x) / 2f,
					(point1.y + point2.y) / 2f,
					(point1.z + point2.z) / 2f
				);

				// add vertex makes sure point is on unit sphere
				int i = vertices.Count;
				vertices.Add(middle.normalized * radius);

				// store it, return index
				cache.Add(key, i);

				return i;
			}

			public static Mesh Create(float radius, int recursionLevel = 3)
			{
				List<Vector3> vertList = new List<Vector3>();
				Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();
				int index = 0;

				// create 12 vertices of a icosahedron
				float t = (1f + Mathf.Sqrt(5f)) / 2f;

				vertList.Add(new Vector3(-1f, t, 0f).normalized * radius);
				vertList.Add(new Vector3(1f, t, 0f).normalized * radius);
				vertList.Add(new Vector3(-1f, -t, 0f).normalized * radius);
				vertList.Add(new Vector3(1f, -t, 0f).normalized * radius);

				vertList.Add(new Vector3(0f, -1f, t).normalized * radius);
				vertList.Add(new Vector3(0f, 1f, t).normalized * radius);
				vertList.Add(new Vector3(0f, -1f, -t).normalized * radius);
				vertList.Add(new Vector3(0f, 1f, -t).normalized * radius);

				vertList.Add(new Vector3(t, 0f, -1f).normalized * radius);
				vertList.Add(new Vector3(t, 0f, 1f).normalized * radius);
				vertList.Add(new Vector3(-t, 0f, -1f).normalized * radius);
				vertList.Add(new Vector3(-t, 0f, 1f).normalized * radius);


				// create 20 triangles of the icosahedron
				List<TriangleIndices> faces = new List<TriangleIndices>();

				// 5 faces around point 0
				faces.Add(new TriangleIndices(0, 11, 5));
				faces.Add(new TriangleIndices(0, 5, 1));
				faces.Add(new TriangleIndices(0, 1, 7));
				faces.Add(new TriangleIndices(0, 7, 10));
				faces.Add(new TriangleIndices(0, 10, 11));

				// 5 adjacent faces
				faces.Add(new TriangleIndices(1, 5, 9));
				faces.Add(new TriangleIndices(5, 11, 4));
				faces.Add(new TriangleIndices(11, 10, 2));
				faces.Add(new TriangleIndices(10, 7, 6));
				faces.Add(new TriangleIndices(7, 1, 8));

				// 5 faces around point 3
				faces.Add(new TriangleIndices(3, 9, 4));
				faces.Add(new TriangleIndices(3, 4, 2));
				faces.Add(new TriangleIndices(3, 2, 6));
				faces.Add(new TriangleIndices(3, 6, 8));
				faces.Add(new TriangleIndices(3, 8, 9));

				// 5 adjacent faces
				faces.Add(new TriangleIndices(4, 9, 5));
				faces.Add(new TriangleIndices(2, 4, 11));
				faces.Add(new TriangleIndices(6, 2, 10));
				faces.Add(new TriangleIndices(8, 6, 7));
				faces.Add(new TriangleIndices(9, 8, 1));


				// refine triangles
				for (int i = 0; i < recursionLevel; i++)
				{
					List<TriangleIndices> faces2 = new List<TriangleIndices>();
					foreach (var tri in faces)
					{
						// replace triangle by 4 triangles
						int a = getMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, radius);
						int b = getMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, radius);
						int c = getMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, radius);

						faces2.Add(new TriangleIndices(tri.v1, a, c));
						faces2.Add(new TriangleIndices(tri.v2, b, a));
						faces2.Add(new TriangleIndices(tri.v3, c, b));
						faces2.Add(new TriangleIndices(a, b, c));
					}

					faces = faces2;
				}

				var vertices = vertList.ToArray();

				var mesh = new Mesh();
				mesh.vertices = vertices;

				List<int> triList = new List<int>();
				for (int i = 0; i < faces.Count; i++)
				{
					triList.Add(faces[i].v1);
					triList.Add(faces[i].v2);
					triList.Add(faces[i].v3);
				}

				mesh.triangles = triList.ToArray();
				mesh.uv = new Vector2[vertices.Length];

				Vector3[] normales = new Vector3[vertList.Count];
				for (int i = 0; i < normales.Length; i++)
					normales[i] = vertList[i].normalized;


				mesh.normals = normales;

				mesh.RecalculateBounds();
				mesh.Optimize();

				return mesh;
			}
		}

		public static Mesh Icosphere(float radius, int recursionLevel = 3)
		{
			return IcosphereUtils.Create(radius, recursionLevel);
		}

		public static Mesh Quad()
		{
			return Quad(Vector3.zero, Vector2.one, Quaternion.identity);
		}
	    
        public static Mesh Quad(Vector3 center, Vector2 size, Quaternion rotation = default)
        {
            return new Mesh
            {
                vertices = new[]
                {
                    center + rotation * new Vector3(-size.x/2, 0, -size.y/2),
                    center + rotation * new Vector3(-size.x/2, 0, +size.y/2),
                    center + rotation * new Vector3(+size.x/2, 0, +size.y/2),
                    center + rotation * new Vector3(+size.x/2, 0, -size.y/2)
                },
                triangles = new[]
                {
                    0, 1, 2,
                    0, 2, 3
                },
                uv = new[]
                {
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0)
                },
                normals = new[]
                {
                    rotation * new Vector3(0, 1, 0),
                    rotation * new Vector3(0, 1, 0),
                    rotation * new Vector3(0, 1, 0),
                    rotation * new Vector3(0, 1, 0)
                }
            };
        }

        public static Mesh Cube()
        {
	        return Cube(Vector3.zero, Vector3.one);
        }

        public static Mesh Cube(Vector3 center, Vector3 size)
        {
	        // Define the co-ordinates of each Corner of the cube 
	        var c = new Vector3[8];

	        c[0] = center + new Vector3(-size.x * .5f, -size.y * .5f, size.z * .5f);
	        c[1] = center + new Vector3(size.x * .5f, -size.y * .5f, size.z * .5f);
	        c[2] = center + new Vector3(size.x * .5f, -size.y * .5f, -size.z * .5f);
	        c[3] = center + new Vector3(-size.x * .5f, -size.y * .5f, -size.z * .5f);

	        c[4] = center + new Vector3(-size.x * .5f, size.y * .5f, size.z * .5f);
	        c[5] = center + new Vector3(size.x * .5f, size.y * .5f, size.z * .5f);
	        c[6] = center + new Vector3(size.x * .5f, size.y * .5f, -size.z * .5f);
	        c[7] = center + new Vector3(-size.x * .5f, size.y * .5f, -size.z * .5f);


	        // Define the vertices that the cube is composed of:
	        // I have used 16 vertices (4 vertices per side). 
	        // This is because I want the vertices of each side to have separate normals.
	        // (so the object renders light/shade correctly) 
	        Vector3[] vertices =
	        {
	            c[0], c[1], c[2], c[3], // Bottom
	            c[7], c[4], c[0], c[3], // Left
	            c[4], c[5], c[1], c[0], // Front
	            c[6], c[7], c[3], c[2], // Back
	            c[5], c[6], c[2], c[1], // Right
	            c[7], c[6], c[5], c[4] // Top
	        };


	        // Define each vertex's Normal
	        var up = Vector3.up;
	        var down = Vector3.down;
	        var forward = Vector3.forward;
	        var back = Vector3.back;
	        var left = Vector3.left;
	        var right = Vector3.right;


	        Vector3[] normals =
	        {
	            down, down, down, down, // Bottom
	            left, left, left, left, // Left
	            forward, forward, forward, forward, // Front
	            back, back, back, back, // Back
	            right, right, right, right, // Right
	            up, up, up, up // Top
	        };


	        // Define each vertex's UV co-ordinates
	        var uv00 = new Vector2(0f, 0f);
	        var uv10 = new Vector2(1f, 0f);
	        var uv01 = new Vector2(0f, 1f);
	        var uv11 = new Vector2(1f, 1f);

	        Vector2[] uvs =
	        {
	            uv11, uv01, uv00, uv10, // Bottom
	            uv11, uv01, uv00, uv10, // Left
	            uv11, uv01, uv00, uv10, // Front
	            uv11, uv01, uv00, uv10, // Back	        
	            uv11, uv01, uv00, uv10, // Right 
	            uv11, uv01, uv00, uv10 // Top
	        };


	        // Define the Polygons (triangles) that make up the our Mesh (cube)
	        // IMPORTANT: Unity uses a 'Clockwise Winding Order' for determining front-facing polygons.
	        // This means that a polygon's vertices must be defined in 
	        // a clockwise order (relative to the camera) in order to be rendered/visible.
	        int[] triangles =
	        {
	            3, 1, 0, 3, 2, 1, // Bottom	
	            7, 5, 4, 7, 6, 5, // Left
	            11, 9, 8, 11, 10, 9, // Front
	            15, 13, 12, 15, 14, 13, // Back
	            19, 17, 16, 19, 18, 17, // Right
	            23, 21, 20, 23, 22, 21, // Top
	        };


	        // Build the Mesh
	        var mesh = new Mesh
	        {
	            vertices = vertices,
	            triangles = triangles,
	            normals = normals,
	            uv = uvs,
	            bounds = new Bounds(center, size)
	        };

	        return mesh;
        }
        
        public static float Intersect(this Mesh mesh, Ray ray)
        {
	        var vertices = mesh.vertices;
	        var triangles = mesh.triangles;

	        for (int i = 0; i < triangles.Length; i+=3)
	        {
		        var v0 = vertices[triangles[i + 0]];
		        var v1 = vertices[triangles[i + 1]];
		        var v2 = vertices[triangles[i + 2]];

		        var intersectRayTriangle = IntersectRayTriangle(ray, v0, v1, v2);
		        if (float.IsNaN(intersectRayTriangle) == false)
		        {
			        return intersectRayTriangle;
		        }
	        }

	        return float.NaN;
        }
        
        public static bool Intersect(this Mesh mesh, Vector3 from, Vector3 to, out Vector3 hit)
        {
	        var ray = new Ray(from, to - from);
	        var distanceHit = mesh.Intersect(ray);

	        hit = Vector3.zero;
	        
	        if (float.IsNaN(distanceHit))
	        {
		        return false;
	        }

	        if (distanceHit > from.Distance(to))
	        {
		        return false;
	        }

	        hit = ray.origin + ray.direction * distanceHit;
	        return true;
        }

        const float kEpsilon = 0.000001f;
     
        /// <summary>
        /// Ray-versus-triangle intersection test suitable for ray-tracing etc.
        /// Port of Möller–Trumbore algorithm c++ version from:
        /// https://en.wikipedia.org/wiki/Möller–Trumbore_intersection_algorithm
        /// </summary>
        /// <returns><c>The distance along the ray to the intersection</c> if one exists, <c>NaN</c> if one does not.</returns>
        /// <param name="ray">Le ray.</param>
        /// <param name="v0">A vertex of the triangle.</param>
        /// <param name="v1">A vertex of the triangle.</param>
        /// <param name="v2">A vertex of the triangle.</param>
        public static float IntersectRayTriangle(Ray ray, Vector3 v0, Vector3 v1, Vector3 v2) {
     
	        // edges from v1 & v2 to v0.     
	        Vector3 e1 = v1 - v0;
	        Vector3 e2 = v2 - v0;
       
	        Vector3 h = Vector3.Cross(ray.direction, e2);
	        float   a = Vector3.Dot  (e1           , h );
	        if ((a > -kEpsilon) && (a < kEpsilon)) {
		        return float.NaN;
	        }
       
	        float   f = 1.0f / a;
       
	        Vector3 s = ray.origin - v0;
	        float   u = f * Vector3.Dot(s, h);
	        if ((u < 0.0f) || (u > 1.0f)) {
		        return float.NaN;
	        }
       
	        Vector3 q = Vector3.Cross(s, e1);
	        float   v = f * Vector3.Dot(ray.direction, q);
	        if ((v < 0.0f) || (u  + v > 1.0f)) {
		        return float.NaN;
	        }
       
	        float t = f * Vector3.Dot(e2, q);
	        if (t > kEpsilon) {
		        return t;
	        }
	        else {
		        return float.NaN;
	        }
        }
    }
}