using System;
using UnityEngine;

namespace ElasticSea.Framework.Util.Callbacks
{
    public class OnRectTransformDimensionsChangeCallback : MonoBehaviour
    {
        public event Action OnRectTransformDimensionsChangeEvent = () => { };
        
        private void OnRectTransformDimensionsChange()
        {
            OnRectTransformDimensionsChangeEvent();
        }
    }
}