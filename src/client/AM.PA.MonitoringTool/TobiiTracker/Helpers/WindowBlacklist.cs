using System.Collections.Generic;

namespace TobiiTracker.Helpers
{
    internal class WindowBlacklist
    {
        private static readonly Dictionary<string, HashSet<string>> Blacklist = new Dictionary<string, HashSet<string>>
        {
            {"explorer", new HashSet<string>
            {
                "", // Task Bar
                "Program Manager", // Desktop
                "Task Switching", // Alt-Tab
                "Task View", // Win-Tab
                "Input Flyout", // Keyboard Layout Selector
            }},
            {"ShellExperienceHost", new HashSet<string>
            {
                "Start", // Start Menu
                "Battery Information",
                "Network Connections",
                "Volume Control",
                "Action Centre", //Win-A
                "Date and Time Information",
                "Windows Ink Workspace", //Win-W
                "CONNECT", // Win-K
                "Project", // Win-P
            }},
            {"SearchUI", new HashSet<string>
            {
                "Cortana" // Start Menu Search
            }},
            {"LockApp", new HashSet<string>
            {
                "Windows Default Lock Screen" // Win-L lock screen
            }},
            {"dwm", new HashSet<string>
            {
                "" // Windows Desktop Manager
            }}
        };

        internal static bool IsBlacklisted(string processName, string windowTitle)
        {
            return Blacklist.ContainsKey(processName) && Blacklist[processName].Contains(windowTitle);
        }
    }
}
