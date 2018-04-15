using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

            Assert.AreEqual(true, results[0]);
            Assert.AreEqual(true, results[1]);
            Assert.AreEqual(true, results[2]);
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

            Assert.AreEqual(true, results[0]);
            Assert.AreEqual(true, results[1]);
            Assert.AreEqual(true, results[2]);
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

            Assert.AreEqual(true, results[0]);
            Assert.AreEqual(true, results[1]);
            Assert.AreEqual(true, results[2]);
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
        }
    }
}
