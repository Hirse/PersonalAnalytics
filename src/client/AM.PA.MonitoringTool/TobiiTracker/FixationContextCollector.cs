using System.Collections.Generic;
using TobiiTracker.Data;

namespace TobiiTracker
{
    internal class FixationContextCollector
    {
        private readonly List<FixationContextEntry> _fixationContextEntries;

        internal FixationContextCollector()
        {
            _fixationContextEntries = new List<FixationContextEntry>();
        }

        internal void Stop()
        {
            SaveConditionally(1);
        }

        internal void Collect(object sender, FixationContextEntry fixationContextEntry)
        {
            _fixationContextEntries.Add(fixationContextEntry);
            SaveConditionally(500);
        }

        private void SaveConditionally(int threshold)
        {
            if (_fixationContextEntries.Count >= threshold)
            {
                Queries.SaveFixationContextEntries(_fixationContextEntries);
                _fixationContextEntries.Clear();
            }
        }
    }
}
