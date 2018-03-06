using Ace.Markup.Patterns;

namespace Ace.Markup
{
	public class MarkupWrapper<T> : AMarkupExtension
	{
		public T Value { get; set; }

		public override object Provide(object targetObject, object targetProperty = null) => Value;
	}

#if XAMARIN
	public class GridLength : MarkupWrapper<Xamarin.Forms.GridLength> { }
	public class LayoutOptions : MarkupWrapper<Xamarin.Forms.LayoutOptions> { }
#else
#endif
}