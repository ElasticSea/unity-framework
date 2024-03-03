using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class ShowBounds : MonoBehaviour
    {
        [SerializeField] private Bounds bounds;
        [SerializeField] private Color color = Color.red;

        public Bounds Bounds
        {
            get => bounds;
            set => bounds = value;
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
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}