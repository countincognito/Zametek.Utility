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
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }
            m_PendingTasks = new List<Task<T>>(tasks);
            m_CompletedTasks = new Queue<Task<T>>();
            m_CompletionSources = new Queue<TaskCompletionSource<T>>();
            foreach (Task<T> task in tasks)
            {
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
                }, TaskScheduler.Default);
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
                var tcs = new TaskCompletionSource<T>();
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

        private static void SetResult(TaskCompletionSource<T> tcs, Task<T> task)
        {
            if (tcs == null)
            {
                throw new ArgumentNullException(nameof(tcs));
            }
            if (task == null)
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
