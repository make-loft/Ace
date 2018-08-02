using System;

namespace Ace.Sugar
{
	public static class Lock
	{
		public static void Invoke(Action action)
		{
			lock (action) action();
		}

		public static void Invoke<TSyncContext>(TSyncContext customSyncContext, Action action)
		{
			lock (customSyncContext) action();
		}

		public static TResult Invoke<TResult>(Func<TResult> func)
		{
			lock (func) return func();
		}

		public static TResult Invoke<TSyncContext, TResult>(TSyncContext customSyncContext, Func<TResult> func)
		{
			lock (customSyncContext) return func();
		}
	}
}