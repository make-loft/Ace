#if !XAMARIN
using System.Windows.Data;
using System.Windows.Markup;

using Binding = System.Windows.Data.Binding;

namespace Ace.Markup;

[ContentProperty(nameof(ElementName))]
public class Reference : Binding
{
	public Reference() => Mode = BindingMode.OneWay;
	public Reference(string elementName) : this() => ElementName = elementName;
}
#endif
