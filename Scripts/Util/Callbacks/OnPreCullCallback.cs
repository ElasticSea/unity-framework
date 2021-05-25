using UnityEngine;

namespace ElasticSea.Framework.Util.Callbacks
{
    public class OnPreCullCallback : MonoBehaviour
    {
        public delegate void OnPreCullHandler();

        public event OnPreCullHandler OnPreCullEvent;

        private void OnPreCull() => OnPreCullEvent?.Invoke();
    }
}