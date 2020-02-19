using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Utility.Tests
{
    public class OrderedTaskCollectionTests
    {
        [Fact]
        public void OrderedTaskCollection_GivenCtor_WhenCalledWithTaskList_ThenShouldSucceed()
        {
            var tasks = new OrderedTaskCollection<int>(
                new[]
                {
                    Task.FromResult(1),
                });
            tasks.Should().NotBeEmpty();
        }

        [Fact]
        public void OrderedTaskCollection_GivenCtor_WhenCalledWithNullTaskList_ThenShouldThrowArgumentNullException()
        {
            Action act = () => new OrderedTaskCollection<int>((IEnumerable<Task<int>>)null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task OrderedTaskCollection_GivenForeach_WhenCalledWithTimedTasks_ThenShouldReturnInCorrectOrder()
        {
            var task1 = Task.Run(() =>
            {
                Thread.Sleep(250);
                return 1;
            });
            var task0 = Task.FromResult(0);
            var task2 = Task.Run(() =>
            {
                Thread.Sleep(500);
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
                results.Add(await task);
            }

            results[0].Should().Be(0);
            results[1].Should().Be(1);
            results[2].Should().Be(2);
        }

        [Fact]
        public void OrderedTaskCollection_GivenForeach_WhenCalledWithExceptionTasks_ThenShouldThrowAggregateException()
        {
            static int throwsException()
            {
                throw new Exception();
            }

            Task<int> task0 = Task.Run(throwsException);
            var tasks = new OrderedTaskCollection<int>(
                new[]
                {
                    task0,
                });
            var results = new List<int>();

            Action act = () =>
            {
                foreach (Task<int> task in tasks)
                {
                    results.Add(task.Result);
                }
            };

            act.Should().Throw<AggregateException>();
        }
    }
}
