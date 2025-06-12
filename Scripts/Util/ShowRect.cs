using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class ShowRect : MonoBehaviour
    {
        [SerializeField] private Rect rect;
        [SerializeField] private Color color = Color.red;

        public Rect Rect
        {
            get => rect;
            set => rect = value;
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
            Gizmos.DrawWireCube(rect.center, rect.size);
        }
    }
}