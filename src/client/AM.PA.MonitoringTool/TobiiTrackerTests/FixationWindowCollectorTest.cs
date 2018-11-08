using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TobiiTracker;
using TobiiTracker.Data;
using TobiiTracker.Data.Fakes;

namespace TobiiTrackerTests
{
    [TestClass]
    public class FixationWindowCollectorTest
    {
        [TestMethod]
        public void TestCollectThreshold()
        {
            using (ShimsContext.Create())
            {
                var called = false;
                ShimQueries.SaveFixationWindowEntriesIEnumerableOfFixationWindowEntry =
                    entry => { called = true; };
                var fixationWindowCollector = new FixationWindowCollector();
                fixationWindowCollector.Start();
                fixationWindowCollector.Collect(null, new FixationWindowEntry());
                Assert.IsFalse(called);
                for (var i = 0; i < 500; i++)
                {
                    fixationWindowCollector.Collect(null, new FixationWindowEntry());
                }
                Assert.IsTrue(called);
            }
        }

        [TestMethod]
        public void TestStop()
        {
            using (ShimsContext.Create())
            {
                var called = 0;
                ShimQueries.SaveFixationWindowEntriesIEnumerableOfFixationWindowEntry =
                    entry => { called += 1; };
                var fixationWindowCollector = new FixationWindowCollector();
                fixationWindowCollector.Collect(null, new FixationWindowEntry());
                Assert.AreEqual(0, called);
                fixationWindowCollector.Start();
                fixationWindowCollector.Collect(null, new FixationWindowEntry());
                fixationWindowCollector.Stop();
                fixationWindowCollector.Collect(null, new FixationWindowEntry());
                Assert.AreEqual(1, called);
            }
        }
    }
}
