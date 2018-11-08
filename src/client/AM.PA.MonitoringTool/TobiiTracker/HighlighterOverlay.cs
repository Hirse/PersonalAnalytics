using GameOverlay.Graphics;
using GameOverlay.Utilities;
using GameOverlay.Windows;
using System;
using System.Timers;
using TobiiTracker.Helpers;
using TobiiTracker.Native;

namespace TobiiTracker
{
    internal class HighlighterOverlay : IDisposable, IStoppable
    {
        private const int FramesPerSecond = 60;
        private const int HighlightTimeout = 3000;

        private readonly OverlayWindow _window;
        private readonly D2DDevice _device;
        private readonly FrameTimer _frameTimer;
        private readonly D2DSolidColorBrush _brush;
        private readonly Timer _timer;

        private bool _shouldDraw;
        private float _x;
        private float _y;
        private int _radius;

        public bool Stopped { get; private set; } = true;

        internal HighlighterOverlay()
        {
            var monitorSize = NativeMonitorMethods.GetPrimaryMonitorSize();

            _window = new OverlayWindow(new OverlayOptions
            {
                BypassTopmost = false,
                Height = (int)monitorSize.Y,
                Width = (int)monitorSize.X,
                WindowTitle = "HighlighterOverlay",
                X = 0,
                Y = 0
            });

            _device = new D2DDevice(new DeviceOptions
            {
                AntiAliasing = true,
                Hwnd = _window.WindowHandle,
                MeasureFps = false,
                MultiThreaded = false,
                VSync = false
            });

            _brush = _device.CreateSolidColorBrush(0x0, 0xFF, 0x0, 0x80);

            _frameTimer = new FrameTimer(_device, FramesPerSecond);
            _frameTimer.OnFrame += _frameTimer_OnFrame;

            _timer = new Timer(HighlightTimeout);
            _timer.Elapsed += (sender, args) => { _shouldDraw = false; };
        }

        public void Start()
        {
            Stopped = false;
            _frameTimer.Start();
        }

        public void Stop()
        {
            _shouldDraw = false;
            _frameTimer.Stop();
            Stopped = true;
        }

        internal void Show(Point point, int radius)
        {
            _x = (float)point.X;
            _y = (float)point.Y;
            _radius = radius;
            _shouldDraw = true;
            _timer.Start();
        }

        private void _frameTimer_OnFrame(FrameTimer timer, D2DDevice device)
        {
            device.ClearScene();
            if (_shouldDraw)
            {
                device.FillCircle(_x, _y, _radius, _brush);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _window.Dispose();
                _device.Dispose();
                _frameTimer.Dispose();
                _brush.Dispose();
            }
        }
    }
}
