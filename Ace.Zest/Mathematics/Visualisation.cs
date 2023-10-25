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
	}
}
