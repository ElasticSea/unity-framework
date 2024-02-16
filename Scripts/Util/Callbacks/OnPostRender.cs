using UnityEngine;

namespace ElasticSea.Framework.Util.Callbacks
{
    public class OnPostRenderCallback : MonoBehaviour
    {
        public event OnPostRenderHandler OnPostRenderEvent;

        public delegate void OnPostRenderHandler();

        private void OnPostRender() => OnPostRenderEvent?.Invoke();
    }
}