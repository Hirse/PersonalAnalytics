/* 
 * Smallest enclosing circle - Test suite (Center#)
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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TobiiTracker.Helpers;

namespace TobiiTrackerTests
{
    [TestClass]
    public class SmallestEnclosingCircleTest
    {
        private const double Epsilon = 1e-12;
        private static readonly Random Rand = new Random();

        [TestMethod]
        public void TestBoundary()
        {
            Assert.AreEqual(-1, SmallestEnclosingCircle.MakeCircle(new List<Point>()).Radius);
            Assert.AreEqual(0, SmallestEnclosingCircle.MakeCircle(new List<Point> { new Point(0, 0) }).Radius);
            Assert.AreEqual(0, SmallestEnclosingCircle.MakeCircle(new List<Point> { new Point(1, 1), new Point(1, 1) }).Radius);
        }

        [TestMethod]
        public void TestMatchingNaiveAlgorithm()
        {
            const int trials = 1000;
            for (var i = 0; i < trials; i++)
            {
                var points = MakeRandomPoints(Rand.Next(30) + 1);
                var reference = SmallestEnclosingCircleNaive(points);
                var actual = SmallestEnclosingCircle.MakeCircle(points);
                Assert.AreEqual(reference.Center.X, actual.Center.X, Epsilon);
                Assert.AreEqual(reference.Center.X, actual.Center.X, Epsilon);
                Assert.AreEqual(reference.Center.Y, actual.Center.Y, Epsilon);
                Assert.AreEqual(reference.Radius, actual.Radius, Epsilon);
            }
        }

        [TestMethod]
        public void TestTranslation()
        {
            const int trials = 100;
            const int checks = 10;
            for (var i = 0; i < trials; i++)
            {
                var points = MakeRandomPoints(Rand.Next(300) + 1);
                var reference = SmallestEnclosingCircle.MakeCircle(points);

                for (var j = 0; j < checks; j++)
                {
                    var dx = NextGaussian();
                    var dy = NextGaussian();
                    var newPoints = new List<Point>();
                    foreach (var p in points)
                        newPoints.Add(new Point(p.X + dx, p.Y + dy));

                    var translated = SmallestEnclosingCircle.MakeCircle(newPoints);
                    Assert.AreEqual(reference.Center.X + dx, translated.Center.X, Epsilon);
                    Assert.AreEqual(reference.Center.Y + dy, translated.Center.Y, Epsilon);
                    Assert.AreEqual(reference.Radius, translated.Radius, Epsilon);
                }
            }
        }

        #region Helper functions

        private static List<Point> MakeRandomPoints(int n)
        {
            var result = new List<Point>();
            if (Rand.NextDouble() < 0.2)
            {  // Discrete lattice (to have a chance of duplicated points)
                for (var i = 0; i < n; i++)
                    result.Add(new Point(Rand.Next(10), Rand.Next(10)));
            }
            else
            {  // Gaussian distribution
                for (var i = 0; i < n; i++)
                    result.Add(new Point(NextGaussian(), NextGaussian()));
            }
            return result;
        }

        // Returns the smallest enclosing circle in O(n^4) time using the naive algorithm.
        private static Circle SmallestEnclosingCircleNaive(List<Point> points)
        {
            // Degenerate cases
            if (points.Count == 0)
                return new Circle(new Point(0, 0), -1);
            if (points.Count == 1)
                return new Circle(points[0], 0);

            // Try all unique pairs
            var result = new Circle(new Point(0, 0), -1);
            for (var i = 0; i < points.Count; i++)
            {
                for (var j = i + 1; j < points.Count; j++)
                {
                    var c = SmallestEnclosingCircle.MakeDiameter(points[i], points[j]);
                    if ((result.Radius < 0 || c.Radius < result.Radius) && points.All(p => c.Contains(p)))
                        result = c;
                }
            }
            if (result.Radius >= 0)
                return result;  // This optimization is not mathematically proven

            // Try all unique triples
            for (var i = 0; i < points.Count; i++)
            {
                for (var j = i + 1; j < points.Count; j++)
                {
                    for (var k = j + 1; k < points.Count; k++)
                    {
                        var c = SmallestEnclosingCircle.MakeCircumcircle(points[i], points[j], points[k]);
                        if (c.Radius >= 0 && (result.Radius < 0 || c.Radius < result.Radius) && points.All(p => c.Contains(p)))
                            result = c;
                    }
                }
            }
            if (result.Radius < 0)
                throw new SystemException("Assertion error");
            return result;
        }

        private static double NextGaussian()
        {
            return Math.Sqrt(-2 * Math.Log(Rand.NextDouble())) * Math.Cos(Rand.NextDouble() * Math.PI * 2);
        }

        #endregion
    }
}
