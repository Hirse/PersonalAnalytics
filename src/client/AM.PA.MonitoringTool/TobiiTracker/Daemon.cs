using Shared;
using System.Collections.Generic;
using System.Reflection;
using Tobii.Interaction;
using TobiiTracker.Data;
using TobiiTracker.Helpers;

namespace TobiiTracker
{
    public class Daemon : BaseTrackerDisposable
    {
        private bool _disposed;
        private Host _host;
        private readonly List<IStoppable> _stoppables;

        public Daemon()
        {
            Name = "Tobii Tracker";
            _stoppables = new List<IStoppable>();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _host.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        public override void Start()
        {
            _host = new Host();
            _stoppables.Clear();

#pragma warning disable 162
            // ReSharper disable HeuristicUnreachableCode
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Settings.FixationContextEnabled)
            {
                var fixationContextProvider = new FixationContextProvider(_host);
                var fixationContextCollector = new FixationContextCollector();
                fixationContextProvider.FixationStarted += fixationContextCollector.Collect;
                _stoppables.Add(fixationContextProvider);
                _stoppables.Add(fixationContextCollector);
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Settings.FixationSummaryEnabled)
            {
                var fixationSummaryProvider = new FixationSummaryProvider(_host);
                var fixationSummaryCollector = new FixationSummaryCollector();
                fixationSummaryProvider.FixationEnded += fixationSummaryCollector.Collect;
                _stoppables.Add(fixationSummaryProvider);
                _stoppables.Add(fixationSummaryCollector);
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Settings.FixationWindowHighlightEnabled)
            {
                var fixationWindowProvider = new FixationWindowProvider(_host);
                var fixationWindowCollector = new FixationWindowCollector();
                var fixationWindowConsumer = new FixationWindowConsumer(new HighlighterOverlay());
                fixationWindowProvider.FixationStarted += fixationWindowCollector.Collect;
                fixationWindowProvider.FixationStarted += fixationWindowConsumer.Collect;
                _stoppables.Add(fixationWindowProvider);
                _stoppables.Add(fixationWindowCollector);
                _stoppables.Add(fixationWindowConsumer);
            }
            // ReSharper restore HeuristicUnreachableCode
#pragma warning restore 162

            foreach (var stoppable in _stoppables)
            {
                stoppable.Start();
            }

            IsRunning = true;
        }

        public override void Stop()
        {
            _host.DisableConnection();
            foreach (var stoppable in _stoppables)
            {
                stoppable.Stop();
            }
            IsRunning = false;
        }

        public override void CreateDatabaseTablesIfNotExist()
        {
            Queries.CreateTables();
        }

        public override void UpdateDatabaseTables(int version)
        {
            // First version. No update.
        }

        public override string GetVersion()
        {
            var version = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version;
            return Shared.Helpers.VersionHelper.GetFormattedVersion(version);
        }

        public override bool IsEnabled()
        {
            return Settings.Enabled;
        }
    }
}
