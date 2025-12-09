using System;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Icon.Lockable
{
    public class LockableIcon : MonoBehaviour
    {
        public string Id;
        public Collider Collider;
        public float Radius;
        public MeshRenderer Background;

        public event Action OnSelected;

        public void Trigger()
        {
            OnSelected?.Invoke();
        }
    }
}