using System.Windows.Data;

namespace Ace.Controls;

#if XAMARIN
using Binding = Xamarin.Forms.Binding;
#endif
#if MAUI
using Binding = Microsoft.Maui.Controls.Binding;
#endif

public class Item : ContentControl
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
