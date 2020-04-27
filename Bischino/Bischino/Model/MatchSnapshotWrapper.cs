using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Model
{
    public class MatchSnapshotWrapper
    {
        public readonly object locker = new object();
        public bool IsWaitingForSnapshot { get; set; } = false;

        private IList<TaskCompletionWrapper> _taskSource = new List<TaskCompletionWrapper> { new TaskCompletionWrapper()};

        public Task<MatchSnapshot> PopFirstAsync()
        {
            lock (locker)
            {
                    var taskWrapper = _taskSource[0];
                taskWrapper.Used = true;
                var ret = taskWrapper.TaskCompletionSource.Task;
                CheckTaskCompleted();
                return ret;
            }
        }

        public void NotifyNew(MatchSnapshot matchSnapshot)
        {
            lock (locker)
            {
                if(_taskSource.Last().Notified)
                    _taskSource.Add(new TaskCompletionWrapper());
                var last = _taskSource.Last();
                last.Notified = true;
                last.TaskCompletionSource.SetResult(matchSnapshot);
                CheckTaskCompleted();
            }
        }

        private void CheckTaskCompleted()
        {
            var taskWrapper = _taskSource[0];
            if (taskWrapper.Notified && taskWrapper.Used)
            {
                _taskSource.RemoveAt(0);
                if(!_taskSource.Any())
                    _taskSource.Add(new TaskCompletionWrapper());
            }
        }
    }

    public class TaskCompletionWrapper
    {
        public TaskCompletionSource<MatchSnapshot> TaskCompletionSource { get; } = new TaskCompletionSource<MatchSnapshot>();
        public bool Used { get; set; }
        public bool Notified { get; set; }
    }
}
