using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TobiiTracker.Helpers;

namespace TobiiTracker.Native
{
    internal static class NativeMethods
    {

        internal static Point GetPrimaryMonitorSize()
        {
            var monitorList = new List<IntPtr>();
            var listHandle = GCHandle.Alloc(monitorList);
            var monitorEnumProc = new MonitorEnumProc((IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData) =>
            {
                monitorList.Add(hMonitor);
                return true;
            });
            try
            {
                EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, monitorEnumProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                {
                    listHandle.Free();
                }
            }
            var primaryMonitorInfo = monitorList.Select(monitorHandle =>
            {
                var monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
                GetMonitorInfo(monitorHandle, ref monitorInfo);
                return monitorInfo;
            }).Single(monitorInfo => (monitorInfo.dwFlags & MONITORINFOF_PRIMARY) != 0);
            var x = primaryMonitorInfo.rcMonitor.Right - primaryMonitorInfo.rcMonitor.Left;
            var y = primaryMonitorInfo.rcMonitor.Bottom - primaryMonitorInfo.rcMonitor.Top;
            return new Point(x, y);
        }

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

        internal static IntPtr GetRootWindow(IntPtr hWnd)
        {
            return GetAncestor(hWnd, GA_ROOT);
        }

        internal static string GetWindowTitle(IntPtr window)
        {
            const int numChars = 256;
            var stringBuilder = new StringBuilder(numChars);
            GetWindowText(window, stringBuilder, numChars);
            return stringBuilder.ToString();
        }

        #region private extern
        // ReSharper disable IdentifierTypo
        // ReSharper disable InconsistentNaming
        // ReSharper disable MemberCanBePrivate.Local

        /// <summary>
        /// The EnumDisplayMonitors function enumerates display monitors (including invisible pseudo-monitors associated with the mirroring drivers) that intersect a region formed by the intersection of a specified clipping rectangle and the visible region of a device context.
        /// EnumDisplayMonitors calls an application-defined MonitorEnumProc callback function once for each monitor that is enumerated.
        /// Note that GetSystemMetrics (SM_CMONITORS) counts only the display monitors.
        /// </summary>
        /// <remarks>https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-enumdisplaymonitors</remarks>
        /// <param name="hdc">A handle to a display device context that defines the visible region of interest. If this parameter is NULL, the hdcMonitor parameter passed to the callback function will be NULL, and the visible region of interest is the virtual screen that encompasses all the displays on the desktop.</param>
        /// <param name="lprcClip">A pointer to a RECT structure that specifies a clipping rectangle. The region of interest is the intersection of the clipping rectangle with the visible region specified by hdc.</param>
        /// <param name="lpfnEnum">A pointer to a MonitorEnumProc application-defined callback function.</param>
        /// <param name="dwData">Application-defined data that EnumDisplayMonitors passes directly to the MonitorEnumProc function.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

        /// <summary>
        /// Retrieves the handle to the ancestor of the specified window.
        /// </summary>
        /// <remarks>https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getancestor</remarks>
        /// <param name="hwnd">A handle to the window whose ancestor is to be retrieved. If this parameter is the desktop window, the function returns NULL.</param>
        /// <param name="gaFlags">The ancestor to be retrieved.</param>
        /// <returns>The return value is the handle to the ancestor window.</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);

        /// <summary>
        /// The GetMonitorInfo function retrieves information about a display monitor.
        /// </summary>
        /// <remarks>https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getmonitorinfoa</remarks>
        /// <param name="hMonitor">A handle to the display monitor of interest.</param>
        /// <param name="lpmi">
        /// A pointer to a MONITORINFO or MONITORINFOEX structure that receives information about the specified display monitor.
        /// You must set the cbSize member of the structure to sizeof(MONITORINFO) or sizeof(MONITORINFOEX) before calling the GetMonitorInfo function.Doing so lets the function determine the type of structure you are passing to it.
        ///
        /// The MONITORINFOEX structure is a superset of the MONITORINFO structure. It has one additional member: a string that contains a name for the display monitor.Most applications have no use for a display monitor name, and so can save some bytes by using a MONITORINFO structure.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        /// <summary>
        /// Copies the text of the specified window's title bar (if it has one) into a buffer. If the specified window is a control, the text of the control is copied. However, GetWindowText cannot retrieve the text of a control in another application.
        /// </summary>
        /// <remarks>https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getwindowtexta</remarks>
        /// <param name="hWnd">A handle to the window or control containing the text.</param>
        /// <param name="lpString">The buffer that will receive the text. If the string is as long or longer than the buffer, the string is truncated and terminated with a null character.</param>
        /// <param name="nMaxCount">The maximum number of characters to copy to the buffer, including the null character. If the text exceeds this limit, it is truncated.</param>
        /// <returns>
        /// If the function succeeds, the return value is the length, in characters, of the copied string, not including the terminating null character.
        /// If the window has no title bar or text, if the title bar is empty, or if the window or control handle is invalid, the return value is zero.
        /// To get extended error information, call GetLastError.
        /// This function cannot retrieve the text of an edit control in another application.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        /// <summary>
        /// Retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the process that created the window.
        /// </summary>
        /// <remarks>https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getwindowthreadprocessid</remarks>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="lpdwProcessId">A pointer to a variable that receives the process identifier. If this parameter is not NULL, GetWindowThreadProcessId copies the identifier of the process to the variable; otherwise, it does not.</param>
        /// <returns>The return value is the identifier of the thread that created the window.</returns>
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        /// <summary>
        /// Retrieves a handle to the window that contains the specified point.
        /// </summary>
        /// <remarks>https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-windowfrompoint</remarks>
        /// <param name="Point">The point to be checked.</param>
        /// <returns>
        /// The return value is a handle to the window that contains the point.
        /// If no window exists at the given point, the return value is NULL.
        /// If the point is over a static text control, the return value is a handle to the window under the static text control.
        /// </returns>
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT Point);

