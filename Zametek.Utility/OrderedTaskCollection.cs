using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zametek.Utility
{
    public class OrderedTaskCollection<T>
       : IEnumerable<Task<T>>
    {
        #region Fields

        private object m_Lock = new object();
        private Queue<TaskCompletionSource<T>> m_CompletionSources;
        private IList<Task<T>> m_PendingTasks;
        private Queue<Task<T>> m_CompletedTasks;

        #endregion

        #region Ctors

        public OrderedTaskCollection(IEnumerable<Func<T>> lambdas)
            : this(lambdas.Select(x => Task.Run(x)))
        {
        }

        public OrderedTaskCollection(IEnumerable<Task<T>> tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException("tasks");
            }
            m_PendingTasks = new List<Task<T>>(tasks);
            m_CompletedTasks = new Queue<Task<T>>();
            m_CompletionSources = new Queue<TaskCompletionSource<T>>();
            foreach (Task<T> task in m_PendingTasks)
            {
                task.ContinueWith(x =>
                {
                    lock (m_Lock)
                    {
                        m_PendingTasks.Remove(x);
                        if (m_CompletionSources.Any())
                        {
                            SetResult(m_CompletionSources.Dequeue(), x);
                        }
                        else
                        {
                            m_CompletedTasks.Enqueue(x);
                        }
                    }
                });
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
                throw new ArgumentNullException("tcs");
            }
            if (task == null)
            {
                throw new ArgumentNullException("task");
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
