using System;
using System.Threading;

namespace Ace
{
	public class WildThread : IDisposable
	{
		private Thread _thread;
		private Action _action = () => { };
		private readonly AutoResetEvent _autoResetEvent = new(true);
		public ThreadPriority Priority { get; set; } = ThreadPriority.Lowest;
		private bool _isRunning = true;

		public void Run(Action action)
		{
			_action = action;
			if (_isRunning)
			{
				_thread?.Abort();
				_thread = new(Invoke) {IsBackground = true, Priority = Priority};
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
