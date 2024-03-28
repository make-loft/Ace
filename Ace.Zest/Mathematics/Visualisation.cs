using System;
using System.Threading.Tasks;

namespace Ace.Mathematics
{
	public static class Visualisation
	{
		public static async void Animate(
			this Action<double> action,
			int framesCount,
			int frameDuration_Milliseconds = 4,
			double from = 0d,
			double till = 1d,
			Func<double, double> change = default,
			Func<bool> repeat = default,
			Action finish = default)
		{
			action ??= value => { };
			change ??= value => value;

			var length = till - from;
			var shift = length / (framesCount - 1);

			repeat:

			var offset = from;
			for (var i = 0; i < framesCount; i++, offset += shift)
			{
				var value = change(offset);

				action(value);

				await Task.Delay(frameDuration_Milliseconds);
			}

			if (repeat?.Invoke() is true)
				goto repeat;

			finish?.Invoke();
		}
	}
}
