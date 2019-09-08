using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zametek.Utility.Tests
{
    [TestClass]
    public class TrackingContextTests
    {
        [TestMethod]
        public void TrackingContext_SerializeAndDeserialize_Success()
        {
            var extraHeaders = new Dictionary<string, string>()
            {
                { "AAA", "AAA" },
                { "BBB", "BBB" },
            };

            var tc1 = new TrackingContext(
                Guid.NewGuid(),
                DateTime.UtcNow,
                extraHeaders);

            byte[] bytes = TrackingContext.Serialize(tc1);

            TrackingContext tc2 = TrackingContext.DeSerialize(bytes);

            Assert.AreNotEqual(0, bytes.Length);
            Assert.AreEqual(tc1.CallChainId, tc2.CallChainId);
            Assert.AreEqual(tc1.OriginatorUtcTimestamp, tc2.OriginatorUtcTimestamp);
            CollectionAssert.AreEquivalent(tc1.ExtraHeaders.ToList(), tc2.ExtraHeaders.ToList());
        }

        [TestMethod]
        public async Task TrackingContext_MultipleTasks_CallChainIdPersists()
        {
            TrackingContext.NewCurrentIfEmpty();

            Guid masterCallChainId = TrackingContext.Current.CallChainId;

            var task0 = Task.Run(() =>
            {
                TrackingContext tc = TrackingContext.Current;
                return masterCallChainId == tc.CallChainId;
            });
            var task1 = Task.Run(() =>
            {
                TrackingContext tc = TrackingContext.Current;
                return masterCallChainId == tc.CallChainId;
            });
            var task2 = Task.Run(() =>
            {
                TrackingContext tc = TrackingContext.Current;
                return masterCallChainId == tc.CallChainId;
            });
            var tasks = new OrderedTaskCollection<bool>(
                new[]
                {
                    task0,
                    task1,
                    task2,
                });
            var results = new List<bool>();

            foreach (Task<bool> task in tasks)
            {
                results.Add(await task);
            }

            foreach (bool result in results)
            {
                Assert.IsTrue(result);
            }

            Assert.IsTrue(masterCallChainId == TrackingContext.Current.CallChainId);
        }

        [TestMethod]
        public async Task TrackingContext_MultipleTasks_OriginatorUtcTimestampPersists()
        {
            TrackingContext.NewCurrentIfEmpty();

            DateTime masterOriginatorUtcTimestamp = TrackingContext.Current.OriginatorUtcTimestamp;

            var task0 = Task.Run(() =>
            {
                TrackingContext tc = TrackingContext.Current;
                return masterOriginatorUtcTimestamp == tc.OriginatorUtcTimestamp;
            });
            var task1 = Task.Run(() =>
            {
                TrackingContext tc = TrackingContext.Current;
                return masterOriginatorUtcTimestamp == tc.OriginatorUtcTimestamp;
            });
            var task2 = Task.Run(() =>
            {
                TrackingContext tc = TrackingContext.Current;
                return masterOriginatorUtcTimestamp == tc.OriginatorUtcTimestamp;
            });
            var tasks = new OrderedTaskCollection<bool>(
                new[]
                {
                    task0,
                    task1,
                    task2,
                });
            var results = new List<bool>();

            foreach (Task<bool> task in tasks)
            {
                results.Add(await task);
            }

            foreach (bool result in results)
            {
                Assert.IsTrue(result);
            }

            Assert.IsTrue(masterOriginatorUtcTimestamp == TrackingContext.Current.OriginatorUtcTimestamp);
        }

        [TestMethod]
        public async Task TrackingContext_MultipleTasks_ExtraHeadersPersists()
        {
            var extraHeaders = new Dictionary<string, string>()
            {
                { "AAA", "AAA" },
                { "BBB", "BBB" },
            };

            TrackingContext.NewCurrentIfEmpty(extraHeaders);

            List<KeyValuePair<string, string>> masterExtraHeaders = TrackingContext.Current.ExtraHeaders.OrderBy(x => x.Key).ToList();

            var task0 = Task.Run(() =>
            {
                TrackingContext tc = TrackingContext.Current;
                return masterExtraHeaders.SequenceEqual(tc.ExtraHeaders.OrderBy(x => x.Key).ToList());
            });
            var task1 = Task.Run(() =>
            {
                TrackingContext tc = TrackingContext.Current;
                return masterExtraHeaders.SequenceEqual(tc.ExtraHeaders.OrderBy(x => x.Key).ToList());
            });
            var task2 = Task.Run(() =>
            {
                TrackingContext tc = TrackingContext.Current;
                return masterExtraHeaders.SequenceEqual(tc.ExtraHeaders.OrderBy(x => x.Key).ToList());
            });
            var tasks = new OrderedTaskCollection<bool>(
                new[]
                {
                    task0,
                    task1,
                    task2,
                });
            var results = new List<bool>();

            foreach (Task<bool> task in tasks)
            {
                results.Add(await task);
            }

            foreach (bool result in results)
            {
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TrackingContext_NestedMultipleTasks_CallChainIdPersists()
        {
            TrackingContext.NewCurrentIfEmpty();

            Guid masterCallChainId = TrackingContext.Current.CallChainId;

            bool result0 = false;
            bool result1 = false;
            bool result2 = false;
            bool result3 = false;
            bool result4 = false;

            await Task.Run(async () =>
            {
                TrackingContext tc0 = TrackingContext.Current;
                result0 = masterCallChainId == tc0.CallChainId;

                await Task.Run(async () =>
                {
                    TrackingContext tc1 = TrackingContext.Current;
                    result1 = masterCallChainId == tc1.CallChainId;

                    await Task.Run(() =>
                    {
                        TrackingContext tc2 = TrackingContext.Current;
                        result2 = masterCallChainId == tc2.CallChainId;
                    });
                });

                await Task.Run(async () =>
                {
                    TrackingContext tc3 = TrackingContext.Current;
                    result3 = masterCallChainId == tc3.CallChainId;

                    await Task.Run(() =>
                    {
                        TrackingContext tc4 = TrackingContext.Current;
                        result4 = masterCallChainId == tc4.CallChainId;
                    });
                });
            });

            Assert.IsTrue(result0);
            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsTrue(result4);
            Assert.IsTrue(masterCallChainId == TrackingContext.Current.CallChainId);
        }

        [TestMethod]
        public async Task TrackingContext_NestedMultipleTasks_CallChainIdCorresponds()
        {
            TrackingContext.NewCurrentIfEmpty();

            Guid masterCallChainId = TrackingContext.Current.CallChainId;

            bool result0 = false;
            bool result1 = false;
            bool result2 = false;
            bool result3 = false;
            bool result4 = false;
            bool result5 = false;
            bool result6 = false;
            bool result7 = false;
            bool result8 = false;
            bool result9 = false;
            bool result10 = false;
            bool result11 = false;

            await Task.Run(async () =>
            {
                TrackingContext tc0 = TrackingContext.Current;
                result0 = masterCallChainId == tc0.CallChainId;

                await Task.Run(async () =>
                {
                    TrackingContext tc1 = TrackingContext.Current;
                    result1 = masterCallChainId == tc1.CallChainId;

                    TrackingContext.NewCurrent();

                    TrackingContext tc2 = TrackingContext.Current;

                    result2 = tc1.CallChainId != tc2.CallChainId && tc2.CallChainId != Guid.Empty;

                    await Task.Run(() =>
                    {
                        TrackingContext tc3 = TrackingContext.Current;
                        result3 = tc2.CallChainId == tc3.CallChainId;

                        TrackingContext.NewCurrent();

                        TrackingContext tc4 = TrackingContext.Current;

                        result4 = tc3.CallChainId != tc4.CallChainId && tc4.CallChainId != Guid.Empty;
                    });

                    TrackingContext tc5 = TrackingContext.Current;

                    result5 = tc2.CallChainId == tc5.CallChainId;
                });

                await Task.Run(async () =>
                {
                    TrackingContext tc1 = TrackingContext.Current;
                    result6 = masterCallChainId == tc1.CallChainId;

                    TrackingContext.NewCurrent();

                    TrackingContext tc2 = TrackingContext.Current;

                    result7 = tc1.CallChainId != tc2.CallChainId && tc2.CallChainId != Guid.Empty;

                    await Task.Run(() =>
                    {
                        TrackingContext tc3 = TrackingContext.Current;
                        result8 = tc2.CallChainId == tc3.CallChainId;

                        TrackingContext.NewCurrent();

                        TrackingContext tc4 = TrackingContext.Current;

                        result9 = tc3.CallChainId != tc4.CallChainId && tc4.CallChainId != Guid.Empty;
                    });

                    TrackingContext tc5 = TrackingContext.Current;

                    result10 = tc2.CallChainId == tc5.CallChainId;
                });

                TrackingContext tc6 = TrackingContext.Current;
                result11 = tc0.CallChainId == tc6.CallChainId;
            });

            Assert.IsTrue(result0);
            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsTrue(result4);
            Assert.IsTrue(result5);

            Assert.IsTrue(result6);
            Assert.IsTrue(result7);
            Assert.IsTrue(result8);
            Assert.IsTrue(result9);
            Assert.IsTrue(result10);
            Assert.IsTrue(result11);
            Assert.IsTrue(masterCallChainId == TrackingContext.Current.CallChainId);
        }

        [TestMethod]
        public void TrackingContext_MultipleParallelTasks_NewContextAlwaysCreated()
        {
            var results = new ConcurrentStack<bool>();
            var nullChecks = new ConcurrentStack<bool>();

            var runningTasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                runningTasks.Add(Task.Run(() =>
                {
                    TrackingContext.ClearCurrent();
                    Assert.IsNull(TrackingContext.Current);

                    ParallelTester.Test(i, results, nullChecks);
                }));
            }

            Task.WaitAll(runningTasks.ToArray());

            foreach (bool result in results)
            {
                Assert.IsFalse(result);
            }

            foreach (bool nullCheck in nullChecks)
            {
                Assert.IsFalse(nullCheck);
            }
        }



        public class ParallelTester
        {
            public static void Test(int i, ConcurrentStack<bool> results, ConcurrentStack<bool> nullChecks)
            {
                Task.Delay(100 * i).GetAwaiter().GetResult();

                TrackingContext.NewCurrent();

                Guid masterCallChainId = TrackingContext.Current.CallChainId;

                Task.Run(() =>
                {
                    Task.Delay(200 * i).GetAwaiter().GetResult();

                    TrackingContext.NewCurrent();

                    Guid newCallChainId = TrackingContext.Current.CallChainId;

                    results.Push(newCallChainId == masterCallChainId);
                    nullChecks.Push(newCallChainId == Guid.Empty);
                }).GetAwaiter().GetResult();

                Assert.IsNotNull(TrackingContext.Current);
                Assert.IsTrue(masterCallChainId == TrackingContext.Current.CallChainId);
            }
        }
    }
}
