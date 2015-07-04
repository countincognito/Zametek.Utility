using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.UnitTests
{
    [TestFixture]
    public class OrderedTaskCollectionTests
    {
        [Test]
        public void OrderedTaskCollection_CtorWhenCalledWithTaskList_ShouldSucceed()
        {
            var tasks = new OrderedTaskCollection<int>(
                new[]
                {
                    Task.FromResult(1),
                });
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrderedTaskCollection_CtorWhenCalledWithNullTaskList_ShouldThrowArgumentNullException()
        {
            var tasks = new OrderedTaskCollection<int>((IEnumerable<Task<int>>)null);
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrderedTaskCollection_CtorWhenCalledWithNullFuncList_ShouldThrowArgumentNullException()
        {
            var tasks = new OrderedTaskCollection<int>((IEnumerable<Func<int>>)null);
            Assert.Fail();
        }

        [Test]
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

            Assert.That(results[0], Is.EqualTo(0));
            Assert.That(results[1], Is.EqualTo(1));
            Assert.That(results[2], Is.EqualTo(2));
        }

        [Test]
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

            Assert.That(results[0], Is.EqualTo(0));
            Assert.That(results[1], Is.EqualTo(1));
            Assert.That(results[2], Is.EqualTo(2));
        }

        [Test]
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
