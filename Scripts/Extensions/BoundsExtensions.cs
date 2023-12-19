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
        
        public static Rect ToRectBounds(this Vector2[] points)
        {
	        var length = points.Length;
	        
	        var min = points[0];
	        var max = points[0];
	        for (int i = 0; i < length; i++)
	        {
		        var point = points[i];
		        min = Vector2.Min(min, point);
		        max = Vector2.Max(max, point);
	        }

	        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
        
        public static (Vector2 center, float radius) ToFastCircleBounds(this Vector2[] points)
        {
	        var length = points.Length;

	        var rectBounds = points.ToRectBounds();

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
		    var max = vertices[1];

		    for (var i = 0; i < vertices.Length; i++)
		    {
			    var current = vertices[i];
			    min = min.Min(current);
			    max = max.Max(current);
		    }

		    return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
	    }

	    public static Bounds SetMinMaxX(this Bounds bounds, float min, float max)
	    {
		    var newBounds = new Bounds();
		    newBounds.SetMinMax(bounds.min.SetX(min), bounds.max.SetX(max));
		    return newBounds;
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

	    public static Bounds Grow(this Bounds bounds, float growBy)
	    {
		    return bounds.Grow(new Vector3(growBy, growBy, growBy));
	    }

	    public static Bounds Grow(this Bounds bounds, Vector3 growBy)
	    {
		    return new Bounds(bounds.center, bounds.size + growBy * 2);
	    }

	    public static Bounds Shrink(this Bounds bounds, float shrinkBy)
	    {
		    return bounds.Grow(-shrinkBy);
	    }

	    public static Bounds Shrink(this Bounds bounds, Vector3 shrinkBy)
	    {
		    return bounds.Grow(-shrinkBy);
	    }
    }
}