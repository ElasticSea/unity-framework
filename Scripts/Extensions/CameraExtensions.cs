using System.Linq;
using ElasticSea.Framework.Scripts.Extensions;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace ElasticSea.Framework.Extensions
{
    public static class CameraExtensions
    {
        /// <summary>
        /// Fill camera viewrect with gameobject
        /// </summary>
        public static void FillCameraView(this Camera camera, GameObject gameObject, float boundsMultiplier = 1)
        {
            var vertices = gameObject.GetWorldVertexPositions();
            var center = vertices.Average();
            var radius = vertices.Select(v => (center - v).magnitude).Max();

            camera.FillCameraView(center, radius * boundsMultiplier);
        }
        
        /// <summary>
        /// Fill camera viewrect with object at position with radius
        /// </summary>
        public static void FillCameraView(this Camera camera, Vector3 targetPosition, float targetRadius)
        {
            if (camera.orthographic)
            {
                camera.transform.position = targetPosition - camera.transform.forward * (targetRadius + camera.nearClipPlane);
                camera.orthographicSize = targetRadius / Mathf.Min(camera.aspect, 1);
            }
            else
            {
                var distance = camera.GetFillCameraViewDistance(targetRadius);
                camera.transform.position = targetPosition - camera.transform.forward * distance;
            }
        }

        /// <summary>
        /// Get distance from the camera to the object based on the objects sphere radius and camera fov
        /// </summary>
        public static float GetFillCameraViewDistance(this Camera camera, float targetRadius)
        {
            // Get the horizontal FOV, since it may be the limiting of the two FOVs to properly encapsulate the objects
            var horizontalFov = 2f * Mathf.Atan(Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2f) * camera.aspect) * Mathf.Rad2Deg;
            // Use the smaller FOV as it limits what would get cut off by the frustum        
            var fov = Mathf.Min(camera.fieldOfView, horizontalFov);

            // var distance = radius / Mathf.Tan((camera.fieldOfView * Mathf.Deg2Rad) / 2f);
            // Take sin so the whole sphere is in the view
            return targetRadius / Mathf.Sin((fov * Mathf.Deg2Rad) / 2f);
        }
    }
}