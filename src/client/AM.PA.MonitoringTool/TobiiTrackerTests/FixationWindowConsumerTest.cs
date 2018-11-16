using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TobiiTracker;
using TobiiTracker.Data;
using TobiiTracker.Fakes;

namespace TobiiTrackerTests
{
    [TestClass]
    public class FixationWindowConsumerTest
    {
        [TestMethod]
        public void TestVisualize()
        {
            using (ShimsContext.Create())
            {
                var called = false;
                const int x = 10;
                const int y = 10;
                var shimHighlighterOverlay = new ShimHighlighterOverlay
                {
                    ShowPointInt32 = (point, radius) =>
                    {
                        called = true;
                        Assert.AreEqual(x, point.X);
                        Assert.AreEqual(y, point.Y);
                    }
                };
                var fixationWindowConsumer = new FixationWindowConsumer(shimHighlighterOverlay);
                fixationWindowConsumer.Start();

                fixationWindowConsumer.Collect(null, new FixationWindowEntry
                {
                    ProcessName = "explorer",
                    WindowTitle = "Window Title"
                });
                Assert.IsFalse(called, "Should not be called after blacklisted window");

                fixationWindowConsumer.Collect(null, new FixationWindowEntry
                {
                    ProcessName = "",
                    WindowTitle = "Window Title",
                    WindowHandle = new IntPtr(1),
                    X = x,
                    Y = y
                });
                Assert.IsFalse(called, "Should not be called after first fixation");

                fixationWindowConsumer.Collect(null, new FixationWindowEntry
                {
                    ProcessName = "",
                    WindowTitle = "Window Title",
                    WindowHandle = new IntPtr(2)
                });
                Assert.IsFalse(called, "Should not be called after fixation on new window");

                fixationWindowConsumer.Collect(null, new FixationWindowEntry
                {
                    ProcessName = "",
                    WindowTitle = "Window Title",
                    WindowHandle = new IntPtr(2)
                });
                Assert.IsFalse(called, "Should not be called after fixation on same window");

                fixationWindowConsumer.Collect(null, new FixationWindowEntry
                {
                    ProcessName = "",
                    WindowTitle = "Window Title",
                    WindowHandle = new IntPtr(1)
                });
                Assert.IsTrue(called, "Should be called after fixation on previous window");
            }
        }

        [TestMethod]
        public void TestStoppable()
        {
            using (ShimsContext.Create())
            {
                var called = 0;
                var shimHighlighterOverlay = new ShimHighlighterOverlay
                {
                    Start = () => called += 1,
                    Stop = () => called += 10
                };
                var fixationWindowConsumer = new FixationWindowConsumer(shimHighlighterOverlay);
                Assert.IsTrue(fixationWindowConsumer.Stopped);
                fixationWindowConsumer.Collect(null, new FixationWindowEntry());
                fixationWindowConsumer.Start();
                Assert.AreEqual(1, called);
                fixationWindowConsumer.Stop();
                Assert.AreEqual(11, called);
            }
        }
    }
}
