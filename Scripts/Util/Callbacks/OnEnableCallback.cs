using System;
using UnityEngine;

namespace ElasticSea.Framework.Util.Callbacks
{
    public class OnEnableCallback : MonoBehaviour
    {
        public event Action OnEnabled;

        private void OnEnable()
        {
            OnEnabled?.Invoke();
        }
    }
}