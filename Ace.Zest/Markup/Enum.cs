using System;

using Ace.Markup.Patterns;

#if XAMARIN
using Xamarin.Forms;
#else
using System.ComponentModel;
using System.Windows.Markup;
#endif

namespace Ace.Markup
{
	[ContentProperty(nameof(Type))]
	public class Enum : AMarkupExtension
	{
		public Enum() => Type = default;
		
		public Enum(Type type) => Type = type;
		
		[TypeConverter(typeof(TypeTypeConverter))]
		public Type Type { get; set; }
		
		public override object Provide(object targetObject, object targetProperty = default) =>
			System.Enum.GetValues(Type);
	}
}