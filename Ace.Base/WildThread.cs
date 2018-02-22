using System;
using System.ComponentModel;
using System.Threading;

namespace Foundation
{
    public class WildThread : IDisposable
    {
        private Thread _thread;
        private Action _action = () => { };
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(true);
        private bool _isRunning = true;
        [DefaultValue(ThreadPriority.Lowest)]
        public ThreadPriority Priority { get; set; }

        public void Run(Action action)
        {
            _action = action;
            if (_isRunning)
            {
                if (_thread != null) _thread.Abort();
                _thread = new Thread(Invoke) {IsBackground = true, Priority = Priority};
                _thread.Start();
            }

            _autoResetEvent.Set();
        }

        private void Invoke()
        {
            do
            {
                _isRunning = false;
                _autoResetEvent.WaitOne();
                _isRunning = true;
                _action();
            } while (true);
            // ReSharper disable once FunctionNeverReturns
        }

        public void Dispose()
        {
            if (_thread == null) return;
            _thread.Abort();
            _thread = null;
        }
    }
}
