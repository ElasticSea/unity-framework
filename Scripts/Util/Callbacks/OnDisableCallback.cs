using System;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util.Callbacks
{
    public class OnDisableCallback : MonoBehaviour
    {
        public event Action OnDisableEvent = () => { };

        private void OnDisable()
        {
            OnDisableEvent();
        }
    }
}