using System;
using System.Threading.Tasks;

namespace Ace
{
	public class Invoke
	{
		public static async Task Delay(int milliseconds, Action callback)
		{
			await Task.Delay(milliseconds);
			callback?.Invoke();
		}
	}
}
