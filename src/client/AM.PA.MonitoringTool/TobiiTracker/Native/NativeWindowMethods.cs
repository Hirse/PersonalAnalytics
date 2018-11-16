using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using TobiiTracker.Helpers;

namespace TobiiTracker.Native
{
    internal static class NativeWindowMethods
    {

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
        /// Retrieves the root window by walking the chain of parent windows.
        /// </summary>
        private const uint GA_ROOT = 2;

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

        // ReSharper restore IdentifierTypo
        // ReSharper restore InconsistentNaming
        #endregion
    }
}
