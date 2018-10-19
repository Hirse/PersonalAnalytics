namespace TobiiTracker.Helpers
{
    internal struct Circle
    {
        private const double MultiplicativeEpsilon = 1 + 1e-14;

        internal Point Center;
        internal readonly double Radius;

        internal Circle(Point center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        internal bool Contains(Point point)
        {
            return Center.Distance(point) <= Radius * MultiplicativeEpsilon;
        }
    }
}
