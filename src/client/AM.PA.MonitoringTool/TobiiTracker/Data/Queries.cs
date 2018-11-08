// Created: 2018-10-16
// 
// Licensed under the MIT License.

using Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TobiiTracker.Data
{
    internal static class Queries
    {
        internal static void CreateTables()
        {
            var db = Database.GetInstance();
            db.ExecuteDefaultQuery($@"CREATE TABLE IF NOT EXISTS {Settings.FixationContextTable} (id INTEGER PRIMARY KEY, x REAL, y REAL, process TEXT, window TEXT, time TEXT);");
            db.ExecuteDefaultQuery($@"CREATE TABLE IF NOT EXISTS {Settings.FixationSummaryTable} (id INTEGER PRIMARY KEY, cx REAL, cy REAL, radius REAL, start TEXT, end TEXT);");
            db.ExecuteDefaultQuery($@"CREATE TABLE IF NOT EXISTS {Settings.FixationWindowTable} (id INTEGER PRIMARY KEY, x REAL, y REAL, processName TEXT, windowTitle TEXT, windowHandle TEXT, time TEXT);");
        }

        internal static void SaveFixationContextEntries(IEnumerable<FixationContextEntry> fixationContextEntries)
        {
            var db = Database.GetInstance();
            var query = $@"INSERT INTO {Settings.FixationContextTable} (x, y, process, window, time) VALUES (?, ?, ?, ?, ?);";
            var parameters = fixationContextEntries.Select(entry => new object[] { entry.X, entry.Y, entry.ProcessName, entry.WindowTitle, entry.Time });
            db.ExecuteBatchQueries(query, parameters);
        }

        internal static void SaveFixationSummaryEntries(IEnumerable<FixationSummaryEntry> fixationSummaryEntries)
        {
            var db = Database.GetInstance();
            var query = $@"INSERT INTO {Settings.FixationSummaryTable} (cx, cy, radius, start, end) VALUES (?, ?, ?, ?, ?);";
            var parameters = fixationSummaryEntries.Select(entry => new object[] { entry.Cx, entry.Cy, entry.Radius, entry.StartTime, entry.EndTime });
            db.ExecuteBatchQueries(query, parameters);
        }

        internal static void SaveFixationWindowEntries(IEnumerable<FixationWindowEntry> fixationWindowEntries)
        {
            var db = Database.GetInstance();
            var query = $@"INSERT INTO {Settings.FixationWindowTable} (x, y, processName, windowTitle, windowHandle, time) VALUES (?, ?, ?, ?, ?, ?);";
            var parameters = fixationWindowEntries.Select(entry => new object[] { entry.X, entry.Y, entry.ProcessName, entry.WindowTitle, entry.WindowHandle, entry.Time });
            db.ExecuteBatchQueries(query, parameters);
        }
    }

    internal struct FixationContextEntry
    {
        public double X;
        public double Y;
        public string ProcessName;
        public string WindowTitle;
        public DateTime Time;
    }

    internal struct FixationSummaryEntry
    {
        public double Cx;
        public double Cy;
        public double Radius;
        public DateTime StartTime;
        public DateTime EndTime;
    }

    internal struct FixationWindowEntry
    {
        public double X;
        public double Y;
        public string ProcessName;
        public string WindowTitle;
        public IntPtr WindowHandle;
        public DateTime Time;
    }
}
