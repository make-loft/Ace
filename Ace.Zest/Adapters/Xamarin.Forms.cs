// ReSharper disable once CheckNamespace
namespace Xamarin.Forms
{
	// ReSharper disable EmptyNamespace
	namespace Xaml
	{
		public enum XamlCompilationOptions { Skip, Compile }

		public class XamlCompilationAttribute : System.Attribute
		{
			public XamlCompilationAttribute(XamlCompilationOptions options) { }
		}
	}

	internal class ContentPropertyAttribute { }
	internal class TypeConverterAttribute { }
	internal class TypeTypeConverter { }
	internal enum BindingMode { }

	public class DataTemplate : System.Windows.DataTemplate { }
	public class ControlTemplate : System.Windows.Controls.ControlTemplate { }
	public class ResourceDictionary : System.Windows.ResourceDictionary { }
	public class StaticResourceExtension : System.Windows.StaticResourceExtension
	{
		public StaticResourceExtension(string key) : base(key) { }
	}
	public class DynamicResourceExtension : System.Windows.DynamicResourceExtension { }
	public class Binding : System.Windows.Data.Binding
	{
		public Binding() { }
		public Binding(string path) : base(path) { }
	}

	public class Border : System.Windows.Controls.Border { }
	public class Button : System.Windows.Controls.Button
	{
		public bool InputTransparent { get; set; }
	}
}

namespace System.Windows
{
	internal static class DependencyAdapter
	{
		public static DependencyProperty Unbox(this DependencyProperty property) => property;
	}
}