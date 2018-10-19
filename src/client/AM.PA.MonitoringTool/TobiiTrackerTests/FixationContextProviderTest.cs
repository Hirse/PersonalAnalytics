using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tobii.Interaction.Fakes;
using TobiiTracker;

namespace TobiiTrackerTests
{
    [TestClass]
    public class FixationContextProviderTest
    {
        [TestMethod]
        public void TestFixationStarted()
        {
            using (ShimsContext.Create())
            {
                var called = false;
                const string process = "__process__";
                const string window = "__window__";
                TobiiTracker.Helpers.Fakes.ShimNativeMethods.GetWindowFromPointDoubleDouble = (x, y) => new IntPtr(0);
                TobiiTracker.Helpers.Fakes.ShimNativeMethods.GetProcessNameIntPtr = ptr => process;
                TobiiTracker.Helpers.Fakes.ShimNativeMethods.GetWindowTitleIntPtr = ptr => window;
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
                var fixationContextProvider = new FixationContextProvider(shimHost);
                fixationContextProvider.FixationStarted += (sender, fixationContextEntry) =>
                {
                    called = true;
                    Assert.AreEqual(process, fixationContextEntry.Process);
                    Assert.AreEqual(window, fixationContextEntry.Window);
                };
                fixationContextProvider.Start();
                Assert.IsTrue(called);
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
                TobiiTracker.Helpers.Fakes.ShimNativeMethods.GetWindowFromPointDoubleDouble = (x, y) => new IntPtr(0);
                TobiiTracker.Helpers.Fakes.ShimNativeMethods.GetProcessNameIntPtr = ptr => process;
                TobiiTracker.Helpers.Fakes.ShimNativeMethods.GetWindowTitleIntPtr = ptr => window;
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
                var fixationContextProvider = new FixationContextProvider(shimHost);
                fixationContextProvider.Start();
                fixationContextProvider.FixationStarted += (sender, fixationContextEntry) => Assert.Fail();
            }
        }
    }
}
