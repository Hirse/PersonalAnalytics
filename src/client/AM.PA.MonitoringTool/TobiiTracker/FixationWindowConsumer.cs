using System;
using System.Collections.Generic;
using TobiiTracker.Data;
using TobiiTracker.Helpers;

namespace TobiiTracker
{
    internal class FixationWindowConsumer : IStoppable
    {
        private FixationWindowEntry _lastFixation;
        private readonly Dictionary<IntPtr, Point> _lastFixationOfWindow;
        private readonly HighlighterOverlay _overlay;

        public bool Stopped { get; private set; } = true;

        internal FixationWindowConsumer(HighlighterOverlay overlay)
        {
            _lastFixationOfWindow = new Dictionary<IntPtr, Point>();
            _overlay = overlay;
        }

        internal void Collect(object sender, FixationWindowEntry fixationWindowEntry)
        {
            if (Stopped) return;

            if (WindowBlacklist.IsBlacklisted(fixationWindowEntry.ProcessName, fixationWindowEntry.WindowTitle)) return;

            if (fixationWindowEntry.WindowHandle != _lastFixation.WindowHandle && _lastFixationOfWindow.ContainsKey(fixationWindowEntry.WindowHandle))
            {
                Visualize(_lastFixationOfWindow[fixationWindowEntry.WindowHandle]);
            }

            _lastFixation = fixationWindowEntry;
            _lastFixationOfWindow[_lastFixation.WindowHandle] = new Point(fixationWindowEntry.X, fixationWindowEntry.Y);
        }

        public void Start()
        {
            Stopped = false;
            _overlay.Start();
        }

        public void Stop()
        {
            _overlay.Stop();
            Stopped = true;
        }

        private void Visualize(Point point)
        {
            _overlay.Show(point);
        }
    }
}
