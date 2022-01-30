#if XAMARIN
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Ace.Markup
{
	public class Dictionary : Xamarin.Forms.ResourceDictionary
	{
		public Dictionary()
		{
			MergedDictionaries = new ObservableCollection<Xamarin.Forms.ResourceDictionary>();
			MergedDictionaries.CollectionChanged += (sender, args) =>
				(args.NewItems ?? new List<object>()).OfType<Xamarin.Forms.ResourceDictionary>()
				.ForEach(this.Merge);
		}

		public new ObservableCollection<Xamarin.Forms.ResourceDictionary> MergedDictionaries { get; }

		public IDictionary SourceDictionary
		{
			set => value.Cast<DictionaryEntry>().ForEach(e => Add(e.Key, e.Value));
		}
	}
}
#else
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Ace.Markup
{
	public class Map : ResourceDictionary, INotifyPropertyChanged
	{
		public ResourceDictionary BasedOn
		{
			get => MergedDictionaries.FirstOrDefault();
			set => MergedDictionaries.Insert(0, value);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void EvokePropertyChanged(string propertyName = Binding.IndexerName) =>
			PropertyChanged?.Invoke(this, new(propertyName));
	}
}
#endif
