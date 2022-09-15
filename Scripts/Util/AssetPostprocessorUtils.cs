using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ElasticSea.Framework.Extensions;
using UnityEditor;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util
{
    public class AssetPostprocessorUtils
    {
        public static void ProcessMeshes(AssetPostprocessor assetPostprocessor, GameObject gameObject, params (string filename, string modelname, Action<MeshFilter> action)[] definitions)
        {
            var aa = definitions.ToArray();

            if (aa.Length == 0)
            {
                return;
            }

            var importer = (ModelImporter) assetPostprocessor.assetImporter;
            var filename = Path.GetFileNameWithoutExtension(importer.assetPath);

            var filenames = aa.Select(a => a.filename).ToSet();

            if (filenames.Contains(filename, StringComparer.InvariantCultureIgnoreCase) == false)
            {
                return;
            }

            var ff = aa.ToDictionary(a => a.modelname.ToLowerInvariant(), a => a.action);
            
            foreach (var mf in gameObject.GetComponentsInChildren<MeshFilter>())
            {
                var mesh = mf.sharedMesh;

                if (ff.TryGetValue(mesh.name.ToLowerInvariant(), out var action))
                {
                    action(mf);
                }
            }
        }
    }
}