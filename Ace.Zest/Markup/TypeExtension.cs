using System;

#if XAMARIN
using Xamarin.Forms;
#else
using System.ComponentModel;
using System.Windows.Markup;
#endif

namespace Ace.Markup
{
	[ContentProperty(nameof(Key))]
	public class TypeExtension : Patterns.AMarkupExtension
	{
		public TypeExtension() => Key = default;
		
		public TypeExtension(Type key) => Key = key;
		
		[TypeConverter(typeof(TypeTypeConverter))]
		public Type Key { get; set; }

		public override object Provide(object targetObject, object targetProperty = null) => Key;
	}
}