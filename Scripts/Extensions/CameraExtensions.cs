using UnityEngine;

namespace ElasticSea.Framework.Extensions
{
    public static class CameraExtensions
    {
        public static Texture2D RenderToTexture(this UnityEngine.Camera camera, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipChain = false)
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

        public static Vector3 FocusSphere(this UnityEngine.Camera camera, GameObject go)
        {
            var bounds = go.GetCompositeRendererBounds();
            var position = bounds.center;

            // Get the radius of a sphere circumscribing the bounds
            var radius = bounds.size.magnitude / 2;

            return camera.FocusSphere(position, radius);
        }
        
        public static Vector3 FocusSphere(this UnityEngine.Camera camera, Vector3 position, float radius)
        {
            var distance = camera.FocusDistance(radius);
            return position - camera.transform.forward * distance;
        }
        
        public static float FocusDistance(this UnityEngine.Camera camera, float radius)
        {
            // Get the horizontal FOV, since it may be the limiting of the two FOVs to properly encapsulate the objects
            var horizontalFov = 2f * Mathf.Atan(Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2f) * camera.aspect) * Mathf.Rad2Deg;
            // Use the smaller FOV as it limits what would get cut off by the frustum        
            var fov = Mathf.Min(camera.fieldOfView, horizontalFov);

            // var distance = radius / Mathf.Tan((camera.fieldOfView * Mathf.Deg2Rad) / 2f);
            // Take sin so the whole sphere is in the view
            var distance = radius / Mathf.Sin((fov * Mathf.Deg2Rad) / 2f);

            if (camera.orthographic)
                camera.orthographicSize = radius / Mathf.Min(camera.aspect, 1);

            return distance;
        }
    }
}