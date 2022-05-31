using Ace.Markup.Patterns;

namespace Ace.Markup
{
	public abstract class ValueExtension : AMarkupExtension
	{
		protected object BoxedValue;
		public override object Provide(object targetObject, object targetProperty) => BoxedValue;
	}

	public class Int32Extension : ValueExtension
	{
		public int Value { set => BoxedValue = value; }
	}

	public class SingleExtension : ValueExtension
	{
		public float Value { set => BoxedValue = value; }
	}

	public class DoubleExtension : ValueExtension
	{
		public double Value { set => BoxedValue = value; }
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