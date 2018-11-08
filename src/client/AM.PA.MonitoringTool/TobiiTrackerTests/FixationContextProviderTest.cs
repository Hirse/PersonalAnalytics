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
                var called = 0;
                const string process = "__process__";
                const string window = "__window__";
                Action<double, double, double> fixationStart = (x, y, t) => { };
                TobiiTracker.Native.Fakes.ShimNativeWindowMethods.GetWindowFromPointDoubleDouble = (x, y) => new IntPtr(0);
                TobiiTracker.Native.Fakes.ShimNativeWindowMethods.GetProcessNameIntPtr = ptr => process;
                TobiiTracker.Native.Fakes.ShimNativeWindowMethods.GetWindowTitleIntPtr = ptr => window;
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
                var fixationContextProvider = new FixationContextProvider(shimHost);
                fixationContextProvider.FixationStarted += (sender, fixationContextEntry) =>
                {
                    called += 1;
                    Assert.AreEqual(process, fixationContextEntry.ProcessName);
                    Assert.AreEqual(window, fixationContextEntry.WindowTitle);
                };
                fixationStart(0, 0, 0);
                Assert.AreEqual(0, called);
                fixationContextProvider.Start();
                fixationStart(0, 0, 0);
                Assert.AreEqual(1, called);
                fixationContextProvider.Stop();
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
                TobiiTracker.Native.Fakes.ShimNativeWindowMethods.GetWindowFromPointDoubleDouble = (x, y) => new IntPtr(0);
                TobiiTracker.Native.Fakes.ShimNativeWindowMethods.GetProcessNameIntPtr = ptr => process;
                TobiiTracker.Native.Fakes.ShimNativeWindowMethods.GetWindowTitleIntPtr = ptr => window;
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
