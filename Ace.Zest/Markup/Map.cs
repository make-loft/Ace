using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
#if XAMARIN
using ResourceDictionary = Xamarin.Forms.ResourceDictionary;
#else
using System.Windows;
#endif

namespace Ace.Markup
{
	public class Map : ResourceDictionary, INotifyPropertyChanged
	{
		public Map() { }
		public Map(ResourceDictionary source) =>
			EnumerateResources(source).ToList().ForEach(p => this[p.Key] = p.Value);

		public ResourceDictionary BasedOn
		{
			get => MergedDictionaries.LastOrDefault();
			set => MergedDictionaries.Add(value);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void EvokePropertyChanged(string propertyName = Binding.IndexerName) =>
			PropertyChanged?.Invoke(this, new(propertyName));

		public static IEnumerable<KeyValuePair<string, object>> EnumerateResources(ResourceDictionary dictionary)
		{
			foreach (var d in dictionary.MergedDictionaries)
			{
				foreach(var pair in EnumerateResources(d))
				{
					yield return pair;
				}
			}
#if XAMARIN
			foreach (KeyValuePair<string, object> pair in dictionary)
			{
				yield return pair;
			}
#else

			foreach (System.Collections.DictionaryEntry pair in dictionary)
			{
				yield return new((string)pair.Key, pair.Value);
			}
#endif
		}
#if XAMARIN
		public new bool ContainsKey(string key) => Keys.Contains(key) || MergedDictionaries.Reverse().Any(d => d.Keys.Contains(key));

		public new bool TryGetValue(string key, out object value)
		{

			if (base.TryGetValue(key, out value))
				return true;

			foreach (var map in MergedDictionaries.Reverse())
			{
				if (map.TryGetValue(key, out value))
					return true;
			}

			return false;
		}
		public new object this[string key]
		{
			get => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException(key);
			set => base[key] = value;
		}
#else
		public void ForEach(System.Action<KeyValuePair<string, object>> action) => this
			.Cast<System.Collections.DictionaryEntry>()
			.ToDictionary(e => (string)e.Key, e => e.Value)
			.ToList()
			.ForEach(action);
#endif
	}
}