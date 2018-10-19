using Shared;
using System.Reflection;
using Tobii.Interaction;
using TobiiTracker.Data;

namespace TobiiTracker
{
    public class Daemon : BaseTrackerDisposable
    {
        private bool _disposed;
        private Host _host;
        private FixationContextCollector _fixationContextCollector;
        private FixationSummaryCollector _fixationSummaryCollector;

        public Daemon()
        {
            Name = "Tobii Tracker";
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
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Settings.FixationContextEnabled)
            {
                var fixationContextProvider = new FixationContextProvider(_host);
                _fixationContextCollector = new FixationContextCollector();
                fixationContextProvider.FixationStarted += _fixationContextCollector.Collect;
                fixationContextProvider.Start();
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Settings.FixationSummaryEnabled)
            {
                var fixationSummaryProvider = new FixationSummaryProvider(_host);
                _fixationSummaryCollector = new FixationSummaryCollector();
                fixationSummaryProvider.FixationEnded += _fixationSummaryCollector.Collect;
                fixationSummaryProvider.Start();
            }

            IsRunning = true;
        }

        public override void Stop()
        {
            _host.DisableConnection();
            _fixationContextCollector.Stop();
            _fixationSummaryCollector.Stop();
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
