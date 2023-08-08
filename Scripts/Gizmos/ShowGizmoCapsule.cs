#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ElasticSea.Framework.Gizmo
{
    public class ShowGizmoCapsule : MonoBehaviour
    {
        public float Radius;
        public float Height;
        public Color Color;
        public bool OnDrawGizmosEnabled = true;
        public bool OnDrawGizmosSelectedEnabled = false;

        private void OnDrawGizmos()
        {
            if (OnDrawGizmosEnabled)
            {
                OnDraw();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (OnDrawGizmosSelectedEnabled)
            {
                OnDraw();
            }
        }

        private void OnDraw()
        {
            DrawWireCapsule(Radius, Height, Color);
        }
        
        public void DrawWireCapsule(float _radius, float _height, Color _color)
        {
#if UNITY_EDITOR
            Handles.color = _color;
            Handles.matrix = transform.localToWorldMatrix;
            
            var pointOffset = (_height - (_radius * 2)) / 2;

            //draw sideways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
            Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
            Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
            //draw frontways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
            Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
            Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
            //draw center
            Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
            Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);
#endif
        }
    }
}