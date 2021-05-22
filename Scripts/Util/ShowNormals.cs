using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class ShowNormals : MonoBehaviour
    {
        [SerializeField] private int length = 1;
        [SerializeField] private int limit = -1;
        
        [SerializeField] private MeshFilter mf;
        [SerializeField] private SkinnedMeshRenderer smr;
        [SerializeField] private Color startColor = Color.blue;
        [SerializeField] private Color endColor = Color.red;

        private void OnDrawGizmosSelected()
        {
            Mesh mesh = null;
            if (mf)
            {
                mesh = Application.isPlaying ? mf.mesh : mf.sharedMesh;
            }
            else if (smr)
            {
                mesh = smr.sharedMesh;
            }
            else if (GetComponent<MeshFilter>())
            {
                mesh = Application.isPlaying ? GetComponent<MeshFilter>().mesh : GetComponent<MeshFilter>().sharedMesh;
            }
            else if (GetComponent<SkinnedMeshRenderer>())
            {
                mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
            }

            if (mesh != null)
            {
                var vertices = mesh.vertices;
                var normals = mesh.normals;

                var len = limit >= 0 ? Mathf.Min(limit, normals.Length) : normals.Length;

                for (var i = 0; i < len; i++)
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