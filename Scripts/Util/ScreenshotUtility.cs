using System.IO;
using ElasticSea.Framework.Scripts.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    [RequireComponent(typeof(Camera))]
    public class ScreenshotUtility : MonoBehaviour
    {
        [SerializeField] private int width = 1920;
        [SerializeField] private int height = 1080;
        [SerializeField] private float fieldOfView = 60;
        
        public void Render()
        {
            var cam = GetComponent<Camera>();

            var prevFov = cam.fieldOfView;
            cam.fieldOfView = fieldOfView;
            
            var rt = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);
            cam.targetTexture = rt;
            cam.Render();

            var tex = rt.ToTexture2D(TextureFormat.ARGB32);

            var pngBytes = tex.EncodeToPNG();
            var path = Path.Combine(Application.persistentDataPath, "output.png");
            File.WriteAllBytes(path, pngBytes);

            cam.fieldOfView = prevFov;
            cam.targetTexture = null;
            Destroy(rt);
            Destroy(tex);
        }
    }
}