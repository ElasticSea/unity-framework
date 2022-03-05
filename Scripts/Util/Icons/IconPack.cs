using System.Collections.Generic;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util.Icons
{
    public abstract class IconPack : MonoBehaviour
    {
        [SerializeField] private Font font;
        public abstract Dictionary<string, int> Icons { get; }

        public Font Font
        {
            get { return font; }
            set { font = value; }
        }
    }
}