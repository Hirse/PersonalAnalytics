using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Data;
using Shared.Data.Fakes;
using TobiiTracker;
using TobiiTracker.Data;

namespace TobiiTrackerTests
{
    [TestClass]
    public class QueriesTest
    {
        private DatabaseImplementation _db;
        private string _exportFilePath;

        [TestInitialize]
        public void Initialize()
        {
            // Set file path to temp to keep tests messages out of log
            _exportFilePath = Shared.Settings.ExportFilePath;
            Shared.Settings.ExportFilePath = Path.GetTempPath();
            _db = new DatabaseImplementation(":memory:");
            _db.Connect();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _db.Disconnect();
            Shared.Settings.ExportFilePath = _exportFilePath;
        }

        [TestMethod]
        public void TestCreateTables()
        {
            using (ShimsContext.Create())
            {
                ShimDatabase.GetInstance = () => _db;
                Queries.CreateTables();
            }
            Assert.IsTrue(_db.HasTable(Settings.FixationContextTable));
            Assert.IsTrue(_db.HasTable(Settings.FixationSummaryTable));
            Assert.IsTrue(_db.HasTable(Settings.FixationWindowTable));
        }

        [TestMethod]
        public void TestSaveFixationContextEntries()
        {
            var entry = new FixationContextEntry
            {
                X = 1.1,
                Y = 2.2,
                ProcessName = "__process",
                WindowTitle = "__window__",
                Time = new DateTime(2000, 3, 3, 0, 0, 0, DateTimeKind.Utc)
            };

            using (ShimsContext.Create())
            {
                ShimDatabase.GetInstance = () => _db;
                Queries.CreateTables();
                Assert.AreEqual(0L, _db.ExecuteScalar2($@"SELECT COUNT(*) FROM {Settings.FixationContextTable};"));
                Queries.SaveFixationContextEntries(new List<FixationContextEntry> { entry });
                var dataTable = _db.ExecuteReadQuery($@"SELECT * FROM {Settings.FixationContextTable};");
                Assert.AreEqual(1, dataTable.Rows.Count);
                Assert.AreEqual(entry.X, dataTable.Rows[0]["x"]);
                Assert.AreEqual(entry.Y, dataTable.Rows[0]["y"]);
                Assert.AreEqual(entry.ProcessName, dataTable.Rows[0]["process"]);
                Assert.AreEqual(entry.WindowTitle, dataTable.Rows[0]["window"]);
                Assert.AreEqual(entry.Time.ToString("u"), dataTable.Rows[0]["time"]);
            }
        }

        [TestMethod]
        public void TestSaveFixationSummaryEntries()
        {
            var entry = new FixationSummaryEntry
            {
                Cx = 1.1,
                Cy = 2.2,
                Radius = 3.3,
                StartTime = new DateTime(2000, 3, 3, 0, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2000, 4, 4, 0, 0, 0, DateTimeKind.Utc)
            };

            using (ShimsContext.Create())
            {
                ShimDatabase.GetInstance = () => _db;
                Queries.CreateTables();
                Assert.AreEqual(0L, _db.ExecuteScalar2($@"SELECT COUNT(*) FROM {Settings.FixationSummaryTable};"));
                Queries.SaveFixationSummaryEntries(new List<FixationSummaryEntry> { entry });
                var dataTable = _db.ExecuteReadQuery($@"SELECT * FROM {Settings.FixationSummaryTable};");
                Assert.AreEqual(1, dataTable.Rows.Count);
                Assert.AreEqual(entry.Cx, dataTable.Rows[0]["cx"]);
                Assert.AreEqual(entry.Cy, dataTable.Rows[0]["cy"]);
                Assert.AreEqual(entry.Radius, dataTable.Rows[0]["radius"]);
                Assert.AreEqual(entry.StartTime.ToString("u"), dataTable.Rows[0]["start"]);
                Assert.AreEqual(entry.EndTime.ToString("u"), dataTable.Rows[0]["end"]);
            }
        }

        [TestMethod]
        public void TestSaveFixationWindowEntries()
        {
            var entry = new FixationWindowEntry
            {
                X = 1.1,
                Y = 2.2,
                ProcessName = "__process",
                WindowTitle = "__window__",
                WindowHandle = new IntPtr(123),
                Time = new DateTime(2000, 3, 3, 0, 0, 0, DateTimeKind.Utc)
            };

            using (ShimsContext.Create())
            {
                ShimDatabase.GetInstance = () => _db;
                Queries.CreateTables();
                Assert.AreEqual(0L, _db.ExecuteScalar2($@"SELECT COUNT(*) FROM {Settings.FixationWindowTable};"));
                Queries.SaveFixationWindowEntries(new List<FixationWindowEntry> { entry });
                var dataTable = _db.ExecuteReadQuery($@"SELECT * FROM {Settings.FixationWindowTable};");
                Assert.AreEqual(1, dataTable.Rows.Count);
                Assert.AreEqual(entry.X, dataTable.Rows[0]["x"]);
                Assert.AreEqual(entry.Y, dataTable.Rows[0]["y"]);
                Assert.AreEqual(entry.ProcessName, dataTable.Rows[0]["processName"]);
                Assert.AreEqual(entry.WindowTitle, dataTable.Rows[0]["windowTitle"]);
                Assert.AreEqual(entry.WindowHandle.ToString(), dataTable.Rows[0]["windowHandle"]);
                Assert.AreEqual(entry.Time.ToString("u"), dataTable.Rows[0]["time"]);
            }
        }
    }
}
