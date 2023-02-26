using UnityEngine;

namespace ElasticSea.Framework.Util.PropertyDrawers
{
    [System.Serializable]
    public class SingleUnityLayer
    {
        [SerializeField] private int m_LayerIndex = 0;

        public int LayerIndex => m_LayerIndex;

        public void Set(int _layerIndex)
        {
            if (_layerIndex > 0 && _layerIndex < 32)
            {
                m_LayerIndex = _layerIndex;
            }
        }

        public int Mask => 1 << m_LayerIndex;
    }
}