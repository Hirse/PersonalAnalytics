using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TobiiTracker.Helpers;

namespace TobiiTracker.Native
{
    internal static class NativeMonitorMethods
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

        #region private extern
        // ReSharper disable InconsistentNaming
        // ReSharper disable MemberCanBePrivate.Local

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

        // ReSharper restore InconsistentNaming
        // ReSharper restore MemberCanBePrivate.Local
        #endregion
    }
}
