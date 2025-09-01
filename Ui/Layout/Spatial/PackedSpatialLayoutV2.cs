using ElasticSea.Framework.Scripts.Extensions;
using Engine.Foundation;
using Instructions.Ui.BlockGrid.Layout.Packing;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    public class PackedSpatialLayoutV2 : ISpatialLayout
    {
        private PackResult bracketGroups;

        public int Count => bracketGroups.Items.Length;

        public Bounds Bounds
        {
            get
            {
                var sizeX = bracketGroups.GridSize.x * bracketGroups.UnitSize.x;
                var sizeY = bracketGroups.GridSize.y * bracketGroups.UnitSize.y;
                
                var size = new Vector3(sizeX, sizeY, 0);
                var center = new Vector3(sizeX * 0.5f, sizeY * 0.5f, 0f);
                return new Bounds(center, size);
            }
        }

        public void Setup(Vector2[] sizes)
        {
            var bracketStep = new Vector2(Constants.SOCKET_SIZE / 2, Constants.SOCKET_SIZE / 2f);
            bracketGroups = BlockPacker.PackToSmallestSquare(sizes, bracketStep);
        }

        public SpatialCell GetCell(int index)
        {
            var cell = bracketGroups.Items[index];
            var rect = cell.Rect.ToRect();
            var center = rect.center * bracketGroups.UnitSize;
            var size = rect.size * bracketGroups.UnitSize;
            
            var trs = Matrix4x4.TRS(center, Quaternion.identity, Vector3.one);
            var bounds = new Bounds(Vector3.zero, size);

            return new (trs, bounds);
        }
    }
}