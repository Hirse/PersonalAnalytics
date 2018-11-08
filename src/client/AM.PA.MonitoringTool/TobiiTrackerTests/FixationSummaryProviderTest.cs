using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tobii.Interaction.Fakes;
using TobiiTracker;

namespace TobiiTrackerTests
{
    [TestClass]
    public class FixationSummaryProviderTest
    {
        [TestMethod]
        public void TestFixationEnded()
        {
            using (ShimsContext.Create())
            {
                var called = 0;
                Action<double, double, double> fixationStart = (x, y, t) => { };
                Action<double, double, double> fixationData = (x, y, t) => { };
                Action<double, double, double> fixationEnd = (x, y, t) => { };
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
                            },
                            DataActionOfDoubleDoubleDouble = action =>
                            {
                                fixationData = action;
                                return new ShimFixationDataStream();
                            },
                            EndActionOfDoubleDoubleDouble = action =>
                            {
                                fixationEnd = action;
                                return new ShimFixationDataStream();
                            }
                        }
                    }
                };
                var fixationSummaryProvider = new FixationSummaryProvider(shimHost);
                fixationSummaryProvider.FixationEnded += (sender, fixationSummaryEntry) => { called += 1; };
                fixationStart(0, 0, 0);
                fixationEnd(0, 0, 0);
                fixationSummaryProvider.Start();
                Assert.AreEqual(0, called);
                fixationStart(0, 0, 0);
                fixationData(0, 0, 0);
                fixationEnd(0, 0, 0);
                Assert.AreEqual(1, called);
                fixationSummaryProvider.Stop();
                fixationStart(0, 0, 0);
                fixationData(0, 0, 0);
                fixationEnd(0, 0, 0);
                Assert.AreEqual(1, called);
            }
        }

        [TestMethod]
        public void TestFixationOrder()
        {
            using (ShimsContext.Create())
            {
                var shimHost = new ShimHost
                {
                    StreamsGet = () => new ShimGlobalDataStreamAgent
                    {
                        CreateFixationDataStreamFixationDataModeBoolean = (mode, enabled) => new ShimFixationDataStream
                        {
                            BeginActionOfDoubleDoubleDouble = action => new ShimFixationDataStream(),
                            DataActionOfDoubleDoubleDouble = action =>
                            {
                                action(2, 2, 0);
                                return new ShimFixationDataStream();
                            },
                            EndActionOfDoubleDoubleDouble = action =>
                            {
                                action(3, 3, 0);
                                return new ShimFixationDataStream();
                            }
                        }
                    }
                };
                var fixationSummaryProvider = new FixationSummaryProvider(shimHost);
                fixationSummaryProvider.FixationEnded += (sender, fixationSummaryEntry) => Assert.Fail();
                fixationSummaryProvider.Start();
            }
        }

        [TestMethod]
        public void TestEventHandler()
        // Coverage for case of no event handler
        {
            using (ShimsContext.Create())
            {
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
                            },
                            DataActionOfDoubleDoubleDouble = action =>
                            {
                                action(2, 2, 0);
                                return new ShimFixationDataStream();
                            },
                            EndActionOfDoubleDoubleDouble = action =>
                            {
                                action(3, 3, 0);
                                return new ShimFixationDataStream();
                            }
                        }
                    }
                };
                var fixationSummaryProvider = new FixationSummaryProvider(shimHost);
                fixationSummaryProvider.Start();
                // Events are fired immediately, register afterwards to verify it doesn't get called again
                fixationSummaryProvider.FixationEnded += (sender, fixationSummaryEntry) => Assert.Fail();
            }
        }
    }
}
