// ReSharper disable RedundantUsingDirective
using System;
using System.ComponentModel;
using TypeConverter = Xamarin.Forms.TypeConverterAttribute;
using TypeTypeConverter = Xamarin.Forms.TypeTypeConverter;

namespace Ace.Markup
{
	public class StoreBinding : System.Windows.Data.Binding
	{
		[TypeConverter(typeof(TypeTypeConverter))]
		public Type StoreKey
		{
			get => Source?.GetType();
			set => Source = Ace.Store.Get(value);
		}
	}
}