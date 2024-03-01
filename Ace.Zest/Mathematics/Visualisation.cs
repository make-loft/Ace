using System;
using System.Threading.Tasks;

namespace Ace.Mathematics
{
	public static class Visualisation
	{
		public static async void Animate(this Action<double> action,
			int durationMilliseconds,
			int framesCount = 64,
			double from = 0d,
			double till = 1d,
			Func<double, double> change = default,
			Func<bool> repeat = default,
			Action finish = default)
		{
			var step = (till - from) / framesCount;
			var frameDurationMilliseconds = durationMilliseconds / framesCount;
			var projection = change.Is() ? change : (x => x);

			do
			{
				for (var value = from; value <= till; value += step)
				{
					action(projection(value));
					await Task.Delay(frameDurationMilliseconds);
				}
			} while (repeat?.Invoke() is true);

			finish?.Invoke();
		}

		public static async void Animate(Action<double> action,
			double from,
			double till,
			int rate = 4,
			int length = 192)
		{
			var framesCount = (double)length / rate;
			var step = 1d / framesCount;

			var len = till - from;
			var x = 0d;
			var i = 0;

			do
			{
				var scale = 1d - Math.Pow(1d - x, 3d);
				action(from + scale * len);
				await Task.Delay(rate);
				x += step;
			}
			while (i++ < framesCount);
		}
	}
}
