using UnityEngine;

namespace _Framework.Scripts.Util
{
    public class ShowNormals : MonoBehaviour
    {
        [SerializeField] private float length = 1;
        [SerializeField] private MeshFilter mf;
        [SerializeField] private SkinnedMeshRenderer smr;
        [SerializeField] private Color startColor = Color.blue;
        [SerializeField] private Color endColor = Color.red;

        private void OnDrawGizmosSelected()
        {
            Mesh mesh = null;
            if (mf)
            {
                mesh = mf.mesh;
            }
            else if (smr)
            {
                mesh = smr.sharedMesh;
            }
            else if (GetComponent<MeshFilter>())
            {
                mesh = GetComponent<MeshFilter>().mesh;
            }
            else if (GetComponent<SkinnedMeshRenderer>())
            {
                mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
            }

            if (mesh != null)
            {
                var vertices = mesh.vertices;
                var normals = mesh.normals;

                for (var i = 0; i < normals.Length; i++)
                {
                    Gizmos.color = Color.Lerp(startColor, endColor, (float) i / (normals.Length - 1));

                    var worldPos = transform.TransformPoint(vertices[i]);
                    var worldNormal = transform.TransformVector(normals[i]);
                    Gizmos.DrawLine(worldPos, worldPos + worldNormal * length);
                }
            }
        }
    }
}