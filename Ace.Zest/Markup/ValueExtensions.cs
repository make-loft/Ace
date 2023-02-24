using Ace.Markup.Patterns;
#if XAMARIN
using Xamarin.Forms;
#else
using System.Windows.Markup;
#endif

namespace Ace.Markup
{
	[ContentProperty("Value")]
	public abstract class ValueExtension<TValue> : AMarkupExtension
	{
		public TValue Value { set => BoxedValue = value; }

		protected object BoxedValue;
		public override object Provide(object targetObject, object targetProperty) => BoxedValue;
	}

	public class Int32Extension : ValueExtension<int>
	{
		public Int32Extension(int value) => Value = value;
		public Int32Extension() { }
	}

	public class SingleExtension : ValueExtension<float>
	{
		public SingleExtension(float value) => Value = value;
		public SingleExtension() { }
	}

	public class DoubleExtension : ValueExtension<double>
	{
		public DoubleExtension(double value) => Value = value;
		public DoubleExtension() { }
	}

	public class StringExtension : ValueExtension<string>
	{
		public StringExtension(string value) => Value = value;
		public StringExtension() { }
	}

	public class NullExtension : AMarkupExtension
	{
		public static readonly object BoxedValue = null;
		public override object Provide(object targetObject, object targetProperty) => BoxedValue;
	}

	public class FalseExtension : AMarkupExtension
	{
		public static readonly object BoxedValue = false;
		public override object Provide(object targetObject, object targetProperty) => BoxedValue;
	}

	public class TrueExtension : AMarkupExtension
	{
		public static readonly object BoxedValue = true;
		public override object Provide(object targetObject, object targetProperty) => BoxedValue;
	}

#if DESKTOP
	public class VisibleExtension : AMarkupExtension
	{
		public static readonly object BoxedValue = System.Windows.Visibility.Visible;
		public override object Provide(object targetObject, object targetProperty) => BoxedValue;
	}

	public class CollapsedExtension : AMarkupExtension
	{
		public static readonly object BoxedValue = System.Windows.Visibility.Collapsed;
		public override object Provide(object targetObject, object targetProperty) => BoxedValue;
	}

	public class HiddenExtension : AMarkupExtension
	{
		public static readonly object BoxedValue = System.Windows.Visibility.Hidden;
		public override object Provide(object targetObject, object targetProperty) => BoxedValue;
	}
#endif
}