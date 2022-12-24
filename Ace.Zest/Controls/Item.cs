using Xamarin.Forms;

namespace Ace.Controls
{
	[ContentProperty(nameof(Content))]
	public class Item : TemplatedView
	{
		public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(object), typeof(Item));
		public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(object), typeof(Item));
		public static readonly BindableProperty ContentTemplateProperty = BindableProperty.Create(nameof(ContentTemplate), typeof(DataTemplate), typeof(Item),
			propertyChanged: (s, o, n) => s.To<Item>().ApplyContent());

		public object Header
		{
			get => GetValue(HeaderProperty);
			set => Set(HeaderProperty, value);
		}

		public object Content
		{
			get => GetValue(ContentProperty);
			set => Set(ContentProperty, value);
		}

		public DataTemplate ContentTemplate
		{
			get => GetValue(ContentTemplateProperty).To<DataTemplate>();
			set => SetValue(ContentTemplateProperty, value);
		}

		void Set(BindableProperty property, object value)
		{
			if (value is Binding binding)
				SetBinding(property, binding);
			else SetValue(property, value);
		}

		private void ApplyContent() => Content = Content.Is(out var item) && ContentTemplate.Is(out var template)
			? template.CreateContent().To(out View view).With(view.BindingContext = item)
			: item.Is(out view) ? view : new Label { Text = item?.ToString() };
	}

}
