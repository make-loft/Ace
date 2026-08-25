using System.Threading;

namespace Ace;

public class WildThread : IDisposable
{
	public ThreadPriority Priority { get; set; } = ThreadPriority.Lowest;

	private Thread _thread;

	public void Run(Action action)
	{
		Dispose();

		_thread = new(() => action?.Invoke()) { IsBackground = true, Priority = Priority };
		_thread.Start();
	}

	public void Dispose()
	{
		_thread?.Abort();
		_thread = null;
	}
}
