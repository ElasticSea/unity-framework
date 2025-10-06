using UnityEngine;

namespace ElasticSea.Framework.Ui.Icon
{
    public class Icon : MonoBehaviour
    {
        public string Id;
        public GameObject Backplate;
        public GameObject Frontplate;
        public Collider Collider;
        public MeshRenderer BackplateRenderer;
        public MeshRenderer FrontplateRenderer;
        public IconMeshData Data;
    }
}