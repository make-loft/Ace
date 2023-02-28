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
	public class Map : ResourceDictionary, INotifyPropertyChanged, IDictionary<string, object>
	{
		public ResourceDictionary BasedOn
		{
			get => MergedDictionaries.LastOrDefault();
			set => MergedDictionaries.Add(value);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void EvokePropertyChanged(string propertyName = Binding.IndexerName) =>
			PropertyChanged?.Invoke(this, new(propertyName));

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
	}
}