using UnityEngine;

namespace _Framework.Scripts.Util.Callbacks
{
    public class CollisionCallback : MonoBehaviour
    {
        public delegate void CollisionHandler(Collision collision);

        public event CollisionHandler OnEnter;
        public event CollisionHandler OnStay;
        public event CollisionHandler OnExit;

        private void OnCollisionEnter(Collision collision) => OnEnter?.Invoke(collision);
        private void OnCollisionStay(Collision collision) => OnStay?.Invoke(collision);
        private void OnCollisionExit(Collision collision) => OnExit?.Invoke(collision);
    }
}