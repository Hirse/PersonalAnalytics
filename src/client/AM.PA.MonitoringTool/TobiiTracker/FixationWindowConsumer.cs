using System;
using System.Collections.Generic;
using TobiiTracker.Data;
using TobiiTracker.Helpers;

namespace TobiiTracker
{
    internal class FixationWindowConsumer : IStoppable
    {
        private const int MissingGazeTimeout = 5000;
        private const int VisualizationThreshold = 500;

        private readonly Dictionary<IntPtr, FixationWindowEntry> _lastFixationOfWindow;
        private readonly HighlighterOverlay _overlay;
        private FixationWindowEntry _lastFixation;

        public bool Stopped { get; private set; } = true;

        internal FixationWindowConsumer(HighlighterOverlay overlay)
        {
            _lastFixationOfWindow = new Dictionary<IntPtr, FixationWindowEntry>();
            _overlay = overlay;
        }

        internal void Collect(object sender, FixationWindowEntry currentFixation)
        {
            // Consumer stopped: Ignore all events
            if (Stopped) return;

            // Fixated Window is blacklisted - Ignore fixation
            if (WindowUtils.IsBlacklisted(currentFixation.ProcessName, currentFixation.WindowTitle)) return;

            // Fixation on the same window as the last
            if (currentFixation.WindowHandle == _lastFixation.WindowHandle)
            {
                // Hide overlay if still visible and fixation in highlight area
                _overlay.HideConditionally(currentFixation.X, currentFixation.Y);

                // If the last gaze on the same window was long ago, highlight
                var timeDiff = currentFixation.Time - _lastFixation.Time;
                if (timeDiff.TotalMilliseconds > MissingGazeTimeout)
                {
                    Visualize(_lastFixationOfWindow[currentFixation.WindowHandle]);
                }
            }
            // Fixation on different window
            else if (_lastFixationOfWindow.ContainsKey(currentFixation.WindowHandle))
            {
                // Only highlight if the last fixation on the current window is not too recent
                var timeDiff = currentFixation.Time - _lastFixationOfWindow[currentFixation.WindowHandle].Time;
                if (timeDiff.TotalMilliseconds > VisualizationThreshold)
                {
                    Visualize(_lastFixationOfWindow[currentFixation.WindowHandle]);
                }
            }

            _lastFixation = currentFixation;
            _lastFixationOfWindow[_lastFixation.WindowHandle] = currentFixation;
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

        private void Visualize(FixationWindowEntry fixationWindowEntry)
        {
            _overlay.Show(fixationWindowEntry.X, fixationWindowEntry.Y);
        }
    }
}
