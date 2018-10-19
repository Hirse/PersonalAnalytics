using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace TobiiTracker.Helpers
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point point);

        internal static IntPtr GetWindowFromPoint(double x, double y)
        {
            return WindowFromPoint(new Point(x, y));
        }

        internal static string GetProcessName(IntPtr window)
        {
            GetWindowThreadProcessId(window, out var processIdUint);
            var processId = (int)processIdUint;
            var process = Process.GetProcessById(processId);
            return process.ProcessName;
        }

        internal static string GetWindowTitle(IntPtr window)
        {
            const int numChars = 256;
            var stringBuilder = new StringBuilder(numChars);
            GetWindowText(window, stringBuilder, numChars);
            return stringBuilder.ToString();
        }
    }
}
