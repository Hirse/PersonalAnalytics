using GameOverlay.Graphics;
using GameOverlay.Graphics.Primitives;
using GameOverlay.Utilities;
using GameOverlay.Windows;
using System;
using System.Timers;
using TobiiTracker.Helpers;
using TobiiTracker.Native;
using Point = TobiiTracker.Helpers.Point;

namespace TobiiTracker
{
    internal class HighlighterOverlay : IDisposable, IStoppable
    {
        private const int FramesPerSecond = 60;
        private const int HighlightTimeout = 3000;
        private const int Size = 100;

        private readonly OverlayWindow _window;
        private readonly D2DDevice _device;
        private readonly FrameTimer _frameTimer;
        private readonly D2DSolidColorBrush _brush;
        private readonly Timer _timer;

        private bool _shouldDraw;
        private float _x;
        private float _y;

        public bool Stopped { get; private set; } = true;

        internal HighlighterOverlay()
        {
            var monitorSize = NativeMethods.GetPrimaryMonitorSize();

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

            _brush = _device.CreateSolidColorBrush(0x0, 0x00, 0x0, 0x40);

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

        internal void Show(Point point)
        {
            _x = (float)point.X;
            _y = (float)point.Y;
            _shouldDraw = true;
            _timer.Start();
        }

        internal void HideConditionally(double x, double y)
        {
            if (Math.Abs(_x - x) < Size / 2F && Math.Abs(_y - y) < Size / 2F)
            {
                _shouldDraw = false;
            }
        }

        private void _frameTimer_OnFrame(FrameTimer timer, D2DDevice device)
        {
            device.ClearScene();
            if (_shouldDraw)
            {
                device.FillRectangle(new Rectangle(0, 0, _x - Size, _y + Size), _brush);
                device.FillRectangle(new Rectangle(_x - Size, 0, _window.Width, _y - Size), _brush);
                device.FillRectangle(new Rectangle(_x + Size, _y - Size, _window.Width, _window.Height), _brush);
                device.FillRectangle(new Rectangle(0, _y + Size, _x + Size, _window.Height), _brush);
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
                _timer.Dispose();
            }
        }
    }
}
