using System;
using System.Collections.Generic;
using System.Timers;
using TobiiTracker.Data;
using TobiiTracker.Helpers;

namespace TobiiTracker
{
    internal class FixationWindowConsumer : IStoppable
    {
        private const int MissingGazeTimeout = 5000;

        private readonly Dictionary<IntPtr, Point> _lastFixationOfWindow;
        private readonly HighlighterOverlay _overlay;
        private readonly Timer _missingGazeTimer;

        private FixationWindowEntry _lastFixation;

        public bool Stopped { get; private set; } = true;

        internal FixationWindowConsumer(HighlighterOverlay overlay)
        {
            _lastFixationOfWindow = new Dictionary<IntPtr, Point>();
            _missingGazeTimer = new Timer(MissingGazeTimeout)
            {
                AutoReset = false
            };
            _missingGazeTimer.Elapsed += MissingGazeTimerOnElapsed;
            _overlay = overlay;
        }

        internal void Collect(object sender, FixationWindowEntry currentFixation)
        {
            if (Stopped) return;

            _missingGazeTimer.Stop();
            _missingGazeTimer.Start();

            if (WindowUtils.IsBlacklisted(currentFixation.ProcessName, currentFixation.WindowTitle)) return;

            if (currentFixation.WindowHandle == _lastFixation.WindowHandle)
            {
                _overlay.HideConditionally(currentFixation.X, currentFixation.Y);
            }
            else if (_lastFixationOfWindow.ContainsKey(currentFixation.WindowHandle))
            {
                Visualize(_lastFixationOfWindow[currentFixation.WindowHandle]);
            }

            _lastFixation = currentFixation;
            _lastFixationOfWindow[_lastFixation.WindowHandle] = new Point(currentFixation.X, currentFixation.Y);
        }

        public void Start()
        {
            Stopped = false;
            _overlay.Start();
        }

        public void Stop()
        {
            _overlay.Stop();
            _missingGazeTimer.Stop();
            Stopped = true;
        }

        private void Visualize(Point point)
        {
            _overlay.Show(point);
        }

        private void MissingGazeTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Visualize(_lastFixationOfWindow[_lastFixation.WindowHandle]);
        }
    }
}
