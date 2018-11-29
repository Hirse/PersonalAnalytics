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

        internal void Collect(object sender, FixationWindowEntry currentFixation)
        {
            if (Stopped) return;

            if (WindowUtils.IsBlacklisted(currentFixation.ProcessName, currentFixation.WindowTitle)) return;
            if (WindowUtils.AreWindowsEqual(currentFixation, _lastFixation))
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
            Stopped = true;
        }

        private void Visualize(Point point)
        {
            _overlay.Show(point);
        }
    }
}
