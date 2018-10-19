using System.Collections.Generic;
using TobiiTracker.Data;

namespace TobiiTracker
{
    internal class FixationSummaryCollector
    {
        private readonly List<FixationSummaryEntry> _fixationSummaryEntries;

        internal FixationSummaryCollector()
        {
            _fixationSummaryEntries = new List<FixationSummaryEntry>();
        }

        internal void Stop()
        {
            SaveConditionally(1);
        }

        internal void Collect(object sender, FixationSummaryEntry fixationSummaryEntry)
        {
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
