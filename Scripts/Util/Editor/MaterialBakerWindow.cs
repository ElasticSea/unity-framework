using System.IO;
using UnityEditor;
using UnityEngine;

namespace ElasticSea.Framework.Util.Editor
{
    public class MaterialBakerWindow : EditorWindow
    {
        private Material sourceMaterial;
        private int textureWidth = 1024;
        private int textureHeight = 1024;
        private bool saveWithAlpha = true;

        [MenuItem("Tools/SDF Texture Baker")]
        public static void ShowWindow()
        {
            GetWindow<MaterialBakerWindow>("Material Baker");
        }

        private void OnGUI()
        {
            GUILayout.Label("Material to Texture Baker", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Now takes a Material instead of a Shader
            sourceMaterial = (Material)EditorGUILayout.ObjectField("Target Material", sourceMaterial, typeof(Material), false);

            textureWidth = EditorGUILayout.IntField("Width", textureWidth);
            textureHeight = EditorGUILayout.IntField("Height", textureHeight);
            saveWithAlpha = EditorGUILayout.Toggle("Include Alpha Channel", saveWithAlpha);

            EditorGUILayout.Space();

            GUI.enabled = sourceMaterial != null;
            if (GUILayout.Button("Bake to PNG", GUILayout.Height(40)))
            {
                BakeTexture();
            }
            GUI.enabled = true;

            if (sourceMaterial == null)
            {
                EditorGUILayout.HelpBox("Please assign a Material to bake.", MessageType.Warning);
            }
        }

        private void BakeTexture()
        {
            // Opens the native OS file explorer window
            string path = EditorUtility.SaveFilePanel(
                "Save Baked Texture",
                AssetDatabase.GetAssetPath(sourceMaterial), // Defaults to your Unity Assets folder
                "BakedSDF",
                "png"
            );

            // If the user hits 'Cancel' in the explorer, stop here
            if (string.IsNullOrEmpty(path)) return;

            // 1. Set up a temporary RenderTexture
            RenderTextureFormat rtFormat = saveWithAlpha ? RenderTextureFormat.ARGB32 : RenderTextureFormat.Default;
            RenderTexture rt = RenderTexture.GetTemporary(textureWidth, textureHeight, 0, rtFormat);
        
            // 2. Render the material directly into the RenderTexture
            Graphics.Blit(null, rt, sourceMaterial, 0);

            // 3. Read the RenderTexture into a standard Texture2D
            RenderTexture previousActive = RenderTexture.active;
            RenderTexture.active = rt;

            TextureFormat texFormat = saveWithAlpha ? TextureFormat.RGBA32 : TextureFormat.RGB24;
            Texture2D bakedTexture = new Texture2D(textureWidth, textureHeight, texFormat, false);
            bakedTexture.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
            bakedTexture.Apply();

            // 4. Encode and save to disk using standard OS paths
            byte[] bytes = bakedTexture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

            // 5. Cleanup memory
            RenderTexture.active = previousActive;
            RenderTexture.ReleaseTemporary(rt);
            DestroyImmediate(bakedTexture);

            // 6. If saved inside the project, tell Unity to refresh so it shows up immediately
            if (path.StartsWith(Application.dataPath))
            {
                AssetDatabase.Refresh();
            }

            Debug.Log($"Successfully baked material to texture at: {path}");
        }
    }
}