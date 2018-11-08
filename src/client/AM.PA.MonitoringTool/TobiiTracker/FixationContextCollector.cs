using System.Collections.Generic;
using TobiiTracker.Data;
using TobiiTracker.Helpers;

namespace TobiiTracker
{
    internal class FixationContextCollector : IStoppable
    {
        private readonly List<FixationContextEntry> _fixationContextEntries;

        public bool Stopped { get; private set; } = true;

        internal FixationContextCollector()
        {
            _fixationContextEntries = new List<FixationContextEntry>();
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

        internal void Collect(object sender, FixationContextEntry fixationContextEntry)
        {
            if (Stopped) return;
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
