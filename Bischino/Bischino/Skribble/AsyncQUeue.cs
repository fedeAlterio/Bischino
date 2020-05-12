using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Bischino.Skribble
{
    public class AsyncQUeue<T>
    {
        protected readonly object Locker = new object();
        public bool IsWaiting{ get; set; } = false;

        protected readonly IList<TaskCompletionWrapper<T>> TasksSource = new List<TaskCompletionWrapper<T>> { new TaskCompletionWrapper<T>() };

        public Task<T> PopAsync()
        {
            lock (Locker)
            {
                var taskWrapper = TasksSource[0];
                taskWrapper.Used = true;
                var ret = taskWrapper.TaskCompletionSource.Task;
                CheckTaskCompleted();
                return ret;
            }
        }


        public int ActualCount()
        {
            lock(Locker)
            {
                return (from taskSource in TasksSource where taskSource.Notified select taskSource).Count();
            }
        }

        public virtual void Add(T t)
        {
            lock (Locker)
            {
                if (TasksSource.Last().Notified)
                    TasksSource.Add(new TaskCompletionWrapper<T>());
                var last = TasksSource.Last();
                last.Notified = true;
                last.TaskCompletionSource.SetResult(t);
                CheckTaskCompleted();
            }
        }

        protected void CheckTaskCompleted()
        {
            var taskWrapper = TasksSource[0];
            if (taskWrapper.Notified && taskWrapper.Used)
            {
                TasksSource.RemoveAt(0);
                if (!TasksSource.Any())
                    TasksSource.Add(new TaskCompletionWrapper<T>());
            }
        }
    }

    public class TaskCompletionWrapper<T>
    {
        public TaskCompletionSource<T> TaskCompletionSource { get; } = new TaskCompletionSource<T>();
        public bool Used { get; set; }
        public bool Notified { get; set; }
    }
}

