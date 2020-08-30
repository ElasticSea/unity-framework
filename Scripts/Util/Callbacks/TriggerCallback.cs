using UnityEngine;

namespace _Framework.Scripts.Util.Callbacks
{
    public class TriggerCallback : MonoBehaviour
    {
        public delegate void TriggerHandler(Collider collider);

        public event TriggerHandler OnEnter;
        public event TriggerHandler OnStay;
        public event TriggerHandler OnExit;

        private void OnTriggerEnter(Collider collider) => OnEnter?.Invoke(collider);
        private void OnTriggerStay(Collider collider) => OnStay?.Invoke(collider);
        private void OnTriggerExit(Collider collider) => OnExit?.Invoke(collider);
    }
}