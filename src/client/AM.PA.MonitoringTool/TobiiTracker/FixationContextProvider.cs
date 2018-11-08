using System;
using Tobii.Interaction;
using TobiiTracker.Data;
using TobiiTracker.Helpers;
using TobiiTracker.Native;

namespace TobiiTracker
{
    internal class FixationContextProvider : IStoppable
    {
        private readonly Host _host;

        public bool Stopped { get; private set; } = true;

        internal event EventHandler<FixationContextEntry> FixationStarted;

        internal FixationContextProvider(Host host)
        {
            _host = host;
        }

        public void Start()
        {
            Stopped = false;
            _host.Streams.CreateFixationDataStream().Begin(LocateFixation);
        }

        public void Stop()
        {
            Stopped = true;
        }

        private void LocateFixation(double x, double y, double timestamp)
        {
            if (Stopped) return;
            var windowHandle = NativeWindowMethods.GetWindowFromPoint(x, y);
            var processName = NativeWindowMethods.GetProcessName(windowHandle);
            var windowTitle = NativeWindowMethods.GetWindowTitle(windowHandle);
            FixationStarted?.Invoke(this, new FixationContextEntry
            {
                ProcessName = processName,
                Time = DateTime.UtcNow,
                WindowTitle = windowTitle,
                X = x,
                Y = y
            });
        }
    }
}
