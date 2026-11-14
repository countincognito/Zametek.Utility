using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
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
            tasks.ShouldNotBeEmpty();
        }

        [Fact]
        public void OrderedTaskCollection_GivenCtor_WhenCalledWithNullTaskList_ThenShouldThrowArgumentNullException()
        {
            Action act = () => new OrderedTaskCollection<int>(null);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public async Task OrderedTaskCollection_GivenForeach_WhenTasksCompleteInTurn_ThenShouldReturnInOrderOfCompletion()
        {
            var source1 = new TaskCompletionSource<int>();
            var source2 = new TaskCompletionSource<int>();
            var tasks = new OrderedTaskCollection<int>(
                new[]
                {
                    source1.Task,
                    source2.Task,
                    Task.FromResult(0),
                });
            var results = new List<int>();

            foreach (Task<int> task in tasks)
            {
                // A source completes only after the collection gives the task that waits for it. The second source
                // completes first, and so the order of completion is not the order of the sequence.
                if (results.Count == 1)
                {
                    source2.SetResult(2);
                }
                else if (results.Count == 2)
                {
                    source1.SetResult(1);
                }
                results.Add(await task);
            }

            results.ShouldBe(new[] { 0, 2, 1 });
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

            act.ShouldThrow<AggregateException>();
        }

        [Fact]
        public async Task OrderedTaskCollection_GivenATaskThatCompletedBeforeTheCollection_WhenEnumerated_ThenTheFirstTaskIsAlreadyComplete()
        {
            var pending = new TaskCompletionSource<int>();
            var tasks = new OrderedTaskCollection<int>(
                new[]
                {
                    pending.Task,
                    Task.FromResult(0),
                });

            Task<int> first = tasks.First();

            first.IsCompleted.ShouldBeTrue();
            (await first).ShouldBe(0);
            pending.SetResult(1);
        }

        [Fact]
        public void OrderedTaskCollection_GivenALazySequence_WhenEnumerated_ThenEachTaskComesOnce()
        {
            int enumerations = 0;

            IEnumerable<Task<int>> lazyTasks()
            {
                enumerations++;
                yield return Task.FromResult(0);
                yield return Task.FromResult(1);
                yield return Task.FromResult(2);
            }

            var tasks = new OrderedTaskCollection<int>(lazyTasks());

            tasks.Take(10).Count().ShouldBe(3);
            enumerations.ShouldBe(1);
        }

        [Fact]
        public async Task OrderedTaskCollection_GivenATaskThatCompletesLater_WhenItCompletes_ThenTheConsumerDoesNotRunInsideTheCompletion()
        {
            var source = new TaskCompletionSource<int>();
            var tasks = new OrderedTaskCollection<int>(
                new[]
                {
                    source.Task,
                });
            Task<int> first = tasks.First();
            int completingThread = Environment.CurrentManagedThreadId;
            bool completing = false;
            bool ranInside = false;
            Task consumer = first.ContinueWith(
                _ => ranInside = completing && Environment.CurrentManagedThreadId == completingThread,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);

            completing = true;
            source.SetResult(1);
            completing = false;

            await consumer;
            ranInside.ShouldBeFalse();
            (await first).ShouldBe(1);
        }
    }
}
