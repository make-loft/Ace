using System;
using System.ComponentModel;
#if XAMARIN
using Xamarin.Forms.Xaml;
#else
using System.Windows.Markup;
#endif

namespace Ace.Markup
{
	public class Enum : MarkupExtension
	{
		public Enum() => Type = null;
		
		public Enum(Type type) => Type = type;
		
		[TypeConverter(typeof(TypeTypeConverter))]
		public Type Type { get; set; }
		
		public override object ProvideValue(IServiceProvider serviceProvider) =>
			System.Enum.GetValues(Type);
	}
}