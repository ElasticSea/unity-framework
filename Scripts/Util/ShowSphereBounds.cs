using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class ShowSphereBounds : MonoBehaviour
    {
        [SerializeField] private Vector3 center;
        [SerializeField] private float radius;
        [SerializeField] private Color color = Color.red;

        public Vector3 Center
        {
            get => center;
            set => center = value;
        }

        public float Radius
        {
            get => radius;
            set => radius = value;
        }

        public Color Color
        {
            get => color;
            set => color = value;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = color;
            Gizmos.DrawWireSphere(center, radius);
        }
    }
}