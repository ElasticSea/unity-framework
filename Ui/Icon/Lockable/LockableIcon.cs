using System;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Icon.Lockable
{
    public class LockableIcon : MonoBehaviour
    {
        public string Id;
        public Collider Collider;

        public LockableIcon(string id, Collider collider)
        {
            Id = id;
            Collider = collider;
        }

        public event Action OnSelected;

        public void Trigger()
        {
            OnSelected?.Invoke();
        }
    }
}