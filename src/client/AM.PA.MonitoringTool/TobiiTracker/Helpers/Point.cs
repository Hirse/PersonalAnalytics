using System;

namespace TobiiTracker.Helpers
{
    internal struct Point
    {
        internal readonly double X;
        internal readonly double Y;

        internal Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        internal Point Difference(Point point)
        {
            return new Point(X - point.X, Y - point.Y);
        }

        internal double Distance(Point point)
        {
            var dx = X - point.X;
            var dy = Y - point.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Signed area / determinant thing
        internal double Cross(Point point)
        {
            return X * point.Y - Y * point.X;
        }
    }
}
