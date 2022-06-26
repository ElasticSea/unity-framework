using UnityEngine;

namespace ElasticSea.Framework.Scripts.Extensions
{
    public static class RenderTextureExtensions
    {
        public static Texture2D ToTexture2D(this RenderTexture renderTexture, TextureFormat textureFormat)
        {
            var texture = new Texture2D(renderTexture.width, renderTexture.height, textureFormat, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
            return texture;
        }
    }
}