using System.Collections;

using Xamarin.Forms;

namespace Ace.Controls
{
	[ContentProperty(nameof(Items))]
	public class List : AView<List>
	{
		public SmartSet<object> Items { get; } = new();

		public static readonly BindableProperty ContentTemplateProperty = Create(v => v.ContentTemplate);
		public static readonly BindableProperty ItemTemplateProperty = Create(v => v.ItemTemplate);
		public static readonly BindableProperty ItemsSourceProperty = Create(v => v.ItemsSource);
		public static readonly BindableProperty ActiveItemProperty = Create(v => v.ActiveItem);

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
			get => Get<IEnumerable>(ItemsSourceProperty) ?? Items;
			set => Set(ItemsSourceProperty, value);
		}

		public int ActiveItemOffset
		{
			get => Items.IndexOf(ActiveItem);
			set => ActiveItem = Items[value];
		}
	}
}
