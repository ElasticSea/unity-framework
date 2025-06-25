using ElasticSea.Framework.Extensions;
using UnityEditor;
using UnityEngine;

namespace ElasticSea.Framework.Util.Editor
{
    public static class MeshSaver
    {
        [MenuItem("CONTEXT/MeshFilter/Save Mesh To Asset")]
        private static void SaveMeshToAsset(MenuCommand command)
        {
            var meshFilter = (MeshFilter)command.context;
            Mesh mesh = meshFilter.sharedMesh;

            if (mesh == null)
            {
                Debug.LogWarning("MeshFilter has no mesh assigned.");
                return;
            }

            var meshName = !mesh.name.IsNullOrWhiteSpace() ? mesh.name : "mesh";
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Mesh Asset",
                $"{meshName}.asset",
                "asset",
                "Choose location to save the mesh asset");

            if (string.IsNullOrEmpty(path))
                return;

            // Create a copy to avoid editing the original mesh
            Mesh meshCopy = Object.Instantiate(mesh);
            AssetDatabase.CreateAsset(meshCopy, path);
            AssetDatabase.SaveAssets();

            Debug.Log($"Mesh saved to {path}");
        }
    }
}