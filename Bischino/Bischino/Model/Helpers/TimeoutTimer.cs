using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bischino.Model.Helpers
{
    public class TimeoutTimer<T>
    {
        public event EventHandler<T> TimeoutEvent;
        public T Tag { get; }
        private readonly int _timeoutMs;
        private CancellationTokenSource _tokenSource;
        private Task _timerTask;
        private bool _isTimeoutForced;

        public bool IsEnabled { get; private set; }

        public TimeoutTimer(int timeoutMs, T tag)
        {
            Tag = tag;
            _timeoutMs = timeoutMs;
        }

        private async Task TimerRoutine()
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
                if (_isTimeoutForced)
                    TimeoutEvent?.Invoke(this, Tag);
            }
            finally
            {
                IsEnabled = false;
            }
        }

        public void Start()
        {
            _timerTask = TimerRoutine();
        }

        public async Task ForceTimeout()
        {
            _isTimeoutForced = true;
            await Stop();
            _isTimeoutForced = false;
        }

        public async Task Reset()
        {
            await Stop(); 
            Start();
        }

        public async Task Stop()
        {
            _tokenSource?.Cancel();
            await _timerTask;
            _tokenSource = null;
        }

    }
}