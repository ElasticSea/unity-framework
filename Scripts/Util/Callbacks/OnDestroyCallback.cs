using System;
using UnityEngine;

namespace _Framework.Scripts.Util.Callbacks
{
    public class OnDestroyCallback : MonoBehaviour
    {
        public event Action OnDestroyEvent = () => { };

        private void OnDestroy()
        {
            OnDestroyEvent();
            foreach(var d in OnDestroyEvent.GetInvocationList())
            {
                OnDestroyEvent -= (Action)d;
            }
        }
    }
}