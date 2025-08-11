using TMPro;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util.Icons
{
    [CreateAssetMenu(fileName = "Icon Font", menuName = "Icons/Icon Font")]
    public class IconFont : ScriptableObject
    {
        [SerializeField] private Font font;
        [SerializeField] private TextAsset codepoints;
        [SerializeField] private TMP_FontAsset tmpFontAsset;
        
        public Font Font => font;
        public string CodePoints => codepoints.text;
        public TMP_FontAsset TmpFontAsset => tmpFontAsset;
    }
}