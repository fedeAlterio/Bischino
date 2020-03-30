using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bischino.Helpers
{
    public class TimeoutTimer<T>
    {
        public event EventHandler<T> TimeoutEvent;
        public T Tag { get; }
        private readonly int _timeoutMs;
        private CancellationTokenSource _tokenSource;

        public bool IsEnabled { get; private set; }

        public TimeoutTimer(int timeoutMs, T tag)
        {
            Tag = tag;
            _timeoutMs = timeoutMs;
        }

        public async void Start()
        {
            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;
            IsEnabled = true;
            try
            {
                await Task.Delay(_timeoutMs, _tokenSource.Token);
                TimeoutEvent?.Invoke(this, Tag);
            }
            catch (TaskCanceledException) when (token.IsCancellationRequested)
            {
            }
            finally
            {
                IsEnabled = false;
            }
        }


        public void Reset()
        {
            Stop();
            Start();
        }

        public void Stop()
        {
            _tokenSource?.Cancel();
            _tokenSource = null;
        }
    }
}