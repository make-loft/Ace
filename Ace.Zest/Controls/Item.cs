#if XAMARIN
using Xamarin.Forms;

using Property = Xamarin.Forms.BindableProperty;
#else
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

using View = System.Windows.FrameworkElement;
using Property = System.Windows.DependencyProperty;
#endif

namespace Ace.Controls
{
	public class Item : ContentView
	{
		public static readonly Property HeaderProperty = Type<Item>.Create(v => v.Header);

		public object Header
		{
			get => GetValue(HeaderProperty);
			set => Set(HeaderProperty, value);
		}

		void Set(Property property, object value)
		{
			if (value is Binding binding)
				SetBinding(property, binding);
			else SetValue(property, value);
		}
	}
}
