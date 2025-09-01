using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    /// <summary>
    /// Aggregates multiple GroupedSpatialGrid instances stacked vertically (one above another)
    /// into a single ISpatialGrid. All children share the same Spacing (overrides child Spacing),
    /// but each child keeps its own Rows, Columns, Size, and Count.
    /// </summary>
    public class StackedSpatialGridsLayout : ISpatialLayout
    {
        private readonly int[] _childCounts;          // Count per child
        private readonly float[] _childHeights;       // Visual height per child grid
        private readonly float[] _childWidths;        // Visual width per child grid
        private readonly float[] _childYOffset;       // Bottom Y offset (from overall bottom) where child starts
        private readonly int[] _startIndex;           // Global start index for each child (prefix sum)
        private readonly int _count;                  // Global count
        private readonly float _totalHeight;          // Sum of child heights + inter-child spacings
        private readonly float _maxWidth;             // Max child width
        private readonly float _maxZ;                 // Max Size.z among children
        private Bounds _bounds;
        
        public Vector2 Spacing { get; }
        public IReadOnlyList<SpatialGridInput> Children { get; }
        public int Count => _count;
        public Bounds Bounds => _bounds;

        public StackedSpatialGridsLayout(SpatialGridInput[] children, Vector2 spacing)
        {
            if (children == null || children.Length == 0)
                throw new ArgumentException("Children cannot be null or empty.", nameof(children));

            Children = children;
            Spacing = spacing;

            int n = children.Length;
            _childCounts  = new int[n];
            _childHeights = new float[n];
            _childWidths  = new float[n];
            _childYOffset = new float[n];
            _startIndex   = new int[n];

            _maxWidth = 0f;
            _totalHeight = 0f;
            _count = 0;
            _maxZ = 0f;

            // 1) Measure each child with the shared Spacing
            for (int i = 0; i < n; i++)
            {
                var g = children[i];

                // Count
                int count = g.Count;
                _childCounts[i] = count;
                _count += count;

                // Width/Height computed like in GroupedSpatialGrid.Bounds but with shared Spacing
                float w = g.Size.x * g.Columns + Spacing.x * (g.Columns - 1);
                float h = g.Size.y * g.Rows    + Spacing.y * (g.Rows    - 1);

                _childWidths[i]  = Mathf.Max(0f, w);
                _childHeights[i] = Mathf.Max(0f, h);

                if (w > _maxWidth) _maxWidth = w;
                if (g.Size.z > _maxZ) _maxZ = g.Size.z;
            }

            // 2) Compute vertical stacking offsets (bottom anchored).
            // Between two adjacent children we add one vertical Spacing.y (as a "row" gap between groups).
            float y = 0f;
            for (int i = 0; i < n; i++)
            {
                _childYOffset[i] = y;
                y += _childHeights[i];
                if (i < n - 1) y += Spacing.y; // gap between groups
            }
            _totalHeight = y;

            // 3) Compute global start indices (prefix sums)
            int acc = 0;
            for (int i = 0; i < n; i++)
            {
                _startIndex[i] = acc;
                acc += _childCounts[i];
            }

            // 4) Overall bounds: centered at (0,0, _maxZ/2), like your original grid class
            _bounds = new Bounds(
                new Vector3(0f, 0f, _maxZ / 2f),
                new Vector3(_maxWidth, _totalHeight, Mathf.Max(_maxZ, 0f))
            );
        }

        public SpatialCell GetCell(int index)
        {
            if ((uint)index >= (uint)_count)
                throw new IndexOutOfRangeException($"Index {index} out of range [0, {_count}).");

            // 1) Find which child this global index belongs to (linear scan is fine for few groups; binary search if many)
            int childIdx = FindChildForIndex(index);
            var child = Children[childIdx];
            int localIndex = index - _startIndex[childIdx];

            // 2) Convert local index → (x,y) within that child
            int cols = child.Columns;
            int x = localIndex % cols;
            int y = localIndex / cols;

            // 3) Compute local offsets using child's Size and shared Spacing
            Vector3 cellSize = child.Size; // preserves per-child cell size, including Z
            float xpos = x * (cellSize.x + Spacing.x);
            float ypos = y * (cellSize.y + Spacing.y);

            // 4) Bottom Y of this child within the aggregate
            float yBase = _childYOffset[childIdx];

            // 5) Cell bounds in its own local space (centered like original)
            var cellBounds = new Bounds(cellSize / 2f, cellSize);

            // 6) Place cell in world: anchor at overall Bounds.min, add child Y offset and local (x,y)
            var origin = _bounds.min; // left-bottom-front
            var cellPos = origin + new Vector3(xpos, yBase + ypos, 0f);

            var cellMatrix = Matrix4x4.TRS(cellPos, Quaternion.identity, Vector3.one);
            return new SpatialCell(cellMatrix, cellBounds);
        }

        private int FindChildForIndex(int globalIndex)
        {
            // Linear search; switch to binary if you expect many groups.
            // _startIndex[i] <= globalIndex < _startIndex[i] + _childCounts[i]
            for (int i = 0; i < _startIndex.Length; i++)
            {
                int start = _startIndex[i];
                int count = _childCounts[i];
                if (globalIndex >= start && globalIndex < start + count)
                    return i;
            }
            // Should never happen due to index check
            return _startIndex.Length - 1;
        }
    }
}
