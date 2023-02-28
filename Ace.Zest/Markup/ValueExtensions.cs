using Ace.Markup.Patterns;

using System.Windows;
#if XAMARIN
using Xamarin.Forms;
#else
using System.Windows.Markup;
using System.Windows.Media;
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

	public class ColorExtension : ValueExtension<Color>
	{
		public ColorExtension(Color value) => Value = value;
		public ColorExtension() { }
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

	public class VisibilityExtension : ValueExtension<Visibility>
	{
		public VisibilityExtension(Visibility value) => Value = value;
		public VisibilityExtension() { }
	}

	public class ThicknessExtension : ValueExtension<Thickness>
	{
		public ThicknessExtension(Thickness value) => Value = value;
		public ThicknessExtension() { }
	}
#if XAMARIN
	public class FontFamilyExtension : ValueExtension<string>
	{
		public FontFamilyExtension(string value) => Value = value;
		public FontFamilyExtension() { }
	}
#endif
}