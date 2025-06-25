using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class BoundingBox : MonoBehaviour
    {
        public Vector3 Center = Vector3.zero;
        [Min(0f)] public Vector3 Size = Vector3.one;

        [Header("Gizmos")]
        [SerializeField] Color color = Color.yellow;
        [SerializeField] bool drawWhenNotSelected = false;

        private void OnDrawGizmos()
        {
            if (drawWhenNotSelected) DrawGizmo();
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!drawWhenNotSelected) DrawGizmo();
        }

        public Bounds LocalBounds
        {
            get => new(Center, Size);
            set
            {
                Size = value.size;
                Center = value.center;
            }
        }

        public Bounds WorldBounds
        {
            get
            {
                var worldCenter = transform.TransformPoint(Center);
                var worldSize = Vector3.Scale(Size, transform.lossyScale);
                return new Bounds(worldCenter, worldSize);
            }
        }

        private void DrawGizmo()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = color;
            Gizmos.DrawWireCube(Center, Size);
        }
    }
}