namespace Ace.Mathematics
{
	public static class ExtensionsDouble
	{
		public static double Clip(in double value, in double from, in double till) => from < till
			? value < from ? from : till < value ? till : value
			: value > from ? from : till > value ? till : value
			;

		public static bool IsMissIntervalCloseClose(this in double value, in double from, in double till) => from < till
			? till < value || value < from
			: till < value && value < from
			;

		public static double Rotate(this double value, in double step, in double from, in double till)
		{
			value += step;

			var isMissInterval = value.IsMissIntervalCloseClose(from, till);
			if (isMissInterval)
			{
				var length = till - from;

				if (value < from)
					value += length;
				else
				if (value > till)
					value -= length;

				if (from < till)
					value %= length;
			}

			return value;
		}
	}
}
