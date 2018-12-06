using System;
using Tobii.Interaction;
using TobiiTracker.Data;
using TobiiTracker.Helpers;
using TobiiTracker.Native;

namespace TobiiTracker
{
    internal class FixationWindowProvider : IStoppable
    {
        private readonly Host _host;

        public bool Stopped { get; private set; } = true;

        internal event EventHandler<FixationWindowEntry> FixationStarted;

        internal FixationWindowProvider(Host host)
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
            var windowHandle = NativeMethods.GetWindowFromPoint(x, y);
            var rootWindowHandle = NativeMethods.GetRootWindow(windowHandle);
            var processName = NativeMethods.GetProcessName(rootWindowHandle);
            var windowTitle = NativeMethods.GetWindowTitle(rootWindowHandle);
            FixationStarted?.Invoke(this, new FixationWindowEntry
            {
                ProcessName = processName,
                Time = DateTime.UtcNow,
                WindowTitle = windowTitle,
                WindowHandle = rootWindowHandle,
                X = x,
                Y = y
            });
        }
    }
}
