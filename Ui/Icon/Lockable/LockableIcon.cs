using System;
using UnityEngine;
using Util;

namespace ElasticSea.Framework.Ui.Icon.Lockable
{
    public class LockableIcon : MonoBehaviour
    {
        public string Id;
        public Collider Collider;
        public float Radius;
        public MeshRenderer Background;
        public bool IsLocked => IsLockedProperty.Value;
        public IProperty<bool> IsLockedProperty = new ValueProperty<bool>();

        public event Action OnSelected;
        public event Action<bool> OnLockChanged;

        public void Trigger()
        {
            OnSelected?.Invoke();
        }
    }
}