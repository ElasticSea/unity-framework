using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace ElasticSea.Framework.Scripts.Extensions
{
    public static class RenderTextureExtensions
    {
        public static Texture2D ToTexture2D(this RenderTexture renderTexture, TextureFormat textureFormat)
        {
            var activeRenderTexture = RenderTexture.active;
            
            var texture = new Texture2D(renderTexture.width, renderTexture.height, textureFormat, renderTexture.mipmapCount, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            
            RenderTexture.active = activeRenderTexture;
            
            return texture;
        }
    }
}