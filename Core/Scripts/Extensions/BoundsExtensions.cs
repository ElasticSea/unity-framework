using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Extensions
{
    public static class BoundsExtensions
    {
        public static Vector3[] GetVertices(this Bounds bounds) => new[]
        {
            new Vector3(bounds.center.x, bounds.center.y, bounds.center.z) + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z),
            new Vector3(bounds.center.x, bounds.center.y, bounds.center.z) + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z),
            new Vector3(bounds.center.x, bounds.center.y, bounds.center.z) + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z),
            new Vector3(bounds.center.x, bounds.center.y, bounds.center.z) + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z),
            new Vector3(bounds.center.x, bounds.center.y, bounds.center.z) + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z),
            new Vector3(bounds.center.x, bounds.center.y, bounds.center.z) + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z),
            new Vector3(bounds.center.x, bounds.center.y, bounds.center.z) + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z),
            new Vector3(bounds.center.x, bounds.center.y, bounds.center.z) + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z),
        };

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
			return bounds.GetVertices()
				.Select(bv => from.transform.TransformPoint(bv, to.transform))
				.ToBounds();
		}

		public static Bounds GetCompositeMeshBounds(this GameObject go)
		{
			var boundsInObject = go.GetComponentsInChildren<MeshFilter>(true)
				.Select(r => (r.mesh ?? r.sharedMesh).bounds)
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
		    bool includeInactive = true)
	    {
		    var boundsInObject = go.GetComponentsInChildren<Collider>(includeInactive)
			    .Where(r => filter == null || filter(r))
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
		
	    public static Bounds? GetLocalCompositeMeshBounds(this GameObject go)
	    {
		    var boundsInObject = go.GetComponentsInChildren<MeshFilter>(true)
			    .Select(mf =>
			    {
				    var bounds = (mf.mesh ?? mf.sharedMesh).bounds;
				    var localBound = mf.transform.TransformBounds(go.transform, bounds);
				    return localBound;
			    })
			    .Where(bounds => bounds.min != bounds.max)
			    .ToArray();

		    if (boundsInObject.Length == 0)
			    return null;

		    if (boundsInObject.Length == 1)
			    return boundsInObject.First();

		    return boundsInObject
			    .Aggregate((a, b) =>
			    {
				    a.Encapsulate(b);
				    return a;
			    });
	    }

	    public static Bounds MoveToBottom(this Bounds from, Bounds to)
	    {
		    var newCenter = from.center.SetY(to.min.y + from.extents.y);
		    return new Bounds(newCenter, from.size);
	    }
    }
}