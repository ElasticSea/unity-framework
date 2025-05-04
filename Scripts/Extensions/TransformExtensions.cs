using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace ElasticSea.Framework.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Sets localPosition to Vector3.zero, localRotation to Quaternion.identity, and localScale to Vector3.one
        /// </summary>
        public static void Reset(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        public static void MoveHierarchyLeft(this Transform t)
        {
            if (t.parent?.parent != null)
                t.SetParent(t.parent.parent);
        }

        public static void MoveHierarchyDown(this Transform t)
        {
            if (t.parent != null)
                t.SetSiblingIndex(Mathf.Min(t.GetSiblingIndex() + 1, t.parent.childCount - 1));
        }

        public static void MoveHierarchyUp(this Transform t)
        {
            if (t.parent != null)
                t.SetSiblingIndex(Mathf.Max(t.GetSiblingIndex() - 1, 0));
        }

        /// <summary>
        /// Transforms position from local space to local space of another transform.
        /// </summary>
        public static Vector3 TransformPoint(this Transform t, Vector3 position, Transform dest)
        {
            var world = t.TransformPoint(position);
            return dest.InverseTransformPoint(world);
        }

        /// <summary>
        /// Transforms world rotation to transform local space
        /// </summary>
        public static Quaternion InverseTransformRotation(this Transform t, Quaternion rotation)
        {
            return Quaternion.Inverse(t.rotation) * rotation;
        }

        /// <summary>
        /// Transforms local rotation to world space
        /// </summary>
        public static Quaternion TransformRotation(this Transform t, Quaternion rotation)
        {
            return t.rotation * rotation;
        }

        /// <summary>
        /// Transforms rotation from local space to local space of another transform
        /// </summary>
        public static Quaternion TransformRotation(this Transform t, Quaternion rotation, Transform dest)
        {
            var world = t.TransformRotation(rotation);
            return dest.InverseTransformRotation(world);
        }

        public static void SetLocalX(this Transform transform, float x)
        {
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
        }

        public static void SetLocalY(this Transform transform, float y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
        }

        public static void SetLocalZ(this Transform transform, float z)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
        }

        public static void SetLocalScaleX(this Transform transform, float x)
        {
            transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        }

        public static void SetLocalScaleY(this Transform transform, float y)
        {
            transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
        }

        public static void SetLocalScaleZ(this Transform transform, float z)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
        }

        public static void SetLocalRotationX(this Transform transform, float x)
        {
            transform.localRotation = Quaternion.Euler(x, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
        }

        public static void SetLocalRotationY(this Transform transform, float y)
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, y, transform.localRotation.eulerAngles.z);
        }

        public static void SetLocalRotationZ(this Transform transform, float z)
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, z);
        }

        public static void SetX(this Transform transform, float x)
        {
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        public static void SetY(this Transform transform, float y)
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

        public static void SetZ(this Transform transform, float z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }

        public static void AddX(this Transform transform, float x)
        {
            transform.position += new Vector3(x, 0, 0);
        }

        public static void AddY(this Transform transform, float y)
        {
            transform.position += new Vector3(0, y, 0);
        }

        public static void AddZ(this Transform transform, float z)
        {
            transform.position += new Vector3(0, 0, z);
        }

        public static Transform CopyLocalFrom(this Transform destination, Transform source)
        {
            destination.localPosition = source.localPosition;
            destination.localRotation = source.localRotation;
            destination.localScale = source.localScale;
            return destination;
        }

        public static Transform CopyWorldFrom(this Transform destination, Transform source)
        {
            destination.position = source.position;
            destination.rotation = source.rotation;
            destination.localScale = source.localScale;
            return destination;
        }

        public static GameObject Instantiate(this GameObject gameObject)
        {
            return UnityObject.Instantiate(gameObject, Vector3.zero, Quaternion.identity);
        }

        public static T InstantiateChild<T>(this Transform parent, T source) where T : Component
        {
            var instance = UnityObject.Instantiate(source, Vector3.zero, Quaternion.identity);
            instance.transform.SetParent(parent, false);
            instance.transform.localScale = Vector3.one;
            return instance;
        }

        public static T Instantiate<T>(this Transform parent, T source, bool worldPositionStays = false) where T : Component
        {
            var instance = UnityObject.Instantiate(source, parent, worldPositionStays);
            instance.transform.localScale = Vector3.one;
            return instance;
        }

        public static void RemoveAllChildren(this Transform transform)
        {
#if UNITY_EDITOR
            while (transform.childCount > 0)
                UnityObject.DestroyImmediate(transform.GetChild(0).gameObject);
#else
            foreach (Transform child in transform) UnityObject.Destroy(child.gameObject);
#endif
        }

        public static Transform[] Children(this Transform transform, bool includeItself = false)
        {
            if (includeItself)
            {
                return ChildrenIncludeSelf(transform);
            }
            else
            {
                
                return Children(transform);
            }
        }
        
        private static Transform[] ChildrenIncludeSelf(Transform transform)
        {
            var array = new Transform[transform.childCount + 1];

            array[0] = transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                array[i + 1] = transform.GetChild(i);
            }
            
            return array;
        }
        
        private static Transform[] Children(Transform transform)
        {
            var array = new Transform[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                array[i] = transform.GetChild(i);
            }
            
            return array;
        }
        
        // public static Transform[] Children(this Transform transform, bool includeItself = false)
        // {
        //     var array = new Transform[transform.childCount + 1];
        //
        //     array[0] = transform;
        //     for (int i = 0; i < transform.childCount; i++)
        //     {
        //         array[i + 1] = transform.GetChild(i);
        //     }
        //     
        //     return array;
        // }

        public static IEnumerable<Transform> ChildrenEnumerable(this Transform transform, bool includeItself = false)
        {
            if (includeItself) yield return transform;

            for (var i = 0; i < transform.childCount; i++)
            {
                yield return transform.GetChild(i);
            }
        }

        public static List<Transform> AllChildren(this Transform transform, bool includeItself = false)
        {
            return transform.AllChildrenEnumerable(includeItself).ToList();
        }
        
        public static List<Transform> GetAllChildren(this Transform transform, Predicate<Transform> filter)
        {
            var output = new List<Transform>();
            transform.GetAllChildren(output, filter);
            return output;
        }
        
        private static void GetAllChildren(this Transform transform, List<Transform> output, Predicate<Transform> filter)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);

                if (filter(child))
                {
                    output.Add(child);
                    child.GetAllChildren(output, filter);
                }
            }
        }

        public static IEnumerable<Transform> AllChildrenEnumerable(this Transform transform, bool includeItself = false)
        {
            if (includeItself) yield return transform;

            for (var i = 0; i < transform.childCount; i++)
            {
                foreach (var v in transform.GetChild(i).AllChildren(true))
                {
                    yield return v;
                }
            }
        }
        
        public static void AlignToCenter(this Transform transform, Bounds thisBounds, Transform other, Bounds otherBounds)
        {
            var (position, rotation) = transform.GetAlignToCenter(thisBounds, other, otherBounds);
            transform.position = position;
            transform.rotation = rotation;
        }
        
        public static (Vector3 position, Quaternion rotation) GetAlignToCenter(this Transform transform, Bounds thisBounds, Transform other, Bounds otherBounds)
        {
            var c0 = otherBounds.center;
            var c1 = thisBounds.center;
            var position = other.TransformPoint(c0 - c1.Multiply(transform.lossyScale).Divide(other.lossyScale));
            var rotation = other.rotation;
            return (position, rotation);
        }

        public static void AlignToBottom(this Transform transform, Bounds thisBounds, Transform other, Bounds otherBounds)
        {
            var (position, rotation) = transform.GetAlignToBottom(thisBounds, other, otherBounds);
            transform.position = position;
            transform.rotation = rotation;
        }

        public static (Vector3 position, Quaternion rotation) GetAlignToBottom(this Transform transform, Bounds thisBounds, Transform other, Bounds otherBounds)
        {
            var c0 = otherBounds.center - new Vector3(0, otherBounds.size.y, 0);
            var c1 = thisBounds.center - new Vector3(0, thisBounds.extents.y, 0);
            var position = other.TransformPoint(c0 - c1.Multiply(transform.lossyScale).Divide(other.lossyScale));
            var rotation = other.rotation;
            return (position, rotation);
        }

        public static Vector3 TransformVector(this Transform t, Vector3 vector, Transform dest)
        {
            var world = t.TransformVector(vector);
            return dest.InverseTransformVector(world);
        }

        public static void SetLocalPose(this Transform t, Pose pose)
        {
            t.SetLocalPositionAndRotation(pose.position, pose.rotation);
        }

        public static Matrix4x4 GetTRSRelativeToParent(this Transform child, Transform parent)
        {
            var trs = Matrix4x4.identity;
            
            while (child != parent)
            {
                trs = child.GetTRS() * trs;
                child = child.parent;
            }

            return trs;
        }

        public static Matrix4x4 GetTRS(this Transform transform)
        {
            return Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        }
    }
}