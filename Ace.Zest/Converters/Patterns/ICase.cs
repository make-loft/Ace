﻿namespace Ace.Converters.Patterns
{
	public interface ICase<TKey, TValue>
	{
		TKey Key { get; set; }
		TValue Value { get; set; }
		bool MatchByKey(TKey key);
		bool MatchByValue(TValue key);
	}
}