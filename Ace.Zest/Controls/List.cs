using System.Collections;
#if XAMARIN
using Xamarin.Forms;
using Property = Xamarin.Forms.BindableProperty;
#else
using Property = System.Windows.DependencyProperty;
using System.Windows.Markup;
using System.Windows;
#endif

namespace Ace.Controls
{
	[ContentProperty(nameof(Items))]
	public class List : AView<List>
	{
		public List() => ItemsSource = Items;

		public SmartSet<object> Items { get; } = new();

		public static readonly Property ContentTemplateProperty = Create(v => v.ContentTemplate);
		public static readonly Property ItemTemplateProperty = Create(v => v.ItemTemplate);
		public static readonly Property ItemsSourceProperty = Create(v => v.ItemsSource);
		public static readonly Property ActiveItemProperty = Create(v => v.ActiveItem);

		public DataTemplate ItemTemplate
		{
			get => Get<DataTemplate>(ItemTemplateProperty);
			set => Set(ItemTemplateProperty, value);
		}

		public DataTemplate ContentTemplate
		{
			get => Get<DataTemplate>(ContentTemplateProperty);
			set => Set(ContentTemplateProperty, value);
		}

		public object ActiveItem
		{
			get => Get<object>(ActiveItemProperty);
			set => Set(ActiveItemProperty, value);
		}

		public IEnumerable ItemsSource
		{
			get => Get<IEnumerable>(ItemsSourceProperty);
			set => Set(ItemsSourceProperty, value);
		}

		public int ActiveItemOffset
		{
			get => Items.IndexOf(ActiveItem);
			set => ActiveItem = value < Items.Count ? Items[value] : default;
		}
	}
}
