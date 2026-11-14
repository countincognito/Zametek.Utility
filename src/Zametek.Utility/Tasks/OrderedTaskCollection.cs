using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility
{
    public class OrderedTaskCollection<T>
       : IEnumerable<Task<T>>
    {
        #region Fields

        private readonly object m_Lock = new object();
        private readonly Queue<TaskCompletionSource<T>> m_CompletionSources;
        private readonly IList<Task<T>> m_PendingTasks;
        private readonly Queue<Task<T>> m_CompletedTasks;

        #endregion

        #region Ctors

        public OrderedTaskCollection(IEnumerable<Task<T>> tasks)
        {
            if (tasks is null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }
            // The sequence is read once: a lazy sequence can give new tasks each time it is read.
            Task<T>[] allTasks = tasks.ToArray();
            m_PendingTasks = new List<Task<T>>(allTasks);
            m_CompletedTasks = new Queue<Task<T>>();
            m_CompletionSources = new Queue<TaskCompletionSource<T>>();
            foreach (Task<T> task in allTasks)
            {
                // The continuation runs at once on the thread that completes the task, and not later from the queue of
                // the thread pool. Thus, the tasks come in the order in which they complete, also when the pool is busy.
                // A task that is already complete is recorded here, before the first enumeration.
                task.ContinueWith(item =>
                {
                    lock (m_Lock)
                    {
                        m_PendingTasks.Remove(item);
                        if (m_CompletionSources.Any())
                        {
                            SetResult(m_CompletionSources.Dequeue(), item);
                        }
                        else
                        {
                            m_CompletedTasks.Enqueue(item);
                        }
                    }
                }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            }
        }

        #endregion

        #region Properties

        private bool HasTasks
        {
            get
            {
                lock (m_Lock)
                {
                    return m_PendingTasks.Any() || m_CompletedTasks.Any();
                }
            }
        }

        #endregion

        #region Private Methods

        private Task<T> NextOrderedTask()
        {
            lock (m_Lock)
            {
                // The code that awaits the task continues later, and not inside SetResult. Thus, it does not run inside the
                // lock, or on the thread that completed the task.
                var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
                if (m_CompletedTasks.Any())
                {
                    Task<T> completedTask = m_CompletedTasks.Dequeue();
                    SetResult(tcs, completedTask);
                }
                else
                {
                    m_CompletionSources.Enqueue(tcs);
                }
                return tcs.Task;
            }
        }

        private static void SetResult(
            TaskCompletionSource<T> tcs,
            Task<T> task)
        {
            if (tcs is null)
            {
                throw new ArgumentNullException(nameof(tcs));
            }
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            if (task.IsFaulted)
            {
                tcs.SetException(task.Exception);
                return;
            }
            if (task.IsCanceled)
            {
                tcs.SetCanceled();
                return;
            }
            tcs.SetResult(task.Result);
        }

        #endregion

        #region IEnumerable<Task<T>>

        public IEnumerator<Task<T>> GetEnumerator()
        {
            while (HasTasks)
            {
                yield return NextOrderedTask();
            }
        }

        #endregion

        #region IEnumerable

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
