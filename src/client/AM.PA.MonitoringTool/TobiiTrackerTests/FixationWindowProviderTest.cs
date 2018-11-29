using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tobii.Interaction.Fakes;
using TobiiTracker;
using TobiiTracker.Native.Fakes;

namespace TobiiTrackerTests
{
    [TestClass]
    public class FixationWindowProviderTest
    {
        [TestMethod]
        public void TestFixationStarted()
        {
            using (ShimsContext.Create())
            {
                var called = 0;
                const string process = "__process__";
                const string window = "__window__";
                Action<double, double, double> fixationStart = (x, y, t) => { };
                ShimNativeMethods.GetWindowFromPointDoubleDouble = (x, y) => new IntPtr(0);
                ShimNativeMethods.GetProcessNameIntPtr = ptr => process;
                ShimNativeMethods.GetWindowTitleIntPtr = ptr => window;
                var shimHost = new ShimHost
                {
                    StreamsGet = () => new ShimGlobalDataStreamAgent
                    {
                        CreateFixationDataStreamFixationDataModeBoolean = (mode, enabled) => new ShimFixationDataStream
                        {
                            BeginActionOfDoubleDoubleDouble = action =>
                            {
                                fixationStart = action;
                                return new ShimFixationDataStream();
                            }
                        }
                    }
                };
                var fixationWindowProvider = new FixationWindowProvider(shimHost);
                fixationWindowProvider.FixationStarted += (sender, fixationWindowEntry) =>
                {
                    called += 1;
                    Assert.AreEqual(process, fixationWindowEntry.ProcessName);
                    Assert.AreEqual(window, fixationWindowEntry.WindowTitle);
                };
                fixationStart(0, 0, 0);
                Assert.AreEqual(0, called);
                fixationWindowProvider.Start();
                fixationStart(0, 0, 0);
                Assert.AreEqual(1, called);
                fixationWindowProvider.Stop();
                fixationStart(0, 0, 0);
                Assert.AreEqual(1, called);
            }
        }

        [TestMethod]
        public void TestEventHandler()
        // Coverage for case of no event handler
        {
            using (ShimsContext.Create())
            {
                const string process = "__process__";
                const string window = "__window__";
                ShimNativeMethods.GetWindowFromPointDoubleDouble = (x, y) => new IntPtr(0);
                ShimNativeMethods.GetProcessNameIntPtr = ptr => process;
                ShimNativeMethods.GetWindowTitleIntPtr = ptr => window;
                var shimHost = new ShimHost
                {
                    StreamsGet = () => new ShimGlobalDataStreamAgent
                    {
                        CreateFixationDataStreamFixationDataModeBoolean = (mode, enabled) => new ShimFixationDataStream
                        {
                            BeginActionOfDoubleDoubleDouble = action =>
                            {
                                action(1, 1, 0);
                                return new ShimFixationDataStream();
                            }
                        }
                    }
                };
                var fixationWindowProvider = new FixationWindowProvider(shimHost);
                fixationWindowProvider.Start();
                fixationWindowProvider.FixationStarted += (sender, fixationWindowEntry) => Assert.Fail();
            }
        }
    }
}
