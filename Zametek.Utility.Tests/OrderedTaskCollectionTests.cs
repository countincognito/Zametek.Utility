using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.UnitTests
{
    [TestClass]
    public class OrderedTaskCollectionTests
    {
        [TestMethod]
        public void OrderedTaskCollection_CtorWhenCalledWithTaskList_ShouldSucceed()
        {
            var tasks = new OrderedTaskCollection<int>(
                new[]
                {
                    Task.FromResult(1),
                });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrderedTaskCollection_CtorWhenCalledWithNullTaskList_ShouldThrowArgumentNullException()
        {
            var tasks = new OrderedTaskCollection<int>((IEnumerable<Task<int>>)null);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrderedTaskCollection_CtorWhenCalledWithNullFuncList_ShouldThrowArgumentNullException()
        {
            var tasks = new OrderedTaskCollection<int>((IEnumerable<Func<int>>)null);
            Assert.Fail();
        }

        [TestMethod]
        public void OrderedTaskCollection_ForeachWhenCalledWithTimedTasks_ShouldReturnInCorrectOrder()
        {
            var task0 = Task.FromResult(0);
            var task1 = Task.Run<int>(() =>
            {
                Thread.Sleep(100);
                return 1;
            });
            var task2 = Task.Run<int>(() =>
            {
                Thread.Sleep(200);
                return 2;
            });
            var tasks = new OrderedTaskCollection<int>(
                new[]
                {
                    task1,
                    task2,
                    task0,
                });
            var results = new List<int>();

            foreach (Task<int> task in tasks)
            {
                results.Add(task.Result);
            }

            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(1, results[1]);
            Assert.AreEqual(2, results[2]);
        }

        [TestMethod]
        public void OrderedTaskCollection_ForeachWhenCalledWithTimedFuncs_ShouldReturnInCorrectOrder()
        {
            var tasks = new OrderedTaskCollection<int>(
               new List<Func<int>>(new Func<int>[]
            {
               () =>
               {
                   Thread.Sleep(100);
                   return 1;
               },
               () => 0,
               () =>
               {
                   Thread.Sleep(200);
                   return 2;
               },
            }));
            var results = new List<int>();

            foreach (Task<int> task in tasks)
            {
                results.Add(task.Result);
            }

            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(1, results[1]);
            Assert.AreEqual(2, results[2]);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void OrderedTaskCollection_ForeachFuncs_ShouldReturnInCorrectOrder()
        {
            var tasks = new OrderedTaskCollection<int>(
               new List<Func<int>>(new Func<int>[]
            {
               () =>
               {
                   throw new Exception();
               },
            }));
            var results = new List<int>();
            foreach (Task<int> task in tasks)
            {
                results.Add(task.Result);
            }
            Assert.Fail();
        }
    }
}
