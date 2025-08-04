using System;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Layout;
using UnityEngine;
using static ElasticSea.Framework.Layout.Align;

namespace ElasticSea.Framework.Ui.Layout.Alignment
{
    public static class BoundsAlignUtils
    {
        public static (Vector3 position, Vector3 scale) Align(Bounds sourceBounds, Matrix4x4 target, Bounds targetBounds,
            Align horizontal, Align vertical, Align depth, ScaleAlignmentType scale = ScaleAlignmentType.ScaleDown,
            Align? oversizedHorizontal = null, Align? oversizedVertical = null,
            Align? oversizedDepth = null)
        {
            // Scale
            var finalScale = Vector3.one;
            var minScale = targetBounds.size.Divide(sourceBounds.size).Min();
            switch (scale)
            {
                case ScaleAlignmentType.None:
                    if (sourceBounds.size.x > targetBounds.size.x) horizontal = oversizedHorizontal ?? horizontal;
                    if (sourceBounds.size.y > targetBounds.size.y) vertical = oversizedVertical ?? vertical;
                    if (sourceBounds.size.z > targetBounds.size.z) depth = oversizedDepth ?? depth;
                    break;
                case ScaleAlignmentType.Fill:
                    finalScale *= minScale;
                    break;
                case ScaleAlignmentType.ScaleDown:
                    if (minScale < 1)
                    {
                        finalScale *= minScale;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scale), scale, null);
            }

            // Bounds
            var scaledBounds = new Bounds(sourceBounds.center.Multiply(finalScale), sourceBounds.size.Multiply(finalScale));

            float GetAlignment(Align alignment, float sourceMin, float sourceMax, float targetMin, float targetMax)
            {
                switch (alignment)
                {
                    case BeforeStart:
                        return targetMin - sourceMax;
                    case Start:
                        return targetMin - sourceMin;
                    case Center:
                        return (targetMax + targetMin - sourceMax - sourceMin) / 2;
                    case End:
                        return targetMax - sourceMax;
                    case AfterEnd:
                        return targetMax - sourceMin;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
                }
            }

            // Position
            var localPosition = new Vector3();
            localPosition.x = GetAlignment(horizontal, scaledBounds.min.x, scaledBounds.max.x, targetBounds.min.x, targetBounds.max.x);
            localPosition.y = GetAlignment(vertical, scaledBounds.min.y, scaledBounds.max.y, targetBounds.min.y, targetBounds.max.y);
            localPosition.z = GetAlignment(depth, scaledBounds.min.z, scaledBounds.max.z, targetBounds.min.z, targetBounds.max.z);

            var worldPosition = target.MultiplyPoint(localPosition);
            return (worldPosition, finalScale);
        }
    }
}