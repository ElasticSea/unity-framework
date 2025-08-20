using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Placement
{
    public interface IPlacement
    {
        public int Count { get; set; }
        Vector3 Size { get; set; }
        Bounds Bounds { get; }

        (Matrix4x4 cellToLocal, Bounds bounds) GetCell(int index);
    }
}