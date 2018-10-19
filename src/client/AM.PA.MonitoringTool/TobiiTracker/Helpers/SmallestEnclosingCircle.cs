/*
 * Smallest enclosing circle - Library (Center#)
 * 
 * Copyright (c) 2017 Project Nayuki
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

namespace TobiiTracker.Helpers
{

    internal static class SmallestEnclosingCircle
    {

        /* 
         * Returns the smallest circle that encloses all the given points. Runs in expected O(n) time, randomized.
         * Note: If 0 points are given, a circle of radius -1 is returned. If 1 point is given, a circle of radius 0 is returned.
         */
        // Initially: No boundary points known
        internal static Circle MakeCircle(List<Point> points)
        {
            // Progressively add points to circle or recompute circle
            var circle = new Circle(new Point(0, 0), -1);
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                if (circle.Radius < 0 || !circle.Contains(point))
                {
                    circle = MakeCircleOnePoint(points.GetRange(0, i + 1), point);
                }
            }
            return circle;
        }

        // One boundary point known
        private static Circle MakeCircleOnePoint(List<Point> points, Point origin)
        {
            var circle = new Circle(origin, 0);
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                if (circle.Contains(point))
                {
                    continue;
                }

                circle = Math.Abs(circle.Radius) < 0.01 ? MakeDiameter(origin, point) : MakeCircleTwoPoints(points.GetRange(0, i + 1), origin, point);
            }
            return circle;
        }

        // Two boundary points known
        private static Circle MakeCircleTwoPoints(IEnumerable<Point> points, Point p, Point q)
        {
            var circle = MakeDiameter(p, q);
            var left = new Circle(new Point(0, 0), -1);
            var right = new Circle(new Point(0, 0), -1);

            // For each point not in the two-point circle
            var pq = q.Difference(p);
            foreach (var point in points)
            {
                if (circle.Contains(point))
                {
                    continue;
                }

                // Form a circumcircle and classify it on left or right side
                var cross = pq.Cross(point.Difference(p));
                var c = MakeCircumcircle(p, q, point);
                if (c.Radius < 0)
                {
                    continue;
                }

                if (cross > 0 && (left.Radius < 0 || pq.Cross(c.Center.Difference(p)) > pq.Cross(left.Center.Difference(p))))
                {
                    left = c;
                }
                else if (cross < 0 && (right.Radius < 0 || pq.Cross(c.Center.Difference(p)) < pq.Cross(right.Center.Difference(p))))
                {
                    right = c;
                }
            }

            // Select which circle to return
            if (left.Radius < 0 && right.Radius < 0)
            {
                return circle;
            }

            if (left.Radius < 0)
            {
                return right;
            }

            if (right.Radius < 0)
            {
                return left;
            }

            return left.Radius <= right.Radius ? left : right;
        }

        internal static Circle MakeDiameter(Point a, Point b)
        {
            var c = new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
            return new Circle(c, Math.Max(c.Distance(a), c.Distance(b)));
        }

        internal static Circle MakeCircumcircle(Point a, Point b, Point c)
        {
            // Mathematical algorithm from Wikipedia: Circumscribed circle
            var ox = (Math.Min(Math.Min(a.X, b.X), c.X) + Math.Max(Math.Min(a.X, b.X), c.X)) / 2;
            var oy = (Math.Min(Math.Min(a.Y, b.Y), c.Y) + Math.Max(Math.Min(a.Y, b.Y), c.Y)) / 2;
            double ax = a.X - ox, ay = a.Y - oy;
            double bx = b.X - ox, by = b.Y - oy;
            double cx = c.X - ox, cy = c.Y - oy;
            var d = (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by)) * 2;
            if (Math.Abs(d) < 0.01)
            {
                return new Circle(new Point(0, 0), -1);
            }

            var x = ((ax * ax + ay * ay) * (by - cy) + (bx * bx + by * by) * (cy - ay) + (cx * cx + cy * cy) * (ay - by)) / d;
            var y = ((ax * ax + ay * ay) * (cx - bx) + (bx * bx + by * by) * (ax - cx) + (cx * cx + cy * cy) * (bx - ax)) / d;
            var p = new Point(ox + x, oy + y);
            var r = Math.Max(Math.Max(p.Distance(a), p.Distance(b)), p.Distance(c));
            return new Circle(p, r);
        }
    }
}
