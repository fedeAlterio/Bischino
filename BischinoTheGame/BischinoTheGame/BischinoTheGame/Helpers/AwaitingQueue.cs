using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Map = Xamarin.Essentials.Map;

namespace Rooms.Helpers
{
    public class AwaitingQueue
    {
        public QueueNode Last { get; private set; }

        public Task Add(Func<Task> action)
        {
            var tcs = new TaskCompletionSource<bool>();
            var node = new QueueNode(action);
            if (Last != null)
                Last.CompletedEvent += async (sender, _) => await node.Invoke();
            node.CompletedEvent += (sender, _) =>
            {
                if (Last == sender as QueueNode)
                    Last = null;
                tcs.SetResult(true);
            };
            var oldLast = Last;
            Last = node;
            if (oldLast == null)
                node.Invoke();
            return tcs.Task;

        }
    }

    public class QueueNode
    {
        private readonly Func<Task> _action;
        public event EventHandler CompletedEvent;
        public async Task Invoke()
        {
            await _action.Invoke();
            CompletedEvent?.Invoke(this, EventArgs.Empty);
        }

        public QueueNode(Func<Task> action)
        {
            _action = action;
        }
    }
}
