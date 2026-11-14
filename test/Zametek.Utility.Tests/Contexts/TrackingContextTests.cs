using Shouldly;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Utility.Tests
{
    public class TrackingContextTests
    {
        [Fact]
        public void TrackingContext_GivenSerializeAndDeserialize_ThenSuccess()
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

            bytes.ShouldNotBeEmpty();
            tc2.CallChainId.ShouldBe(tc1.CallChainId);
            tc2.OriginatorUtcTimestamp.ShouldBe(tc1.OriginatorUtcTimestamp);
            tc2.ExtraHeaders.ToList().ShouldBe(tc1.ExtraHeaders.ToList(), ignoreOrder: true);
        }

        [Fact]
        public async Task TrackingContext_GivenMultipleTasks_ThenCallChainIdPersists()
        {
            TrackingContext.NewCurrent();

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
                result.ShouldBeTrue();
            }

            TrackingContext.Current.CallChainId.ShouldBe(masterCallChainId);
        }

        [Fact]
        public async Task TrackingContext_GivenMultipleTasks_ThenOriginatorUtcTimestampPersists()
        {
            TrackingContext.NewCurrent();

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
                result.ShouldBeTrue();
            }

            TrackingContext.Current.OriginatorUtcTimestamp.ShouldBe(masterOriginatorUtcTimestamp);
        }

        [Fact]
        public async Task TrackingContext_GivenMultipleTasks_ThenExtraHeadersPersists()
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
                result.ShouldBeTrue();
            }
        }

        [Fact]
        public async Task TrackingContext_GivenNestedMultipleTasks_ThenCallChainIdPersists()
        {
            TrackingContext.NewCurrent();

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
            }, TestContext.Current.CancellationToken);

            result0.ShouldBeTrue();
            result1.ShouldBeTrue();
            result2.ShouldBeTrue();
            result3.ShouldBeTrue();
            result4.ShouldBeTrue();
            TrackingContext.Current.CallChainId.ShouldBe(masterCallChainId);
        }

        [Fact]
        public async Task TrackingContext_GivenNestedMultipleTasks_ThenCallChainIdCorresponds()
        {
            TrackingContext.NewCurrent();

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
            }, TestContext.Current.CancellationToken);

            result0.ShouldBeTrue();
            result1.ShouldBeTrue();
            result2.ShouldBeTrue();
            result3.ShouldBeTrue();
            result4.ShouldBeTrue();
            result5.ShouldBeTrue();
            result6.ShouldBeTrue();
            result7.ShouldBeTrue();
            result8.ShouldBeTrue();
            result9.ShouldBeTrue();
            result10.ShouldBeTrue();
            result11.ShouldBeTrue();
            TrackingContext.Current.CallChainId.ShouldBe(masterCallChainId);
        }

        [Fact]
        public async Task TrackingContext_GivenMultipleParallelTasks_ThenNewContextAlwaysCreated()
        {
            var results = new ConcurrentStack<bool>();
            var nullChecks = new ConcurrentStack<bool>();

            var runningTasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                runningTasks.Add(Task.Run(() =>
                {
                    TrackingContext.ClearCurrent();
                    TrackingContext.Current.ShouldBeNull();

                    ParallelTester.Test(i, results, nullChecks);
                }, TestContext.Current.CancellationToken));
            }

            await Task.WhenAll(runningTasks.ToArray());

            foreach (bool result in results)
            {
                result.ShouldBeFalse();
            }

            foreach (bool nullCheck in nullChecks)
            {
                nullCheck.ShouldBeFalse();
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

                TrackingContext.Current.ShouldNotBeNull();
                TrackingContext.Current.CallChainId.ShouldBe(masterCallChainId);
            }
        }
    }
}
