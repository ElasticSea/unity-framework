using System;
using System.Collections.Generic;
using System.Linq;
using ElasticSea.Framework.Scripts.Util;
using ElasticSea.Framework.Util;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace ElasticSea.Framework.Extensions
{
    public static class BoundsExtensions
    {
	    public static Vector3[] GetVertices(this Bounds bounds)
	    {
		    var center = bounds.center;
		    var extent = bounds.extents;
		    return new[]
		    {
			    center + new Vector3(-extent.x, -extent.y, -extent.z),
			    center + new Vector3(+extent.x, -extent.y, -extent.z),
			    center + new Vector3(-extent.x, -extent.y, +extent.z),
			    center + new Vector3(+extent.x, -extent.y, +extent.z),
			    center + new Vector3(-extent.x, +extent.y, -extent.z),
			    center + new Vector3(+extent.x, +extent.y, -extent.z),
			    center + new Vector3(-extent.x, +extent.y, +extent.z),
			    center + new Vector3(+extent.x, +extent.y, +extent.z),
		    };
	    }

	    public static (Vector3[] vertices, Vector3 normal)[] GetFaces(this Bounds bounds)
	    {
		    var min = bounds.min;
		    var max = bounds.max;

		    var faces = new (Vector3[] vertices, Vector3 normal)[6];

		    var lbb = new Vector3(min.x, min.y, min.z);
		    var rbb = new Vector3(max.x, min.y, min.z);
		    var ltb = new Vector3(min.x, max.y, min.z);
		    var rtb = new Vector3(max.x, max.y, min.z);
		    var lbf = new Vector3(min.x, min.y, max.z);
		    var rbf = new Vector3(max.x, min.y, max.z);
		    var ltf = new Vector3(min.x, max.y, max.z);
		    var rtf = new Vector3(max.x, max.y, max.z);

		    faces[0] = (new[] { lbb, ltb, rtb, rbb }, Vector3.back);
		    faces[1] = (new[] { rbb, rtb, rtf, rbf }, Vector3.right);
		    faces[2] = (new[] { rbf, rtf, ltf, lbf }, Vector3.forward);
		    faces[3] = (new[] { lbf, ltf, ltb, lbb }, Vector3.left);
		    faces[4] = (new[] { lbf, lbb, rbb, rbf }, Vector3.down);
		    faces[5] = (new[] { ltb, ltf, rtf, rtb }, Vector3.up);

		    return faces;
	    }

        public static Bounds ToBounds(this IEnumerable<Vector3> vertices)
        {
            return vertices.ToArray().ToBounds();
        }
        
        public static Bounds ToBounds(this Vector3[] vertices)
        {
	        var min = Vector3.one * float.MaxValue;
	        var max = Vector3.one * float.MinValue;

	        for (int i = 0; i < vertices.Length; i++)
	        {
		        min = vertices[i].Min(min);
		        max = vertices[i].Max(max);
	        }

	        return new Bounds((max - min) / 2 + min, max - min);
        }

        public static BoundsInt ToBounds(this IEnumerable<Vector3Int> vertices)
        {
	        return vertices.ToArray().ToBounds();
        }
        
        public static BoundsInt ToBounds(this Vector3Int[] vertices)
        {
	        var min = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
	        var max = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

	        for (int i = 0; i < vertices.Length; i++)
	        {
		        min = vertices[i].Min(min);
		        max = vertices[i].Max(max);
	        }

	        return new BoundsInt((max - min) / 2 + min, max - min);
        }
        
        public static (Vector3 center, float radius) ToSphereBounds(this IEnumerable<Vector3> points)
        {
	        return points.ToArray().ToSphereBounds();
        }
        
        public static (Vector3 center, float radius) ToSphereBounds(this Vector3[] points)
        {
	        var center = points.ToBounds().center;
	        var length = points.Length;
	        var radius = 0f;
	        for (int i = 0; i < length; i++)
	        {
		        var point = points[i];
		        var currentRadius = (center - point).magnitude;
		        radius = Mathf.Max(currentRadius, radius);
	        }
	        return (center, radius);
        }
        
        public static (Vector2 center, float radius) ToFastCircleBounds(this Vector2[] points)
        {
	        var length = points.Length;

	        var rectBounds = points.ToRect();

	        var center = rectBounds.center;
	        var radius = 0f;
	        for (int i = 0; i < length; i++)
	        {
		        var point = points[i];
		        var currentRadius = (center - point).magnitude;
		        radius = Mathf.Max(currentRadius, radius);
	        }
	        return (center, radius);
        }
        
        public static (Vector2 center, float radius) ToCircleBounds(this Vector2[] points)
        {
	        var circle = SmallestEnclosingCircle.MakeCircle(points);
	        return (circle.c, (float) circle.r);
        }
        
        public static CylinderBounds ToCylinderBounds(this Vector3[] points)
        {
	        var length = points.Length;
	        var points2d = new Vector2[length];
	        var minY = points[0].y;
	        var maxY = points[0].y;
	        for (int i = 0; i < length; i++)
	        {
		        var vector3 = points[i];
		        points2d[i] = new Vector2(vector3.x, vector3.z);
		        minY = Mathf.Min(minY, vector3.y);
		        maxY = Mathf.Max(maxY, vector3.y);
	        }

	        var centerY = Mathf.Lerp(minY, maxY, 0.5f);
	        var height = maxY - minY;
	        var circle = SmallestEnclosingCircle.MakeCircle(points2d);
	        return new CylinderBounds(new Vector3(circle.c.x, centerY, circle.c.y), (float) circle.r, height);
        }
        
        public static Bounds ToCubeBounds(this Bounds[] localBounds)
        {
	        var clusterBlocksLength = localBounds.Length;
	        if (clusterBlocksLength == 0)
	        {
		        return default;
	        }
	        else
	        {
		        var newBounds = localBounds[0];

		        for (var i = 1; i < clusterBlocksLength; i++)
		        {
			        newBounds.Encapsulate(localBounds[i]);
		        }

		        return newBounds;
	        }
        }

        public static CylinderBounds ToCylinderBounds(this Bounds[] localBounds)
        {
	        var points = new Vector3[localBounds.Length * 8];
	        var pointsCount = 0;
	        for (int i = 0; i < localBounds.Length; i++)
	        {
		        var vertices = localBounds[i].GetVertices();
		        for (int j = 0; j < vertices.Length; j++)
		        {
			        points[pointsCount++] = vertices[j];
		        }
	        }

	        return points.ToCylinderBounds();
        }

        public static Bounds Encapsulate(this List<Bounds> bounds)
        {
            if (bounds.Count == 0)
                throw new ArgumentException("Bounds list is empty.");
            
            if (bounds.Count == 1)
                return bounds[0];

            return bounds
                .Aggregate((a, b) =>
                {
                    a.Encapsulate(b);
                    return a;
                });
        }
        
        public static Bounds TransformBounds(this Transform from, Transform to, Bounds bounds)
        {
	        var transformedVertices = bounds.GetVertices();
	        var length = transformedVertices.Length;
	        for (var i = 0; i < length; i++)
	        {
		        transformedVertices[i] = from.transform.TransformPoint(transformedVertices[i], to.transform);
	        }
	        return transformedVertices.ToBounds();
        }
        
        // Transforms local from one transform to another
        public static Bounds TransformBounds(this Matrix4x4 from, Matrix4x4 to, Bounds bounds)
        {
	        var transformedVertices = bounds.GetVertices();
	        var length = transformedVertices.Length;
	        for (var i = 0; i < length; i++)
	        {
		        var localFromPoint = transformedVertices[i];
		        var worldFromPoint = from.MultiplyPoint(localFromPoint);
		        var localToPoint = to.inverse.MultiplyPoint(worldFromPoint);
		        transformedVertices[i] = localToPoint;
	        }
	        return transformedVertices.ToBounds();
        }
        
        // Transforms world bounds to local bounds
        public static Bounds InverseTransformBounds(this Transform transform, Bounds bounds)
        {
	        var transformedVertices = bounds.GetVertices();
	        var length = transformedVertices.Length;
	        for (var i = 0; i < length; i++)
	        {
		        transformedVertices[i] = transform.InverseTransformPoint(transformedVertices[i]);
	        }
	        return transformedVertices.ToBounds();
        }
        
        public static Bounds LocalToWorldBounds(this Bounds localBounds, Transform transform)
        {
	        var vertices = localBounds.GetVertices();
	        var worldMin = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
	        var worldMax = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
	        for (var i = 0; i < vertices.Length; i++)
	        {
		       var worldVertex = transform.TransformPoint(vertices[i]);
		       worldMin = Vector3.Min(worldVertex, worldMin);
		       worldMax = Vector3.Max(worldVertex, worldMax);
	        }

	        var b = new Bounds();
	        b.SetMinMax(worldMin, worldMax);
	        return b;
        }
        
        public static Vector3[] TransformVertices(this Transform from, Transform to, Bounds bounds)
        {
	        var transformedVertices = bounds.GetVertices();
	        var length = transformedVertices.Length;
	        for (var i = 0; i < length; i++)
	        {
		        transformedVertices[i] = from.transform.TransformPoint(transformedVertices[i], to.transform);
	        }
	        return transformedVertices;
        }
	    
		public static Bounds GetCompositeRendererBounds(this GameObject go, Predicate<Renderer> filter = null,
			bool includeInactive = true)
		{
			var boundsInObject = go.GetComponentsInChildren<Renderer>(includeInactive)
				.Where(r => filter == null || filter(r))
				.Select(r => r.bounds)
				.Where(bounds => bounds.min != bounds.max)
				.ToArray();

			if (boundsInObject.Length == 0)
				return new Bounds();

			return boundsInObject
				.Aggregate((a, b) =>
				{
					a.Encapsulate(b);
					return a;
				});
		}

		public static Bounds GetCompositeBoundingBounds(this GameObject go, LayerMask? ignore = null)
		{
			var boundsInObject = go.GetComponentsInChildren<Renderer>(true)
				.Where(r => ignore == null || ignore.Value.Contains(r.gameObject.layer) == false)
				.Where(r => r is ParticleSystemRenderer == false)
				.Select(r => r.bounds)
				.Where(bounds => bounds.min != bounds.max)
				.ToArray();

			if (boundsInObject.Length == 0)
				return new Bounds();

			return boundsInObject
				.Aggregate((a, b) =>
				{
					a.Encapsulate(b);
					return a;
				});
		}
		
		public static bool Contains(this LayerMask mask, int layer)
		{
			return mask == (mask | (1 << layer));
		}

	    public static Bounds GetCompositeColliderBounds(this GameObject go, Predicate<Collider> filter = null,
		    bool includeInactive = true, bool ignoreTriggers = true)
	    {
		    var boundsInObject = go.GetComponentsInChildren<Collider>(includeInactive)
			    .Where(r => filter == null || filter(r))
			    .Where(r => r.isTrigger != ignoreTriggers)
			    .Select(r => r.bounds)
			    .Where(bounds => bounds.min != bounds.max)
			    .ToArray();

		    if (boundsInObject.Length == 0)
			    return new Bounds();

		    if (boundsInObject.Length == 1)
			    return boundsInObject[0];

		    return boundsInObject
			    .Aggregate((a, b) =>
			    {
				    a.Encapsulate(b);
				    return a;
			    });
	    }

	    public static Bounds GetCompositeColliderBounds(this GameObject[] go)
	    {
	        var boundsInObject = go
	            .Select(r => r.GetCompositeColliderBounds())
	            .Where(bounds => bounds.min != bounds.max)
	            .ToArray();

	        if (boundsInObject.Length == 0)
	            return new Bounds();

	        if (boundsInObject.Length == 1)
	            return boundsInObject[0];

	        return boundsInObject
	            .Aggregate((a, b) =>
	            {
	                a.Encapsulate(b);
	                return a;
	            });
	    }
		
	    public static Bounds Encapsulate(this IEnumerable<Bounds> bounds)
	    {
		    var enumerator = bounds.GetEnumerator();
		    if (enumerator.MoveNext() == false)
		    {
			    return default;
		    }

		    var bound = enumerator.Current;

		    while (enumerator.MoveNext())
		    {
			    bound.Encapsulate(enumerator.Current);
		    }

		    return bound;
	    }
		
	    public static Bounds Encapsulate(this Bounds[] bounds)
	    {
		    var length = bounds.Length;

		    if (length == 0)
			    return default;

		    var bound = bounds[0];

		    var min = bound.min;
		    var max = bound.max;
		    for (var i = 1; i < bounds.Length; i++)
		    {
			    var newBounds = bounds[i];
			    var newBoundsMin = newBounds.min;
			    var newBoundsMax = newBounds.max;
			    
			    min.x = Mathf.Min(newBoundsMin.x, min.x);
			    min.y = Mathf.Min(newBoundsMin.y, min.y);
			    min.z = Mathf.Min(newBoundsMin.z, min.z);
			    max.x = Mathf.Max(newBoundsMax.x, max.x);
			    max.y = Mathf.Max(newBoundsMax.y, max.y);
			    max.z = Mathf.Max(newBoundsMax.z, max.z);
		    }
		    
		    bound.SetMinMax(min, max);
		    return bound;
	    }
		
	    [Obsolete]
	    public static Bounds GetCompositeMeshBounds(this GameObject go, bool isSharedMesh = false, Predicate<MeshFilter> filter = null)
	    {
		    var bounds = go.GetComponentsInChildren<MeshFilter>(true)
			    .Where(mf => filter == null || filter(mf))
			    .Select(mf =>
			    {
				    var mesh = isSharedMesh ? mf.sharedMesh : mf.mesh;
				    var localBound = mf.transform.TransformBounds(go.transform, mesh.bounds);
				    return localBound;
			    })
			    .Where(b => b.size != Vector3.zero)
			    .ToArray();

		    if (bounds.Length == 0)
			    return new Bounds();

		    if (bounds.Length == 1)
			    return bounds[0];

		    var compositeBounds = bounds[0];

		    for (var i = 1; i < bounds.Length; i++)
		    {
			    compositeBounds.Encapsulate(bounds[i]);
		    }

		    return compositeBounds;
	    }
		
	    public static Bounds GetMeshBounds(this GameObject go, bool includeInactive = false, bool isSharedMesh = false, Predicate<Transform> filter = null)
	    {
		    var meshFilters = go.GetComponentsInChildren<MeshFilter>(includeInactive);
		    var skinnedMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive);

		    var pairs = new List<(Mesh, Transform)>();
		    
		    for (var i = 0; i < meshFilters.Length; i++)
		    {
			    var mf = meshFilters[i];
			    var mesh = isSharedMesh ? mf.sharedMesh : mf.mesh;
			    pairs.Add((mesh, mf.transform));
		    }
		    
		    for (var i = 0; i < skinnedMeshRenderers.Length; i++)
		    {
			    var mf = skinnedMeshRenderers[i];
			    var mesh =  mf.sharedMesh;
			    pairs.Add((mesh, mf.transform));
		    }

		    var bounds = new List<Bounds>();
		    
		    for (var i = 0; i < pairs.Count; i++)
		    {
			    var pair = pairs[i];
			    var transform = pair.Item2;
			    var mesh = pair.Item1;
			    
			    if(filter != null && filter(transform) == false)
				    continue;
			    
			    if(mesh == null)
				    continue;
			    
			    if(mesh.bounds.size == default)
				    continue;

			    var localBound = transform.TransformBounds(go.transform, mesh.bounds);
			    
			    bounds.Add(localBound);
		    }

		    return bounds.ToArray().Encapsulate();
	    }
		
	    public static Vector3[] GetMeshPoints(this GameObject go, bool includeInactive = false, bool isSharedMesh = false, Predicate<MeshFilter> filter = null)
	    {
		    IEnumerable<MeshFilter> meshFilters = go.GetComponentsInChildren<MeshFilter>(includeInactive);

		    if (filter != null)
		    {
			    meshFilters = meshFilters.Where(mf => filter(mf));
		    }
		    
		    var localVertices = new List<Vector3>();
		    foreach (var mf in meshFilters)
		    {
			    var mesh = isSharedMesh ? mf.sharedMesh : mf.mesh;
			    var vertices = mesh.vertices;
			    for (var index = 0; index < vertices.Length; index++)
			    {
				    vertices[index] =  mf.transform.TransformPoint(vertices[index], go.transform);
			    }
			    
			    localVertices.AddRange(vertices);
		    }
		    
		    return localVertices.ToArray();
	    }
	    
	    /// <summary>
	    /// Returns world positions for all vertices in mesh filter meshes
	    /// </summary>
	    public static Vector3[] GetWorldVertexPositions(this GameObject go, bool isSharedMesh = false)
	    {
		    var meshFilters = go.GetComponentsInChildren<MeshFilter>(true)
			    .Select(mf =>
			    {
				    var mesh = isSharedMesh ? mf.sharedMesh : mf.mesh;
				    return (mf, mesh);
			    })
			    .ToArray();

		    var verticesCount = 0;
		    for (var i = 0; i < meshFilters.Length; i++)
		    {
			    var (mf, mesh) = meshFilters[i];
			    verticesCount += mesh.vertices.Length;
		    }
		    
		    var vertices = new Vector3[verticesCount];
		    var index = 0;
		    for (var i = 0; i < meshFilters.Length; i++)
		    {
			    var (mf, mesh) = meshFilters[i];
			    var verts = mesh.vertices;
			    for (var j = 0; j < verts.Length; j++)
			    {
				    vertices[index++] = mf.transform.TransformPoint(verts[j]);
			    }
		    }

		    return vertices;
	    }

	    public static Bounds MoveToBottom(this Bounds from, Bounds to)
	    {
		    var newCenter = from.center.SetY(to.min.y + from.extents.y);
		    return new Bounds(newCenter, from.size);
	    }

	    public static Rect ToRect(this IEnumerable<Vector2> vertices)
	    {
		    return vertices.ToArray().ToRect();
	    }

	    public static Rect ToRect(this Vector2[] vertices)
	    {
		    var min = vertices[0];
		    var max = vertices[0];

		    for (var i = 1; i < vertices.Length; i++)
		    {
			    var current = vertices[i];
			    min = min.Min(current);
			    max = max.Max(current);
		    }

		    return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
	    }

	    public static Bounds SetMinMaxXOld(this Bounds bounds, float min, float max)
	    {
		    var newBounds = new Bounds();
		    newBounds.SetMinMax(bounds.min.SetX(min), bounds.max.SetX(max));
		    return newBounds;
	    }

	    public static Bounds SetMinXOld(this Bounds bounds, float min)
	    {
		    var newBounds = new Bounds();
		    newBounds.SetMinMax(bounds.min.SetX(min), bounds.max);
		    return newBounds;
	    }

	    public static Bounds SetMinYOld(this Bounds bounds, float min)
	    {
		    var newBounds = new Bounds();
		    newBounds.SetMinMax(bounds.min.SetY(min), bounds.max);
		    return newBounds;
	    }

	    public static Bounds SetMinZOld(this Bounds bounds, float min)
	    {
		    var newBounds = new Bounds();
		    newBounds.SetMinMax(bounds.min.SetZ(min), bounds.max);
		    return newBounds;
	    }

	    public static Bounds SetMaxXOld(this Bounds bounds, float max)
	    {
		    var newBounds = new Bounds();
		    newBounds.SetMinMax(bounds.min, bounds.max.SetX(max));
		    return newBounds;
	    }

	    public static Bounds SetMaxYOld(this Bounds bounds, float max)
	    {
		    var newBounds = new Bounds();
		    newBounds.SetMinMax(bounds.min, bounds.max.SetY(max));
		    return newBounds;
	    }

	    public static Bounds SetMaxZOld(this Bounds bounds, float max)
	    {
		    var newBounds = new Bounds();
		    newBounds.SetMinMax(bounds.min, bounds.max.SetZ(max));
		    return newBounds;
	    }

	    public static void SetMaxX(this ref Bounds bounds, float x)
	    {
		    var max = bounds.max;
		    bounds.max = new Vector3(x, max.y, max.z);
	    }

	    public static Bounds Rotate(this Bounds bounds, Quaternion rotate)
	    {
		    var center = bounds.center;
		    var extent = bounds.extents;
		    
		    var points = new[]
		    {
			    center + new Vector3(-extent.x, -extent.y, -extent.z),
			    center + new Vector3(+extent.x, -extent.y, -extent.z),
			    center + new Vector3(-extent.x, -extent.y, +extent.z),
			    center + new Vector3(+extent.x, -extent.y, +extent.z),
			    center + new Vector3(-extent.x, +extent.y, -extent.z),
			    center + new Vector3(+extent.x, +extent.y, -extent.z),
			    center + new Vector3(-extent.x, +extent.y, +extent.z),
			    center + new Vector3(+extent.x, +extent.y, +extent.z),
		    };

		    var min = Vector3.positiveInfinity;
		    var max = Vector3.negativeInfinity;
		    for (int i = 0; i < points.Length; i++)
		    {
			    points[i] = rotate * points[i];
			    min = Vector3.Min(min, points[i]);
			    max = Vector3.Max(max, points[i]);
		    }
		    
		    return Utils.Bounds(min, max);
	    }

	    public static Bounds Scale(this Bounds bounds, float scaleBy)
	    {
		    return bounds.Scale(new Vector3(scaleBy, scaleBy, scaleBy));
	    }

	    public static Bounds Scale(this Bounds bounds, Vector3 scaleBy)
	    {
		    return new Bounds(bounds.center.Multiply(scaleBy), bounds.size.Multiply(scaleBy));
	    }

	    public static Bounds Grow(this Bounds bounds, float growBy)
	    {
		    return bounds.Grow(new Vector3(growBy, growBy, growBy));
	    }

	    public static Bounds Grow(this Bounds bounds, Vector3 growBy)
	    {
		    return new Bounds(bounds.center, bounds.size + growBy * 2);
	    }

	    public static Bounds GrowRight(this Bounds bounds, float growBy) => GrowDir(bounds, Vector3.right, growBy);
	    public static Bounds GrowLeft(this Bounds bounds, float growBy) => GrowDir(bounds, Vector3.left, growBy);
	    public static Bounds GrowUp(this Bounds bounds, float growBy) => GrowDir(bounds, Vector3.up, growBy);
	    public static Bounds GrowDown(this Bounds bounds, float growBy) => GrowDir(bounds, Vector3.down, growBy);
	    public static Bounds GrowForward(this Bounds bounds, float growBy) => GrowDir(bounds, Vector3.forward, growBy);
	    public static Bounds GrowBack(this Bounds bounds, float growBy) => GrowDir(bounds, Vector3.back, growBy);

	    public static Bounds ShrinkRight(this Bounds bounds, float shrinkBy) => GrowDir(bounds, Vector3.right, -shrinkBy);
	    public static Bounds ShrinkLeft(this Bounds bounds, float shrinkBy) => GrowDir(bounds, Vector3.left, -shrinkBy);
	    public static Bounds ShrinkUp(this Bounds bounds, float shrinkBy) => GrowDir(bounds, Vector3.up, -shrinkBy);
	    public static Bounds ShrinkDown(this Bounds bounds, float shrinkBy) => GrowDir(bounds, Vector3.down, -shrinkBy);
	    public static Bounds ShrinkForward(this Bounds bounds, float shrinkBy) => GrowDir(bounds, Vector3.forward, -shrinkBy);
	    public static Bounds ShrinkBack(this Bounds bounds, float shrinkBy) => GrowDir(bounds, Vector3.back, -shrinkBy);

	    private static Bounds GrowDir(Bounds bounds, Vector3 direction, float growBy)
	    {
		    var offset = direction * growBy;
		    return new Bounds(bounds.center + offset / 2, bounds.size + direction.Abs() * growBy);
	    }

	    public static Bounds Shrink(this Bounds bounds, float shrinkBy)
	    {
		    return bounds.Grow(-shrinkBy);
	    }

	    public static Bounds Shrink(this Bounds bounds, Vector3 shrinkBy)
	    {
		    return bounds.Grow(-shrinkBy);
	    }

	    public static Vector3 TopCenter(this Bounds bounds)
	    {
		    return new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
	    }

	    public static Vector3 BottomCenter(this Bounds bounds)
	    {
		    return new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
	    }

	    public static Vector3 RightCenter(this Bounds bounds)
	    {
		    return new Vector3(bounds.max.x, bounds.center.y, bounds.center.z);
	    }

	    public static Vector3 LeftCenter(this Bounds bounds)
	    {
		    return new Vector3(bounds.min.x, bounds.center.y, bounds.center.z);
	    }

	    public static Vector3 ForwardCenter(this Bounds bounds)
	    {
		    return new Vector3(bounds.center.x, bounds.center.y, bounds.max.z);
	    }

	    public static Vector3 BackCenter(this Bounds bounds)
	    {
		    return new Vector3(bounds.center.x, bounds.center.y, bounds.min.z);
	    }

	    public static Rect BottomSide(this Bounds bounds)
	    {
		    return new Rect(bounds.min.x, bounds.min.z, bounds.size.x, bounds.size.z);
	    }

	    public static Rect TopSide(this Bounds bounds) => bounds.BottomSide();

	    public static Rect BackSide(this Bounds bounds)
	    {
		    return new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);
	    }

	    public static Rect FrontSide(this Bounds bounds) => bounds.BackSide();

	    public static Rect RightSide(this Bounds bounds)
	    {
		    return new Rect(bounds.min.z, bounds.min.y, bounds.size.z, bounds.size.y);
	    }

	    public static Rect LeftSide(this Bounds bounds) => bounds.RightSide();

	    public static Vector3 ClosestOnTheSurface(this Bounds bounds, Vector3 point, float maxDist)
	    {
		    var min = bounds.min;
		    var max = bounds.max;
		    var center = bounds.center;
		    
		    var insideX = point.x >= min.x && point.x <= max.x;
		    var insideY = point.y >= min.y && point.y <= max.y;
		    var insideZ = point.z >= min.z && point.z <= max.z;

		    float surfaceDist(float value, float min, float max)
		    {
			    return Mathf.Min(Mathf.Abs(min - value), Mathf.Abs(max - value));
		    }

		    var xSurfaceDist = (insideY && insideZ) ? surfaceDist(point.x, min.x, max.x) : float.PositiveInfinity;
		    var ySurfaceDist = (insideX && insideZ) ? surfaceDist(point.y, min.y, max.y) : float.PositiveInfinity;
		    var zSurfaceDist = (insideX && insideY) ? surfaceDist(point.z, min.z, max.z) : float.PositiveInfinity;

		    var smallest = float.PositiveInfinity;
		    var smallestId = -1;

		    if (xSurfaceDist < smallest)
		    {
			    smallest = xSurfaceDist;
			    smallestId = 0;
		    }
		    
		    if (ySurfaceDist < smallest)
		    {
			    smallest = ySurfaceDist;
			    smallestId = 1;
		    }
		    
		    if (zSurfaceDist < smallest)
		    {
			    smallest = zSurfaceDist;
			    smallestId = 2;
		    }


		    if (smallest < maxDist)
		    {
			    switch (smallestId)
			    {
				    case 0:
					    // x
					    point.x = point.x > center.x ? max.x : min.x;
					    break;

				    case 1:
					    // y
					    point.y = point.y > center.y ? max.y : min.y;
					    break;

				    case 2:
					    // z
					    point.z = point.z > center.z ? max.z : min.z;
					    break;
			    }
		    }

		    return point;
	    }

	    public static void Move(this ref Bounds bounds, Vector3 value)
	    {
			bounds.SetMinMax(bounds.min + value, bounds.max + value);
	    }
    }
}