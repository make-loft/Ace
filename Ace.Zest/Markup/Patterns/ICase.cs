using System;

namespace Ace.Markup.Patterns
{
	public interface ICase<TKey, TValue>
	{
		TKey Key { get; set; }
		TValue Value { get; set; }
		bool MatchByKey(TKey key, StringComparison comparison);
	}
}