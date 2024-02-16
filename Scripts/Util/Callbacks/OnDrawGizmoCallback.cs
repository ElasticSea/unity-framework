using UnityEngine;

namespace ElasticSea.Framework.Util.Callbacks
{
    public class OnDrawGizmoCallback : MonoBehaviour
    {
        public delegate void OnDrawGizmosHandler();
        
        public event OnDrawGizmosHandler OnDrawGizmosEvent;
        public event OnDrawGizmosHandler OnDrawGizmosSelectedEvent;

        private void OnDrawGizmos() => OnDrawGizmosEvent?.Invoke();
        private void OnDrawGizmosSelected() => OnDrawGizmosSelectedEvent?.Invoke();
    }
}