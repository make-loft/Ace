#if XAMARIN
using Xamarin.Forms;
using Property = Xamarin.Forms.BindableProperty;
#else
using Property = System.Windows.DependencyProperty;
using System.Windows;
using System.Windows.Markup;

using View = System.Windows.FrameworkElement;
using Label = System.Windows.Controls.TextBlock;
using ContentView = System.Windows.Controls.ContentPresenter;
#endif

namespace Ace.Controls
{
	[ContentProperty(nameof(Item))]
	public class Slot : ContentView
	{
		public Slot() => this.ContextChanged(args => ApplyContent());

		public static readonly Property ItemTemplateProperty
			= Type<Slot>.Create(s => s.ItemTemplate, args => args.Sender.ApplyContent());

		public static readonly Property ItemProperty
			= Type<Slot>.Create(s => s.Item, args => args.Sender.ApplyContent());

		public DataTemplate ItemTemplate
		{
			get => GetValue(ItemTemplateProperty).To<DataTemplate>();
			set => SetValue(ItemTemplateProperty, value);
		}

		public object Item
		{
			get => GetValue(ItemProperty);
			set => SetValue(ItemProperty, value);
		}

		private void ApplyContent() => Content = (Item ?? this.GetContext()).Is(out var item) && ItemTemplate.Is(out var template)
			? template.CreateView(item)
			: item.Is(out View view) ? view : new Label { Text = item?.ToString() };
	}
}
