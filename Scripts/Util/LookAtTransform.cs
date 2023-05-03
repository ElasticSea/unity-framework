using UnityEngine;
using UnityEngine.Serialization;

namespace ElasticSea.Framework.Scripts.Util
{
    public class LookAtTransform : MonoBehaviour
    {
        public Transform target;

        public Transform Target
        {
            get => target;
            set
            {
                target = value;
                Update();
            }
        }

        private void Update()
        {
            transform.rotation = Quaternion.LookRotation(target.forward, target.up);
        }
    }
}