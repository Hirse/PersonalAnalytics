using System;
using System.Collections.Generic;
using Tobii.Interaction;
using TobiiTracker.Data;
using TobiiTracker.Helpers;

namespace TobiiTracker
{
    internal class FixationSummaryProvider
    {
        private readonly Host _host;
        private DateTime _fixationStart;
        private readonly List<Point> _fixationPoints;

        internal event EventHandler<FixationSummaryEntry> FixationEnded;

        internal FixationSummaryProvider(Host host)
        {
            _host = host;
            _fixationPoints = new List<Point>();
        }

        internal void Start()
        {
            _host.Streams.CreateFixationDataStream().Begin(OnBegin);
            _host.Streams.CreateFixationDataStream().Data(OnData);
            _host.Streams.CreateFixationDataStream().End(OnEnd);
        }

        private void OnBegin(double x, double y, double timestamp)
        {
            _fixationStart = DateTime.UtcNow;
            _fixationPoints.Add(new Point(x, y));
        }

        private void OnData(double x, double y, double timestamp)
        {
            if (_fixationPoints.Count > 0)
            {
                _fixationPoints.Add(new Point(x, y));
            }
        }

        private void OnEnd(double x, double y, double timestamp)
        {
            if (_fixationPoints.Count > 0)
            {
                var circle = SmallestEnclosingCircle.MakeCircle(_fixationPoints);
                FixationEnded?.Invoke(this, new FixationSummaryEntry
                {
                    Cx = circle.Center.X,
                    Cy = circle.Center.Y,
                    Radius = circle.Radius,
                    StartTime = _fixationStart,
                    EndTime = DateTime.UtcNow
                });
            }
            _fixationPoints.Clear();
        }
    }
}
