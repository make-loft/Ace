using System;

namespace Ace.Sugar
{
	public static class Lock<TResult>
	{
		public static readonly object GlobalSyncContext = new object();
	}
	
	public static class Lock
	{
		public static readonly object GlobalSyncContext = new object();
		
		public static void Invoke(Action action)
		{
			lock (GlobalSyncContext) action();
		}

		public static void Invoke<TSyncContext>(TSyncContext customSyncContext, Action<TSyncContext> action)
		{
			lock (customSyncContext) action(customSyncContext);
		}

		public static TResult Invoke<TResult>(Func<TResult> func)
		{
			lock (Lock<TResult>.GlobalSyncContext) return func();
		}

		public static TResult Invoke<TSyncContext, TResult>(TSyncContext customSyncContext, Func<TSyncContext, TResult> func)
		{
			lock (customSyncContext) return func(customSyncContext);
		}
	}
}