using System.IO;
using ElasticSea.Framework.Scripts.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class CanvasScreenshotUtility : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private int width = 1920;
        [SerializeField] private int height = 1080;
        [SerializeField] private string layerMask;
        [SerializeField] private string fileName;
        
        public void Render()
        {
            var texture = canvas.Render(TextureFormat.RGB24, layerMask, width, height);

            var pngBytes = texture.EncodeToPNG();
            var path = Path.Combine(Application.persistentDataPath, $"{fileName}.png");
            File.WriteAllBytes(path, pngBytes);
        }
    }
}