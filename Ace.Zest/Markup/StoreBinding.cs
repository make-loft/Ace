using System;

#if XAMARIN
using Xamarin.Forms;
#else
using System.ComponentModel;
#endif

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