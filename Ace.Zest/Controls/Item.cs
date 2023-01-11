using Xamarin.Forms;

namespace Ace.Controls
{
	[ContentProperty(nameof(Content))]
	public class Item : TemplatedView
	{
		public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(object), typeof(Item));
		public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(object), typeof(Item));

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

		void Set(BindableProperty property, object value)
		{
			if (value is Binding binding)
				SetBinding(property, binding);
			else SetValue(property, value);
		}
	}
}
