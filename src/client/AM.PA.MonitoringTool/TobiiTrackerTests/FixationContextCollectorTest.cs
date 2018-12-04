﻿using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TobiiTracker;
using TobiiTracker.Data;
using TobiiTracker.Data.Fakes;

namespace TobiiTrackerTests
{
    [TestClass]
    public class FixationContextCollectorTest
    {
        [TestMethod]
        public void TestCollectThreshold()
        {
            using (ShimsContext.Create())
            {
                var called = false;
                ShimQueries.SaveFixationContextEntriesIEnumerableOfFixationContextEntry =
                    entry => { called = true; };
                var fixationContextCollector = new FixationContextCollector();
                fixationContextCollector.Collect(null, new FixationContextEntry());
                Assert.IsFalse(called);
                for (var i = 0; i < 500; i++)
                {
                    fixationContextCollector.Collect(null, new FixationContextEntry());
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
                ShimQueries.SaveFixationContextEntriesIEnumerableOfFixationContextEntry =
                    entry => { called = true; };
                var fixationContextCollector = new FixationContextCollector();
                fixationContextCollector.Stop();
                Assert.IsFalse(called);
                fixationContextCollector.Collect(null, new FixationContextEntry());
                fixationContextCollector.Stop();
                Assert.IsTrue(called);
            }
        }
    }
}
