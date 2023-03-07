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
		public List()
		{
			Items.CollectionChanged += (o, e) => ItemsSource = Items;
			Items.CollectionChanged += (o, e) => ActiveItem = 0 <= ActiveItemOffset && ActiveItemOffset < ItemsSource.Count
				? ItemsSource[ActiveItemOffset]
				: ActiveItem;
		}

		public SmartSet<object> Items { get; } = new();

		public static readonly Property ContentTemplateProperty = Create(v => v.ContentTemplate);
		public static readonly Property ItemTemplateProperty = Create(v => v.ItemTemplate);
		public static readonly Property ItemsSourceProperty = Create(v => v.ItemsSource);

		public static readonly Property ActiveItemOffsetProperty = Create(v => v.ActiveItemOffset, args =>
		{
			var value = args.NewValue;
			var items = args.Sender.ItemsSource;
			var activeItem = 0 <= value && value < items.Count ? items[value] : default;
			if (args.Sender.ActiveItem.IsNot(activeItem) && activeItem.IsNot(default))
				args.Sender.ActiveItem = activeItem;
		});

		public static readonly Property ActiveItemProperty = Create(v => v.ActiveItem, args =>
		{
			var value = args.NewValue;
			var items = args.Sender.ItemsSource;
			var activeItemOffset = items.Is() ? items.IndexOf(value) : -1;
			if (args.Sender.ActiveItemOffset.IsNot(activeItemOffset))
				args.Sender.ActiveItemOffset = activeItemOffset;
		});

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

		public IList ItemsSource
		{
			get => Get<IList>(ItemsSourceProperty);
			set => Set(ItemsSourceProperty, value);
		}

		public int ActiveItemOffset
		{
			get => Get<int>(ActiveItemOffsetProperty);
			set => Set(ActiveItemOffsetProperty, value);
		}

		public static readonly Property ActiveItemUnsetProperty = Type<List>.Create(p => p.ActiveItemUnset, true);
		public bool ActiveItemUnset
		{
			get => GetValue(ActiveItemUnsetProperty).To<bool>();
			set => SetValue(ActiveItemUnsetProperty, value);
		}

		public static readonly Property ActiveCellProperty = Type<List>.Create(p => p.ActiveCell);
		public ItemCell ActiveCell
		{
			get => GetValue(ActiveCellProperty).To<ItemCell>();
			set => SetValue(ActiveCellProperty, value);
		}
	}
}
