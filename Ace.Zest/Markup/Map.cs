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
		public ResourceDictionary BasedOn
		{
			get => MergedDictionaries.FirstOrDefault();
			set => MergedDictionaries.Add(value);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void EvokePropertyChanged(string propertyName = Binding.IndexerName) =>
			PropertyChanged?.Invoke(this, new(propertyName));
	}
}