        /// <summary>
        /// A MonitorEnumProc function is an application-defined callback function that is called by the EnumDisplayMonitors function.
        /// A value of type MONITORENUMPROC is a pointer to a MonitorEnumProc function.
        /// </summary>
        /// <remarks>https://docs.microsoft.com/en-ca/windows/desktop/api/winuser/nc-winuser-monitorenumproc</remarks>
        /// <param name="hMonitor">A handle to a display device context that defines the visible region of interest. If this parameter is NULL, the hdcMonitor parameter passed to the callback function will be NULL, and the visible region of interest is the virtual screen that encompasses all the displays on the desktop.</param>
        /// <param name="hdcMonitor">A pointer to a RECT structure that specifies a clipping rectangle. The region of interest is the intersection of the clipping rectangle with the visible region specified by hdc.</param>
        /// <param name="lprcMonitor">A pointer to a MonitorEnumProc application-defined callback function.</param>
        /// <param name="dwData">Application-defined data that EnumDisplayMonitors passes directly to the MonitorEnumProc function.</param>
        /// <returns>To continue the enumeration, return TRUE. To stop the enumeration, return FALSE.</returns>
        private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        /// <summary>
        /// Retrieves the root window by walking the chain of parent windows.
        /// </summary>
        private const uint GA_ROOT = 2;

        /// <summary>
        /// This is the primary display monitor.
        /// </summary>
        private const int MONITORINFOF_PRIMARY = 0x00000001;

        /// <summary>
        /// The MONITORINFO structure contains information about a display monitor.
        /// The GetMonitorInfo function stores information into a MONITORINFO structure or a MONITORINFOEX structure.
        /// The MONITORINFO structure is a subset of the MONITORINFOEX structure. The MONITORINFOEX structure adds a string member to contain a name for the display monitor.
        /// </summary>
        /// <remarks>https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-tagmonitorinfo</remarks>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MONITORINFO
        {
            /// <summary>
            /// The size, in bytes, of the structure.
            /// Set this member to <c>sizeof(MONITORINFO)</c> before calling the <see cref="GetMonitorInfo"/> function.
            /// Doing so lets the function determine the type of structure you are passing to it.
            /// </summary>
            public int cbSize;

            /// <summary>
            /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates. 
            /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
            /// </summary>
            public readonly RECT rcMonitor;

            /// <summary>
            /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications, 
            /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor. 
            /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars. 
            /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
            /// </summary>
            public readonly RECT rcWork;

            /// <summary>
            /// A set of flags that represent attributes of the display monitor.
            /// </summary>
            public readonly uint dwFlags;
        }

        /// <summary>
        /// The POINT structure defines the x- and y- coordinates of a point.
        /// </summary>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/ecb0f0e1-90c2-48ab-a069-552262b49c7c
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            /// <summary>
            /// The x-coordinate of the point.
            /// </summary>
            private readonly int X;

            /// <summary>
            /// The y-coordinate of the point.
            /// </summary>
            private readonly int Y;

            private POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static implicit operator POINT(Point point)
            {
                return new POINT((int)point.X, (int)point.Y);
            }

            public static explicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        /// <summary>
        /// The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
        /// </summary>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/9439cb6c-f2f7-4c27-b1d7-8ddf16d81fe8
        /// By convention, the right and bottom edges of the rectangle are normally considered exclusive. 
        /// In other words, the pixel whose coordinates are ( right, bottom ) lies immediately outside of the the rectangle. 
        /// For example, when RECT is passed to the FillRect function, the rectangle is filled up to, but not including, 
        /// the right column and bottom row of pixels. This structure is identical to the RECTL structure.
        /// </remarks>
        [Serializable, StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            /// <summary>
            /// The x-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public readonly int Left;

            /// <summary>
            /// The y-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public readonly int Top;

            /// <summary>
            /// The x-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public readonly int Right;

            /// <summary>
            /// The y-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public readonly int Bottom;
        }

        // ReSharper restore IdentifierTypo
        // ReSharper restore InconsistentNaming
        // ReSharper restore MemberCanBePrivate.Local
        #endregion
    }
}
