namespace Ace.Mathematics;

public static class ExtensionsInt32
{
	public static int Clip(this in int value, in int from, in int till) => from < till
		? value < from ? from : till < value ? till : value
		: value > from ? from : till > value ? till : value
		;

	public static bool IsMissIntervalCloseClose(this in int value, in int from, in int till) => from < till
		? till < value || value < from
		: till < value && value < from
		;

	public static int Rotate(this int value, in int step, in int from, in int till)
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
