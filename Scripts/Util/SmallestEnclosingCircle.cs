using System.Linq;
using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util
{
    /* 
 * Smallest enclosing circle - Library (C#)
 * 
 * Copyright (c) 2020 Project Nayuki
 * https://www.nayuki.io/page/smallest-enclosing-circle
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program (see COPYING.txt and COPYING.LESSER.txt).
 * If not, see <http://www.gnu.org/licenses/>.
 */

    using System;
    using System.Collections.Generic;


    public sealed class SmallestEnclosingCircle
    {
        /* 
         * Returns the smallest circle that encloses all the given Vector2s. Runs in expected O(n) time, randomized.
         * Note: If 0 Vector2s are given, a circle of radius -1 is returned. If 1 Vector2 is given, a circle of radius 0 is returned.
         */
        // Initially: No boundary Vector2s known
        public static Circle MakeCircle(Vector2[] points)
        {
            // Clone list to preserve the caller's data, do Durstenfeld shuffle

            points = points.CloneArray();
            points.ShuffleFastInPlace();

            // Progressively add Vector2s to circle or recompute circle
            Circle c = Circle.INVALID;
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 p = points[i];
                if (c.r < 0 || !c.Contains(p))
                {
                    var range = new Vector2[i + 1];
                    Array.Copy(points, range, range.Length);
                    c = MakeCircleOneVector2(range, p);
                }
            }

            return c;
        }


        // One boundary Vector2 known
        private static Circle MakeCircleOneVector2(Vector2[] Vector2s, Vector2 p)
        {
            Circle c = new Circle(p, 0);
            for (int i = 0; i < Vector2s.Length; i++)
            {
                Vector2 q = Vector2s[i];
                if (!c.Contains(q))
                {
                    if (c.r == 0)
                    {
                        c = MakeDiameter(p, q);
                    }
                    else
                    {
                        var range = new Vector2[i + 1];
                        Array.Copy(Vector2s, range, range.Length);
                        c = MakeCircleTwoVector2s(range, p, q);
                    }
                }
            }

            return c;
        }


        // Two boundary Vector2s known
        private static Circle MakeCircleTwoVector2s(Vector2[] Vector2s, Vector2 p, Vector2 q)
        {
            Circle circ = MakeDiameter(p, q);
            Circle left = Circle.INVALID;
            Circle right = Circle.INVALID;

            // For each Vector2 not in the two-Vector2 circle
            Vector2 pq = q.Subtract(p);
            for (var i = 0; i < Vector2s.Length; i++)
            {
                var r = Vector2s[i];
                if (circ.Contains(r))
                    continue;

                // Form a circumcircle and classify it on left or right side
                double cross = Cross(pq, r.Subtract(p));
                Circle c = MakeCircumcircle(p, q, r);
                if (c.r < 0)
                {
                    continue;
                }
                else if (cross > 0 && (left.r < 0 || Cross(pq, c.c.Subtract(p)) > Cross(pq, left.c.Subtract(p))))
                {
                    left = c;
                }
                else if (cross < 0 && (right.r < 0 || Cross(pq, c.c.Subtract(p)) < Cross(pq, right.c.Subtract(p))))
                {
                    right = c;
                }
            }

            // Select which circle to return
            if (left.r < 0 && right.r < 0)
                return circ;
            else if (left.r < 0)
                return right;
            else if (right.r < 0)
                return left;
            else
                return left.r <= right.r ? left : right;
        }

        private static float Cross(Vector2 value1, Vector2 value2)
        {
            return value1.x * value2.y - value1.y * value2.x;
        }


        private static Circle MakeDiameter(Vector2 a, Vector2 b)
        {
            Vector2 c = new Vector2((a.x + b.x) / 2, (a.y + b.y) / 2);
            return new Circle(c, Math.Max(c.Distance(a), c.Distance(b)));
        }


        private static Circle MakeCircumcircle(Vector2 a, Vector2 b, Vector2 c)
        {
            // Mathematical algorithm from Wikipedia: Circumscribed circle
            double ox = (Math.Min(Math.Min(a.x, b.x), c.x) + Math.Max(Math.Max(a.x, b.x), c.x)) / 2;
            double oy = (Math.Min(Math.Min(a.y, b.y), c.y) + Math.Max(Math.Max(a.y, b.y), c.y)) / 2;
            double ax = a.x - ox, ay = a.y - oy;
            double bx = b.x - ox, by = b.y - oy;
            double cx = c.x - ox, cy = c.y - oy;
            double d = (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by)) * 2;
            if (d == 0)
                return Circle.INVALID;
            double x = ((ax * ax + ay * ay) * (by - cy) + (bx * bx + by * by) * (cy - ay) + (cx * cx + cy * cy) * (ay - by)) / d;
            double y = ((ax * ax + ay * ay) * (cx - bx) + (bx * bx + by * by) * (ax - cx) + (cx * cx + cy * cy) * (bx - ax)) / d;
            Vector2 p = new Vector2((float)(ox + x), (float)(oy + y));
            double r = Math.Max(Math.Max(p.Distance(a), p.Distance(b)), p.Distance(c));
            return new Circle(p, r);
        }
    }


    public struct Circle
    {
        public static readonly Circle INVALID = new Circle(new Vector2(0, 0), -1);

        private const double MULTIPLICATIVE_EPSILON = 1 + 1e-14;


        public Vector2 c; // Center
        public double r; // Radius


        public Circle(Vector2 c, double r)
        {
            this.c = c;
            this.r = r;
        }


        public bool Contains(Vector2 p)
        {
            return c.Distance(p) <= r * MULTIPLICATIVE_EPSILON;
        }


        public bool Contains(ICollection<Vector2> ps)
        {
            foreach (Vector2 p in ps)
            {
                if (!Contains(p))
                    return false;
            }

            return true;
        }
    }
}