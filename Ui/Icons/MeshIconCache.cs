using System.Collections.Generic;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Icons
{
    public class MeshIconCache
    {
        private class VisualBoundsPair
        {
            public Mesh Visual;
            public Mesh Bounds;
        }

        private readonly Dictionary<string, VisualBoundsPair> meshes = new();

        public bool TryGetMesh(string id, out Mesh visual, out Mesh bounds)
        {
            if (meshes.TryGetValue(id, out var pair))
            {
                visual = pair.Visual;
                bounds = pair.Bounds;
                return true;
            }

            visual = null;
            bounds = null;
            return false;
        }

        public void PutMesh(string id, Mesh visual, Mesh bounds)
        {
            meshes[id] = new VisualBoundsPair { Visual = visual, Bounds = bounds };
        }
    }
}