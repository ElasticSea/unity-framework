using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util.Icons
{
    public class GoogleIconPack : IconPack
    {
        [System.Serializable]
        private class Config
        {
            [System.Serializable]
            internal class Category
            {
                [System.Serializable]
                internal class Icon
                {
                    public string name;
                    public string codepoint;
                }

                public List<Icon> icons;
            }

            public List<Category> categories;
        }
        
        [SerializeField] private TextAsset config;

        public override Dictionary<string, int> Icons => JsonUtility
            .FromJson<Config>(config.text)
            .categories.SelectMany(c => c.icons)
            .ToDictionary(g => g.name, g => int.Parse(g.codepoint, NumberStyles.HexNumber));
    }
}