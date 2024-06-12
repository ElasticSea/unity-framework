using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ElasticSea.Framework.Extensions
{
	public static class VectorExtensions
	{
		public static float SqrDistanceToV3(this Vector3 v, Vector3 to)
		{
			return (v - to).sqrMagnitude;
		}

		public static float SqrDistanceToV2(this Vector2 v, Vector2 to)
		{
			return (v - to).sqrMagnitude;
		}

		public static Vector2 Add(this Vector2 v, float f)
		{
			return v + new Vector2(f, f);
		}

		public static Vector3 Add(this Vector3 v, float f)
		{
			return v + new Vector3(f, f);
		}

		public static Vector2 Subtract(this Vector2 v, float f)
		{
			return v - new Vector2(f, f);
		}

		public static Vector3 Subtract(this Vector3 v, float f)
		{
			return v - new Vector3(f, f);
		}
		
		public static Vector3 ToVector3(this Vector2 v)
		{
			return new Vector3(v.x, v.y, 0);
		}
		
		public static Vector3 ToVector3(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}
		
		public static Vector2 ToVector2(this Vector3 v)
		{
			return new Vector2(v.x, v.y);
		}
		
		public static Vector3 FlipXy(this Vector3 v)
		{
			return new Vector3(v.y, v.x, v.z);
		}

		public static Vector3 FlipXz(this Vector3 v)
		{
			return new Vector3(v.z, v.y, v.x);
		}

		public static Vector3 FlipYz(this Vector3 v)
		{
			return new Vector3(v.x, v.z, v.y);
		}

		public static bool ApproxEqual(this Vector2 a, Vector2 b)
		{
			return ApproxEqual(new Vector3(a.x, a.y), new Vector3(b.x, b.y));
		}

		public static bool ApproxEqual(this Vector3 a, Vector3 b)
		{
			return a.ApproxEqual(b, 0.001f);
		}

		// http://answers.unity3d.com/questions/131624/vector3-comparison.html#answer-131672
		public static bool ApproxEqual(this Vector3 a, Vector3 b, float angleError)
		{
			//if they aren't the same length, don't bother checking the rest.
			if (!Mathf.Approximately(a.magnitude, b.magnitude))
				return false;

			var cosAngleError = Mathf.Cos(angleError * Mathf.Deg2Rad);

			//A value between -1 and 1 corresponding to the angle.
			//The dot product of normalized Vectors is equal to the cosine of the angle between them.
			//So the closer they are, the closer the value will be to 1. Opposite Vectors will be -1
			//and orthogonal Vectors will be 0.
			var cosAngle = Vector3.Dot(a.normalized, b.normalized);

			//If angle is greater, that means that the angle between the two vectors is less than the error allowed.
			return cosAngle >= cosAngleError;
		}

		/// <summary>
		/// Determine the signed angle between two vectors, with normal 'n'
		/// as the rotation axis.
		/// </summary>
		public static float AngleSigned(this Vector3 v1, Vector3 v2, Vector3 n)
		{
			return Mathf.Atan2(
				Vector3.Dot(n, Vector3.Cross(v1, v2)),
				Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
		}

		public static Vector2 Clamp(this Vector2 vector, float min, float max)
		{
			var x = Mathf.Clamp(vector.x, min, max);
			var y = Mathf.Clamp(vector.y, min, max);
			return new Vector2(x, y);
		}

		public static Vector2 Clamp(this Vector2 vector, Vector2 min, Vector2 max)
		{
			var x = Mathf.Clamp(vector.x, min.x, max.x);
			var y = Mathf.Clamp(vector.y, min.y, max.y);
			return new Vector2(x, y);
		}

		public static Vector3 Clamp(this Vector3 vector, float min, float max)
		{
			var x = Mathf.Clamp(vector.x, min, max);
			var y = Mathf.Clamp(vector.y, min, max);
			var z = Mathf.Clamp(vector.z, min, max);
			return new Vector3(x, y, z);
		}

		public static Vector3 ClampX(this Vector3 vector, float min, float max)
		{
			var x = Mathf.Clamp(vector.x, min, max);
			return new Vector3(x, vector.y, vector.z);
		}

		public static Vector3 ClampY(this Vector3 vector, float min, float max)
		{
			var y = Mathf.Clamp(vector.y, min, max);
			return new Vector3(vector.x, y, vector.z);
		}

		public static Vector3 ClampZ(this Vector3 vector, float min, float max)
		{
			var z = Mathf.Clamp(vector.z, min, max);
			return new Vector3(vector.x, vector.y, z);
		}

		public static Vector3 Clamp(this Vector3 vector, Vector3 min, Vector3 max)
		{
			var x = Mathf.Clamp(vector.x, min.x, max.x);
			var y = Mathf.Clamp(vector.y, min.y, max.y);
			var z = Mathf.Clamp(vector.z, min.z, max.z);
			return new Vector3(x, y, z);
		}

		public static Vector3 ClampNegative(this Vector3 vector)
		{
			var x = clampNegative(vector.x);
			var y = clampNegative(vector.y);
			var z = clampNegative(vector.z);
			return new Vector3(x, y, z);
		}

		public static Vector3 FloorNegative(this Vector3 vector)
		{
			var x = FloorNegative(vector.x);
			var y = FloorNegative(vector.y);
			var z = FloorNegative(vector.z);
			return new Vector3(x, y, z);
		}

		public static Vector3 Floor(this Vector3 vector)
		{
			var x = Mathf.Floor(vector.x);
			var y = Mathf.Floor(vector.y);
			var z = Mathf.Floor(vector.z);
			return new Vector3(x, y, z);
		}

		public static Vector3 Round(this Vector3 vector)
		{
			var x = Mathf.Round(vector.x);
			var y = Mathf.Round(vector.y);
			var z = Mathf.Round(vector.z);
			return new Vector3(x, y, z);
		}

		public static Vector3 Round(this Vector3 vector, int multiple)
		{
			var x = Mathf.Round(vector.x / multiple) * multiple;
			var y = Mathf.Round(vector.y / multiple) * multiple;
			var z = Mathf.Round(vector.z / multiple) * multiple;
			return new Vector3(x, y, z);
		}

		public static Vector3 Abs(this Vector3 vector)
		{
			var x = Mathf.Abs(vector.x);
			var y = Mathf.Abs(vector.y);
			var z = Mathf.Abs(vector.z);
			return new Vector3(x, y, z);
		}

		public static Vector2 Abs(this Vector2 vector)
		{
			var x = Mathf.Abs(vector.x);
			var y = Mathf.Abs(vector.y);
			return new Vector2(x, y);
		}

		public static Vector3 Flip(this Vector3 vector) => vector * -1;

		public static Vector2 FlipXY(this Vector2 vector) => new Vector2(vector.y, vector.x);

		public static Vector3 Snap(this Vector3 vector, float roundTo, float precision = float.MaxValue, float offset = 0) => new Vector3(
			vector.x.Snap(roundTo, precision, offset),
			vector.y.Snap(roundTo, precision, offset),
			vector.z.Snap(roundTo, precision, offset)
		);

		public static float Snap(this float value, float roundTo, float precision = float.MaxValue, float offset = 0)
		{
			var rounded = (float)(Mathf.RoundToInt((value + offset) / roundTo) * (decimal)roundTo) - offset;
			return Mathf.Abs(rounded - value) < precision ? rounded : value;
		}
		
		public static Vector3 RoundTo(this Vector3 vector, float x, float y, float z)
		{
			return vector.RoundTo(new Vector3(x, y, z));
		}
		
		public static Vector3 RoundTo(this Vector3 vector, Vector3 roundTo)
		{
			var x = vector.x.RoundTo(roundTo.x);
			var y = vector.y.RoundTo(roundTo.y);
			var z = vector.z.RoundTo(roundTo.z);
			
			return new Vector3(x, y, z);
		}

		public static Vector3 GetClosestPointOnLine(this Vector3 point, Vector3 a, Vector3 b)
		{
			var dir = (b - a).normalized;
			var v = point - a;
			var d = Vector3.Dot(v, dir);
			return a + dir * d;
		}

		public static float LineSignOffset(this Vector2 point, Vector2 a, Vector2 b)
		{
			return Mathf.Sign((b.x - a.x) * (point.y - a.y) - (b.y - a.y) * (point.x - a.x));
		}

		private static float clampNegative(float value)
		{
			return value >= 0 ? Mathf.Ceil(value) : Mathf.Floor(value);
		}

		private static float FloorNegative(float value)
		{
			return value >= 0 ? Mathf.Floor(value) : Mathf.Ceil(value);
		}

		public static Vector2 Subtract(this Vector2 vectorA, Vector2 vectorB)
		{
			return new Vector2(vectorA.x - vectorB.x, vectorA.y - vectorB.y);
		}

		public static Vector3 Divide(this Vector3 vectorA, Vector3 vectorB)
		{
			return new Vector3(vectorA.x / vectorB.x, vectorA.y / vectorB.y, vectorA.z / vectorB.z);
		}

		public static Vector2 Divide(this Vector2 vectorA, Vector2 vectorB)
		{
			return new Vector2(vectorA.x / vectorB.x, vectorA.y / vectorB.y);
		}

		public static Vector2 Divide(this Vector2 vectorA, float value)
		{
			return new Vector2(vectorA.x / value, vectorA.y / value);
		}

		public static Vector3 Multiply(this Vector3 vectorA, Vector3 vectorB)
		{
			return Vector3.Scale(vectorA, vectorB);
		}

		public static Vector2 Multiply(this Vector2 vectorA, Vector2 vectorB)
		{
			return Vector2.Scale(vectorA, vectorB);
		}

		public static Vector2 Multiply(this Vector2 vectorA, float value)
		{
			return vectorA * value;
		}

		public static Vector3 Multiply(this Vector3 vectorA, float value)
		{
			return vectorA * value;
		}

		public static float Sum(this Vector2 vector)
		{
			return vector.x + vector.y;
		}

		public static float Sum(this Vector3 vector)
		{
			return vector.x + vector.y + vector.z;
		}

		public static float Distance(this Vector3 vectorA, Vector3 vectorB)
		{
			return Vector3.Distance(vectorA, vectorB);
		}

		public static float Distance(this Vector2 vectorA, Vector2 vectorB)
		{
			return Vector2.Distance(vectorA, vectorB);
		}

		public static float Angle(this Vector3 vectorA, Vector3 vectorB)
		{
			return Vector3.Angle(vectorA, vectorB);
		}

		public static float Angle(this Vector2 vectorA, Vector2 vectorB)
		{
			return Vector2.Angle(vectorA, vectorB);
		}
	    
		public static float SignedAngle(this Vector2 vectorA, Vector2 vectorB)
		{
			return Vector2.SignedAngle(vectorA, vectorB);
		}

		public static Vector3 Cross(this Vector3 vectorA, Vector3 vectorB)
		{
			return Vector3.Cross(vectorA, vectorB);
		}

		public static Vector3 Min(this Vector3 vectorA, Vector3 vectorB)
		{
			return Vector3.Min(vectorA, vectorB);
		}

		public static Vector3Int Min(this Vector3Int vectorA, Vector3Int vectorB)
		{
			return Vector3Int.Min(vectorA, vectorB);
		}

		public static Vector2 Min(this Vector2 vectorA, Vector2 vectorB)
		{
			return Vector2.Min(vectorA, vectorB);
		}

		public static float Min(this Vector3 vector)
		{
			return Mathf.Min(vector.x, Mathf.Min(vector.y, vector.z));
		}

		public static float Min(this Vector2 vector)
		{
			return Mathf.Min(vector.x, vector.y);
		}

		public static Vector3 Max(this Vector3 vectorA, Vector3 vectorB)
		{
			return Vector3.Max(vectorA, vectorB);
		}

		public static Vector3Int Max(this Vector3Int vectorA, Vector3Int vectorB)
		{
			return Vector3Int.Max(vectorA, vectorB);
		}

		public static Vector2 Max(this Vector2 vectorA, Vector2 vectorB)
		{
			return Vector2.Max(vectorA, vectorB);
		}

		public static float Max(this Vector3 vector)
		{
			return Mathf.Max(vector.x, Mathf.Max(vector.y, vector.z));
		}

		public static float Max(this Vector2 vector)
		{
			return Mathf.Max(vector.x, vector.y);
		}

		public static Vector3 Smallest(this Vector3 thisVector, Vector3 otherVector)
		{
			if (thisVector.magnitude < otherVector.magnitude)
			{
				return thisVector;
			}
			else
			{
				return otherVector;
			}
		}

		public static Vector2 FromXY(this Vector3 vector)
		{
			return new Vector2(vector.x, vector.y);
		}

		public static Vector2 FromXZ(this Vector3 vector)
		{
			return new Vector2(vector.x, vector.z);
		}

		public static Vector2 FromYZ(this Vector3 vector)
		{
			return new Vector2(vector.y, vector.z);
		}

		public static Vector2 FromZY(this Vector3 vector)
		{
			return new Vector2(vector.z, vector.y);
		}

		public static Vector3 ToXy(this Vector2 vector)
		{
			return new Vector3(vector.x, vector.y, 0);
		}

		public static Vector3 ToXZ(this Vector2 vector)
		{
			return new Vector3(vector.x, 0, vector.y);
		}

		public static Vector3 ToYZ(this Vector2 vector)
		{
			return new Vector3(0, vector.x, vector.y);
		}

		public static Vector2 ToFloat(this Vector2Int intVec)
		{
			return new Vector2(intVec.x, intVec.y);
		}

		public static Vector3 ToFloat(this Vector3Int intVec)
		{
			return new Vector3(intVec.x, intVec.y, intVec.z);
		}

		public static Vector3Int ToInt(this Vector3 vector)
		{
			return new Vector3Int((int) vector.x, (int) vector.y, (int) vector.z);
		}

		public static Vector2Int ToInt(this Vector2 vector)
		{
			return new Vector2Int((int) vector.x, (int) vector.y);
		}

		public static Vector2 Normal(this Vector2 vector)
		{
			return new Vector2(-vector.y, vector.x);
		}

		public static Vector2 ClampMagnitude(this Vector2 vector, int value)
		{
			return Vector2.ClampMagnitude(vector, value);
		}

		public static Vector3 LockToAxis(this Vector3 vector, Vector3 lockAxis)
		{
			return (vector.Multiply(lockAxis)).normalized;
		}

		public static Vector3 HalfWay(this Vector3 vector, Vector3 anotherVector)
		{
			return Vector3.Lerp(vector, anotherVector, 0.5f);
		}

		public static Vector3 AddX(this Vector3 vector3, float x)
		{
			return new Vector3(vector3.x + x, vector3.y, vector3.z);
		}

		public static Vector3 AddY(this Vector3 vector3, float y)
		{
			return new Vector3(vector3.x, vector3.y + y, vector3.z);
		}

		public static Vector3 AddZ(this Vector3 vector3, float z)
		{
			return new Vector3(vector3.x, vector3.y, vector3.z + z);
		}

		public static Vector3 SetX(this Vector3 vector3, float x)
		{
			return new Vector3(x, vector3.y, vector3.z);
		}

		public static Vector3 SetY(this Vector3 vector3, float y)
		{
			return new Vector3(vector3.x, y, vector3.z);
		}

		public static Vector3 SetZ(this Vector3 vector3, float z)
		{
			return new Vector3(vector3.x, vector3.y, z);
		}

		public static Vector2 SetX(this Vector2 vector2, float x)
		{
			return new Vector2(x, vector2.y);
		}

		public static Vector2 SetY(this Vector2 vector2, float y)
		{
			return new Vector2(vector2.x, y);
		}

		public static Vector3 Center(this IEnumerable<Vector3> points)
		{
			return points.Any() ? points.Aggregate((a, b) => a + b) / points.Count() : Vector3.zero;
		}

		public static float Dot(this Vector3 vectorA, Vector3 vectorB)
		{
			return Vector3.Dot(vectorA, vectorB);
		}

		public static float Dot(this Vector2 vectorA, Vector2 vectorB)
		{
			return Vector2.Dot(vectorA, vectorB);
		}

		public static Vector3 Project(this Vector3 vector3, Vector3 normal)
		{
			return Vector3.Project(vector3, normal);
		}

		public static Vector3 ProjectOnPlane(this Vector3 vector3, Plane plane)
		{
			return vector3 - (vector3 - (-plane.normal * plane.distance)).Dot(plane.normal) * plane.normal;
		}

		public static float Angle(this Plane plane, Vector3 vector3)
		{
			var projected = vector3.ProjectOnPlane(plane);
			return projected.Angle(vector3);
		}

		public static bool RaycastPoint(this Plane plane, Ray ray, out Vector3 point)
		{
			if (plane.Raycast(ray, out var d))
			{
				point =  ray.origin + ray.direction * d;
				return true;
			}
			else
			{
				point = Vector3.zero;
				return false;
			}
		}

		public static bool RaycastPoint(this Plane plane, Vector3 start, Vector3 end, out Vector3 point)
		{
			var ray = new Ray(start, end - start);
			if (plane.Raycast(ray, out var d))
			{
				if (Mathf.Abs(d) >= (end - start).magnitude)
				{
					point = Vector3.zero;
					return false;
				}
				
				point =  ray.origin + ray.direction * d;
				return true;
			}
			else
			{
				point = Vector3.zero;
				return false;
			}
		}

		public static Vector3? Intersect(this Plane plane, Ray ray)
		{
			float enter;
			var intersect = plane.Raycast(ray, out enter);
			return intersect ? ray.origin + ray.direction * enter : (Vector3?)null;
		}

		public static Vector3? Intersect(this Ray ray)
		{
			RaycastHit hit;
			var intersect = Physics.Raycast(ray, out hit);
			return intersect ? hit.point : (Vector3?)null;
		}

		public static RaycastHit? RaycastHit(this Ray ray)
		{
			RaycastHit hit;
			var intersect = Physics.Raycast(ray, out hit);
			return intersect ? hit : (RaycastHit?) null;
		}

		public static Vector3 ProjectOnPlane(this Vector3 vector3, Vector3 planeNormal)
		{
			return Vector3.ProjectOnPlane(vector3, planeNormal);
		}

		public static Vector3 ClampMagnitude(this Vector3 vector3, float max)
		{
			return Vector3.ClampMagnitude(vector3, max);
		}

		public static Vector3 Lerp(this Vector3 vector3A, Vector3 vector3B, float t)
		{
			return Vector3.Lerp(vector3A, vector3B, t);
		}

		public static Vector2 Lerp(this Vector2 vector3A, Vector2 vector3B, float t)
		{
			return Vector2.Lerp(vector3A, vector3B, t);
		}

		public static float InverseLerpWeird(this Vector3 from, Vector3 to, Vector3 point)
		{
			var mainVec = to - from;
			var toProject = point - from;
			var projected = toProject.Project(mainVec);
			return projected.magnitude / mainVec.magnitude * Mathf.Sign(mainVec.Dot(projected));
		}

		public static Vector3 InverseLerp(this Vector3 from, Vector3 to, Vector3 point)
		{
			var x = Mathf.InverseLerp(from.x, to.x, point.x);
			var y = Mathf.InverseLerp(from.y, to.y, point.y);
			var z = Mathf.InverseLerp(from.z, to.z, point.z);
			return new Vector3(x, y, z);
		}
		
		public static Vector3 Average(this IEnumerable<Vector3> points)
		{
			var v = Vector3.zero;
			var count = 0;
			foreach (var point in points)
			{
				v += point;
				count++;
			}
			return v / count;
		}

		public static Vector3 Average(this Vector3[] points)
		{
			var v = Vector3.zero;
			for (var i = 0; i < points.Length; i++)
			{
				v += points[i];
			}
			return v / points.Length;
		}

		public static Vector2 Average(this Vector2[] points)
		{
			var v = Vector2.zero;
			for (var i = 0; i < points.Length; i++)
			{
				v += points[i];
			}
			return v / points.Length;
		}

		public static Vector3 Lerp(this IList<Vector3> source, float t)
		{
			var count = source.Count;
			var segments = count - 1;
			var index = t * segments;
			var lowerBound = Mathf.FloorToInt(index);
			var upperBound = Mathf.CeilToInt(index);
			var tSegment = (t - lowerBound / (float) segments) * segments;
			return source[lowerBound].Lerp(source[upperBound], tSegment);
		}
	    
		public static float DistanceToPoint(this Ray ray, Vector3 point)
		{
			return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
		}
		
		public static Vector3 Lerp(this Vector3 a, Vector3 b, Vector3 t)
		{
			var x = a.x + (b.x - a.x) * Mathf.Clamp01(t.x);
			var y = a.y + (b.y - a.y) * Mathf.Clamp01(t.y);
			var z = a.z + (b.z - a.z) * Mathf.Clamp01(t.z);
			return new Vector3(x, y, z);
		}

		public static Vector3Int CeilToInt(this Vector3 vec)
		{
			var x = Mathf.CeilToInt(vec.x);
			var y = Mathf.CeilToInt(vec.y);
			var z = Mathf.CeilToInt(vec.z);
			return new Vector3Int(x, y, z);
		}

		public static Vector3Int RoundToInt(this Vector3 vec)
		{
			var x = Mathf.RoundToInt(vec.x);
			var y = Mathf.RoundToInt(vec.y);
			var z = Mathf.RoundToInt(vec.z);
			return new Vector3Int(x, y, z);
		}

		public static Vector2Int RoundToInt(this Vector2 vec)
		{
			var x = Mathf.RoundToInt(vec.x);
			var y = Mathf.RoundToInt(vec.y);
			return new Vector2Int(x, y);
		}
	}
}