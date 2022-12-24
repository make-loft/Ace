using Xamarin.Forms;

namespace Ace.Controls
{
	[ContentProperty(nameof(Item))]
	public class Slot : ContentView
	{
		public Slot() => BindingContextChanged += (o, e) => ApplyContent();

		public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(Slot),
			propertyChanged: (s, o, n) => s.To<Slot>().ApplyContent());

		public static readonly BindableProperty ItemProperty = BindableProperty.Create(nameof(Item), typeof(object), typeof(Slot),
			propertyChanged: (s, o, n) => s.To<Slot>().ApplyContent());

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

		private void ApplyContent() => Content = (Item ?? BindingContext).Is(out var item) && ItemTemplate.Is(out var template)
			? template.CreateContent().To(out View view).With(view.BindingContext = item)
			: item.Is(out view) ? view : new Label { Text = item?.ToString() };
	}
}
