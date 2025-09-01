using System.Linq;
using ElasticSea.Framework.Scripts.Extensions;
using Engine.Foundation;
using Instructions.Ui.BlockGrid.Layout.Packing;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    public class PackedSpatialLayout : ISpatialLayout
    {
        private PackResult bracketGroups;
        private int[] order;

        public int Count => bracketGroups.Items.Length;
        public int[] Order => order;

        public Bounds Bounds
        {
            get
            {
                var sizeX = bracketGroups.GridSize.x * bracketGroups.UnitSize.x;
                var sizeY = bracketGroups.GridSize.y * bracketGroups.UnitSize.y;
                return new Bounds(Vector3.zero, new Vector3(sizeX, sizeY, 0));
            }
        }

        public void Setup(Vector2[] sizes)
        {
            var bracketStep = new Vector2(Constants.SOCKET_SIZE / 2, Constants.SOCKET_SIZE / 2f);
            // bracketGroups = BlockPacker.PackToSmallestSquare(sizes, bracketStep);
            bracketGroups = BlockPacker2.PackByHeight(sizes, bracketStep);
            order = bracketGroups.Items.Select(i => i.Index).ToArray();
        }
        
        // private void OnDrawGizmos()
        // {
        //     SpatialLayoutUtils.DrawCells(this, Count, transform);
        // }

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