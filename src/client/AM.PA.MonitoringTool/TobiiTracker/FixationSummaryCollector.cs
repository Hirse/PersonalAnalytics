using System.Collections.Generic;
using TobiiTracker.Data;
using TobiiTracker.Helpers;

namespace TobiiTracker
{
    internal class FixationSummaryCollector : IStoppable
    {
        private readonly List<FixationSummaryEntry> _fixationSummaryEntries;

        public bool Stopped { get; private set; } = true;

        internal FixationSummaryCollector()
        {
            _fixationSummaryEntries = new List<FixationSummaryEntry>();
        }

        public void Start()
        {
            Stopped = false;
        }

        public void Stop()
        {
            Stopped = true;
            SaveConditionally(1);
        }

        internal void Collect(object sender, FixationSummaryEntry fixationSummaryEntry)
        {
            if (Stopped) return;
            _fixationSummaryEntries.Add(fixationSummaryEntry);
            SaveConditionally(500);
        }

        private void SaveConditionally(int threshold)
        {
            if (_fixationSummaryEntries.Count >= threshold)
            {
                Queries.SaveFixationSummaryEntries(_fixationSummaryEntries);
                _fixationSummaryEntries.Clear();
            }
        }
    }
}
