#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

namespace ElasticSea.Framework.Util
{
    public class ForceReserializeAssetsUtils
    {
        [MenuItem("Assets/Force Reserialize Assets")]
        public static void ForceReserializeAssets()
        {
            var paths = Selection.assetGUIDs.Select(guid => AssetDatabase.GUIDToAssetPath(guid));
            AssetDatabase.ForceReserializeAssets(paths);
        }
    }
}
#endif