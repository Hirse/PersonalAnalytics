using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                var called = false;
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
                fixationSummaryProvider.FixationEnded += (sender, fixationSummaryEntry) => { called = true; };
                fixationSummaryProvider.Start();
                Assert.IsTrue(called);
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
