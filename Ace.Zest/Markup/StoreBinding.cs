using System;

#if XAMARIN
using Xamarin.Forms;
#else
using System.ComponentModel;
using System.Windows.Markup;
#endif

namespace Ace.Markup
{
	[ContentProperty(nameof(Path))]
	public class StoreBinding : System.Windows.Data.Binding
	{
		public StoreBinding() { }
		public StoreBinding(string path) => Path = new(path);

		[TypeConverter(typeof(TypeTypeConverter))]
		public Type StoreKey
		{
			get => Source?.GetType();
			set => Source = Ace.Store.Get(value);
		}
	}
}