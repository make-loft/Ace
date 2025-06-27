using System.Collections;
#if XAMARIN
using Xamarin.Forms;
using Property = Xamarin.Forms.BindableProperty;
#else
using Property = System.Windows.DependencyProperty;
using System.Windows.Markup;
using System.Windows;

using View = System.Windows.Controls.ContentControl;
#endif

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
namespace Ace.Controls
{
	[ContentProperty(nameof(Items))]
	public class List : View
	{
		static List()
		{
			Type<List>.When(v => v.ActiveItemOffset).Changed += args =>
			{
				var value = args.NewValue;
				var items = args.Sender.ItemsSource;
				if (items.IsNot()) return;

				var activeItem = 0 <= value && value < items.Count ? items[value] : default;
				if (args.Sender.ActiveItem.IsNot(activeItem))
					args.Sender.ActiveItem = activeItem;
			};

			Type<List>.When(v => v.ActiveItem).Changed += args =>
			{
				var value = args.NewValue;
				var items = args.Sender.ItemsSource;
				if (items.IsNot()) return;

				if (args.NewValue.IsNot() && args.Sender.ActiveItemUnset.Is(false))
					return;

				var activeItemOffset = items.Is() ? items.IndexOf(value) : -1;
				if (args.Sender.ActiveItemOffset.IsNot(activeItemOffset))
					args.Sender.ActiveItemOffset = activeItemOffset;
			};
		}

		public List()
		{
			Items.CollectionChanged += (o, e) => ItemsSource = Items;
			Items.CollectionChanged += (o, e) => ActiveItem = 0 <= ActiveItemOffset && ActiveItemOffset < ItemsSource.Count
				? ItemsSource[ActiveItemOffset]
				: ActiveItem
				;
		}

		public SmartSet<object> Items { get; } = new();

		[RegisterProperty]
		public DataTemplate ItemTemplate
		{
			get => this.Get(default(DataTemplate));
			set => this.Set(value);
		}

		[RegisterProperty]
		public DataTemplate ContentTemplate
		{
			get => this.Get(default(DataTemplate));
			set => this.Set(value);
		}

		[RegisterProperty]
		public object ActiveItem
		{
			get => this.Get(default(object));
			set => this.Set(value);
		}

		[RegisterProperty]
		public IList ItemsSource
		{
			get => this.Get(default(IList));
			set => this.Set(value);
		}

		[RegisterProperty]
		public int ActiveItemOffset
		{
			get => this.Get(default(int));
			set => this.Set(value);
		}

		[RegisterProperty]
		public bool ActiveItemUnset
		{
			get => this.Get(true);
			set => this.Set(value);
		}

		[RegisterProperty]
		public ItemCell ActiveCell
		{
			get => this.Get(default(ItemCell));
			set => this.Set(value);
		}
	}
}
