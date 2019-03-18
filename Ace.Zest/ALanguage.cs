using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace Ace
{
	public abstract class ALanguage<T> : ResourceManager where T : ResourceManager, new()
	{
		public static T ResourceManager = new T();

		protected ALanguage() => _keyToValue = GetDictionary();

		private readonly Dictionary<string, string> _keyToValue;

		public override string BaseName => GetType().Name;

		public abstract Dictionary<string, string> GetDictionary();

		public override string GetString(string key) =>
			_keyToValue.TryGetValue(key, out var value) ? value : default;

		public override string GetString(string key, CultureInfo culture) =>
			_keyToValue.TryGetValue(key, out var value) ? value : default;
	}
}