// ReSharper disable RedundantUsingDirective
using System;
using System.ComponentModel;
using Ace.Adapters;
using TypeConverter = Xamarin.Forms.TypeConverterAttribute;
using TypeTypeConverter = Xamarin.Forms.TypeTypeConverter;

namespace Ace.Markup
{
	public class TypeExtension : Patterns.AMarkupExtension
	{
		public TypeExtension() => Key = null;
		
		public TypeExtension(Type key) => Key = key;
		
		[TypeConverter(typeof(TypeTypeConverter))]
		public Type Key { get; set; }

		public override object Provide(object targetObject, object targetProperty = null) => Key;
	}
}