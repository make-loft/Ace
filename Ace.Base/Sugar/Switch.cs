
namespace Ace;

public class Switch<T>(T value)
{
	private readonly object _value = value;
	private object[] _pattern;

	public Switch(T value, object[] pattern) : this(value) => _pattern = pattern;

	public bool Case(params object[] pattern)
	{
		pattern ??= new[] {(object) null};
		_pattern ??= new[] {_value};
		for (var i = 0; i < pattern.Length && i < _pattern.Length; i++)
		{
			if (Equals(pattern[i], _pattern[i])) continue;
			return false;
		}

		return true;
	}

	public bool Case<TValue>() where TValue : T => _value.Is<TValue>();
	public bool Case(out T value) => Case<T>(out value);

	public bool Case<TValue>(out TValue value, TValue fallbackValue = default) where TValue : T
		=> _value.Is(out value, fallbackValue);
}