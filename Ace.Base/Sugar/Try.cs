using System;

namespace Ace
{
	public static class Try
	{
		public static bool Invoke(this Action action) => Invoke(action, out _);
		public static bool Invoke(this Action action, out Exception exception)
		{
			try
			{
				exception = default;
				action.Invoke();
				return true;
			}
			catch (Exception e)
			{
				exception = e;
				return false;
			}
		}

		public static bool Invoke<TReturn>(this Func<TReturn> func, out TReturn result) => Invoke(func, out result, out _);
		public static bool Invoke<TReturn>(this Func<TReturn> func, out TReturn result, out Exception exception)
		{
			try
			{
				result = func.Invoke();
				exception = default;
				return true;
			}
			catch (Exception e)
			{
				result = default;
				exception = e;
				return false;
			}
		}
	}
}
