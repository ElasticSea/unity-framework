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

            var dict = new Dictionary<string, List<Action<MeshFilter>>>();
            foreach (var (fname, modelname, action) in aa)
            {
                var mname = modelname.ToLowerInvariant();
                if (dict.ContainsKey(mname) == false)
                {
                    dict[mname] = new List<Action<MeshFilter>>();
                }
                
                dict[mname].Add(action);
            }
            
            foreach (var mf in gameObject.GetComponentsInChildren<MeshFilter>())
            {
                var mesh = mf.sharedMesh;

                if (dict.TryGetValue(mesh.name.ToLowerInvariant(), out var actions))
                {
                    foreach (var action in actions)
                    {
                        action(mf);
                    }
                }
            }
        }
    }
}