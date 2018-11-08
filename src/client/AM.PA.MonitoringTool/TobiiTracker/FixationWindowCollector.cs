using System.Collections.Generic;
using TobiiTracker.Data;
using TobiiTracker.Helpers;

namespace TobiiTracker
{
    internal class FixationWindowCollector : IStoppable
    {
        private readonly List<FixationWindowEntry> _fixationWindowEntries;

        public bool Stopped { get; private set; } = true;

        internal FixationWindowCollector()
        {
            _fixationWindowEntries = new List<FixationWindowEntry>();
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

        internal void Collect(object sender, FixationWindowEntry fixationWindowEntry)
        {
            if (Stopped) return;
            _fixationWindowEntries.Add(fixationWindowEntry);
            SaveConditionally(500);
        }

        private void SaveConditionally(int threshold)
        {
            if (_fixationWindowEntries.Count >= threshold)
            {
                Queries.SaveFixationWindowEntries(_fixationWindowEntries);
                _fixationWindowEntries.Clear();
            }
        }
    }
}
