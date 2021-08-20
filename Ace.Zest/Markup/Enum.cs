using System;
using System.ComponentModel;
using Ace.Markup.Patterns;
#if XAMARIN
using Xamarin.Forms.Xaml;
#else
using System.Windows.Markup;
#endif

namespace Ace.Markup
{
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