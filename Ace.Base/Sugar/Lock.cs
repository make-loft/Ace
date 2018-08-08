using System;

namespace Ace.Sugar
{
	public static class Lock
	{
		public static void Invoke<TSyncContext>(TSyncContext customSyncContext, Action action)
		{
			lock (customSyncContext) action();
		}

		public static TResult Invoke<TSyncContext, TResult>(TSyncContext customSyncContext, Func<TResult> func)
		{
			lock (customSyncContext) return func();
		}
	}
}