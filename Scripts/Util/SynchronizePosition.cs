using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util
{
    public class SynchronizePosition : MonoBehaviour
    {
        private Transform target;
        private Vector3 offset;

        public Vector3 Offset
        {
            get => offset;
            set
            {
                offset = value;
                if (target)
                {
                    Update();
                }
            }
        }

        public Transform Target
        {
            get => target;
            set
            {
                target = value;
                Update();
            }
        }

        public void Update()
        {
            transform.position = target.position + offset;
        }
    }
}