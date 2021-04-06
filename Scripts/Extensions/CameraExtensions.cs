using System.Linq;
using UnityEngine;

namespace ElasticSea.Framework.Extensions
{
    public static class CameraExtensions
    {
        public static Texture2D RenderToTexture(this Camera camera, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipChain = false)
        {
            var currentRT = RenderTexture.active;
            RenderTexture.active = camera.targetTexture;
            camera.Render();
            var image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height, textureFormat, mipChain);
            image.ReadPixels(new Rect(0, 0, image.width, image.height), 0, 0);
            image.Apply();
            RenderTexture.active = currentRT;
            return image;
        }

        /// <summary>
        /// Focuses orthographic camera so that gameobject fills viewrect
        /// </summary>
        public static void FocusCamera(this Camera camera, GameObject gameObject)
        {
            var vertices = gameObject.GetWorldVertexPositions();
            var center = vertices.Average();
            var radius = vertices.Select(v => (center - v).magnitude).Max();

            camera.FocusCamera(center, radius);
        }
        
        /// <summary>
        /// Focuses orthographic camera on target position so that target with radius fills viewrect
        /// </summary>
        public static void FocusCamera(this Camera camera, Vector3 targetPosition, float targetRadius)
        {
            if (camera.orthographic)
            {
                camera.transform.position = targetPosition - camera.transform.forward * (targetRadius + camera.nearClipPlane);
                camera.orthographicSize = targetRadius / Mathf.Min(camera.aspect, 1);
            }
            else
            {
                // Get the horizontal FOV, since it may be the limiting of the two FOVs to properly encapsulate the objects
                var horizontalFov = 2f * Mathf.Atan(Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2f) * camera.aspect) * Mathf.Rad2Deg;
                // Use the smaller FOV as it limits what would get cut off by the frustum        
                var fov = Mathf.Min(camera.fieldOfView, horizontalFov);

                // var distance = radius / Mathf.Tan((camera.fieldOfView * Mathf.Deg2Rad) / 2f);
                // Take sin so the whole sphere is in the view
                var distance = targetRadius / Mathf.Sin((fov * Mathf.Deg2Rad) / 2f);

                camera.transform.position = targetPosition - camera.transform.forward * distance;
            }
        }
    }
}