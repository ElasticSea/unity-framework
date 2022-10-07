using UnityEngine;

namespace ElasticSea.Framework.Scripts.Extensions
{
    public static class CanvasExtensions
    {
        public static Texture2D Render(this Canvas canvas, TextureFormat format, string renderLayerName, int width, int height)
        {
            var previousLayer = canvas.gameObject.layer;
            var previousRenderMode = canvas.renderMode;
            var previousWorldCamera = canvas.worldCamera;
            
            canvas.gameObject.layer = LayerMask.NameToLayer(renderLayerName);
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            var cam = new GameObject().AddComponent<Camera>();
            cam.enabled = false;
            // Enabling HDR makes the background black
            cam.allowHDR = false;
            cam.cullingMask = LayerMask.GetMask(renderLayerName);
            cam.clearFlags = CameraClearFlags.Color;
            cam.targetTexture = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);
            canvas.worldCamera = cam;
            cam.Render();
            var texture = cam.targetTexture.ToTexture2D(format);
            GameObject.DestroyImmediate(cam.targetTexture);
            GameObject.DestroyImmediate(cam.gameObject);

            canvas.gameObject.layer = previousLayer;
            canvas.renderMode = previousRenderMode;
            canvas.worldCamera = previousWorldCamera;
            return texture;
        }
    }
}