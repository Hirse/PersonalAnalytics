using System;
using System.Collections.Generic;
using Tobii.Interaction;
using TobiiTracker.Data;
using TobiiTracker.Helpers;

namespace TobiiTracker
{
    internal class FixationSummaryProvider : IStoppable
    {
        private readonly Host _host;
        private DateTime _fixationStart;
        private readonly List<Point> _fixationPoints;

        public bool Stopped { get; private set; } = true;

        internal event EventHandler<FixationSummaryEntry> FixationEnded;

        internal FixationSummaryProvider(Host host)
        {
            _host = host;
            _fixationPoints = new List<Point>();
        }

        public void Start()
        {
            Stopped = false;
            _host.Streams.CreateFixationDataStream().Begin(OnBegin);
            _host.Streams.CreateFixationDataStream().Data(OnData);
            _host.Streams.CreateFixationDataStream().End(OnEnd);
        }

        public void Stop()
        {
            Stopped = true;
        }

        private void OnBegin(double x, double y, double timestamp)
        {
            if (Stopped) return;
            _fixationStart = DateTime.UtcNow;
            _fixationPoints.Add(new Point(x, y));
        }

        private void OnData(double x, double y, double timestamp)
        {
            if (Stopped) return;
            if (_fixationPoints.Count > 0)
            {
                _fixationPoints.Add(new Point(x, y));
            }
        }

        private void OnEnd(double x, double y, double timestamp)
        {
            if (Stopped) return;
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
