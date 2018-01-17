// ReSharper disable once CheckNamespace
namespace Xamarin.Forms
{
	// ReSharper disable EmptyNamespace
	namespace Xaml { }
	internal class ContentPropertyAttribute { }
	internal class TypeConverterAttribute { }
	internal class TypeTypeConverter { }
	internal enum BindingMode { }
}

namespace System.Windows
{
	internal static class DependencyAdapter
	{
		public static DependencyProperty Unbox(this DependencyProperty property) => property;
	}
}