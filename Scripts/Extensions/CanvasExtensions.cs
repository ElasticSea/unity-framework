using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Extensions
{
    public static class CanvasExtensions
    {
        public static Texture2D Render(this Canvas canvas, TextureFormat format, string renderLayerName, int width, int height)
        {
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
            return texture;
        }
    }
}