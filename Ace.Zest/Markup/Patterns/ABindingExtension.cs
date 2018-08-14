using System;
using System.Globalization;
using System.Windows.Data;

namespace Ace.Markup.Patterns
{
	public abstract class ABindingExtension : Binding, IValueConverter
	{
		protected static object DataContext = null;
		protected static RelativeSource RelativeSelf = new RelativeSource(RelativeSourceMode.Self);
		
		protected ABindingExtension() => Source = Converter = this;

		protected ABindingExtension(object source) // Source, RelativeSource, null for DataContext
		{
			if (source.Is(out RelativeSource relativeSource))
				RelativeSource = relativeSource;
			else if (source.Is()) Source = source;
			Converter = this;
		}

		public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

		public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			throw new NotImplementedException();
	}
}