using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
#if XAMARIN
using FrameworkElement = Xamarin.Forms.Element;
using BindingMode = Xamarin.Forms.BindingMode;
using IValueConverter = System.Windows.Data.IValueConverter;
#endif

namespace Ace
{
	public static class WatcherSugar
	{
#if XAMARIN
		public static FrameworkElement GetDataContextWatcher(this FrameworkElement element) => element;
#else
		public static PropertyChangedWatcher GetDataContextWatcher(this FrameworkElement element)
		{
			var expression = element.GetBindingExpression(PropertyChangedWatcher.WatchedPropertyProperty);
			return expression.IsNot()
				? new PropertyChangedWatcher(element)
				: (PropertyChangedWatcher)expression.ParentBinding.Converter;
		}
#endif
		public static PropertyChangedWatcher GetWatcher(this DependencyObject element, string path) =>
			new PropertyChangedWatcher(element, path);
	}

	public class PropertyChangedWatcher : DependencyObject, INotifyPropertyChanged, IValueConverter
	{
		public static readonly DependencyProperty WatchedPropertyProperty =
			DependencyProperty.RegisterAttached("WatchedProperty", typeof(object),
				typeof(PropertyChangedWatcher), new PropertyMetadata());

		public object GetWatchedProperty() => GetValue(WatchedPropertyProperty);
		public void SetWatchedProperty(object value) => SetValue(WatchedPropertyProperty, value);

		public string PropertyName { get; }

		public event PropertyChangedEventHandler PropertyChanged;

		internal PropertyChangedWatcher(object source, string path = default, BindingMode mode = BindingMode.OneWay)
		{
			PropertyName = path?.Split('.').Last();
			var binding = path.IsNot()
				? new Binding { Source = source, Mode = mode, Converter = this, FallbackValue = Binding.DoNothing }
				: new Binding(path) { Source = source, Mode = mode, Converter = this, FallbackValue = Binding.DoNothing };
			BindingOperations.SetBinding(source as DependencyObject ?? this, WatchedPropertyProperty, binding);
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
	}
}