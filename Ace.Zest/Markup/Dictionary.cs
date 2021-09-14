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
	}
}