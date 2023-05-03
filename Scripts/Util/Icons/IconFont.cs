using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util.Icons
{
    [CreateAssetMenu(fileName = "Icon Font", menuName = "Icons/Icon Font")]
    public class IconFont : ScriptableObject
    {
        [SerializeField] private Font font;
        [SerializeField] private TextAsset codepoints;
        
        public Font Font => font;
        public string CodePoints => codepoints.text;
    }
}