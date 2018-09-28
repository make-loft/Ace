using System;
using System.Threading;

namespace Ace
{
    public class WildThread : IDisposable
    {
        private Thread _thread;
        private Action _action = () => { };
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(true);
        private bool _isRunning = true;
        
        public void Run(Action action, ThreadPriority priority = ThreadPriority.Lowest)
        {
            _action = action;
            if (_isRunning)
            {
	            _thread?.Abort();
	            _thread = new Thread(Invoke) {IsBackground = true, Priority = priority};
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
            _thread?.Abort();
            _thread = null;
        }
    }
}
