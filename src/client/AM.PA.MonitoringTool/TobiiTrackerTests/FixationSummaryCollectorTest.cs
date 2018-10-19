﻿using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TobiiTracker;
using TobiiTracker.Data;
using TobiiTracker.Data.Fakes;

namespace TobiiTrackerTests
{
    [TestClass]
    public class FixationSummaryCollectorTest
    {
        [TestMethod]
        public void TestCollectThreshold()
        {
            using (ShimsContext.Create())
            {
                var called = false;
                ShimQueries.SaveFixationSummaryEntriesIEnumerableOfFixationSummaryEntry =
                    entry => { called = true; };
                var fixationSummaryCollector = new FixationSummaryCollector();
                fixationSummaryCollector.Collect(null, new FixationSummaryEntry());
                Assert.IsFalse(called);
                for (var i = 0; i < 500; i++)
                {
                    fixationSummaryCollector.Collect(null, new FixationSummaryEntry());
                }
                Assert.IsTrue(called);
            }
        }

        [TestMethod]
        public void TestStop()
        {
            using (ShimsContext.Create())
            {
                var called = false;
                ShimQueries.SaveFixationSummaryEntriesIEnumerableOfFixationSummaryEntry =
                    entry => { called = true; };
                var fixationSummaryCollector = new FixationSummaryCollector();
                fixationSummaryCollector.Stop();
                Assert.IsFalse(called);
                fixationSummaryCollector.Collect(null, new FixationSummaryEntry());
                fixationSummaryCollector.Stop();
                Assert.IsTrue(called);
            }
        }
    }
}
