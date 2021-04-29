using UnityEngine;

namespace ElasticSea.Framework.Extensions
{
    public static class FloatExtensions
    {
        public static float RoundTo(this float value, float roundTo)
        {
            return (float)(Mathf.RoundToInt((value) / roundTo) * (double)roundTo);
        }
        
        public static float FloorTo(this float value, float roundTo)
        {
            return (float)(Mathf.FloorToInt((value) / roundTo) * (double)roundTo);
        }
    }
}