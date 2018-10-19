using System;
using Tobii.Interaction;
using TobiiTracker.Data;
using TobiiTracker.Helpers;

namespace TobiiTracker
{
    internal class FixationContextProvider
    {
        private readonly Host _host;

        internal event EventHandler<FixationContextEntry> FixationStarted;

        internal FixationContextProvider(Host host)
        {
            _host = host;
        }

        internal void Start()
        {
            _host.Streams.CreateFixationDataStream().Begin(LocateFixation);
        }

        private void LocateFixation(double x, double y, double timestamp)
        {
            var windowHandle = NativeMethods.GetWindowFromPoint(x, y);
            var processName = NativeMethods.GetProcessName(windowHandle);
            var windowTitle = NativeMethods.GetWindowTitle(windowHandle);
            FixationStarted?.Invoke(this, new FixationContextEntry
            {
                Process = processName,
                Time = DateTime.UtcNow,
                Window = windowTitle,
                X = x,
                Y = y
            });
        }
    }
}
