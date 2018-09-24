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
#if WINDOWS_PHONE
		public static DataContextWatcher GetDataContextWatcher(this FrameworkElement element)
		{
			var expression = element.GetBindingExpression(DataContextWatcher.ContextProperty);
			return expression.IsNot()
				? new DataContextWatcher(element)
				: (DataContextWatcher) expression.ParentBinding.Converter;
		}
#else
		public static FrameworkElement GetDataContextWatcher(this FrameworkElement element) => element;
#endif
		public static PropertyChangedWatcher GetWatcher<T>(this T element, string path)
			where T : DependencyObject, INotifyPropertyChanged => new PropertyChangedWatcher(element, path);
	}

	public sealed class DataContextWatcher : IValueConverter
	{
		internal static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached(
			"Context", typeof(object), typeof(PropertyChangedWatcher), new PropertyMetadata(default(object)));

		public event EventHandler DataContextChanged = (sender, args) => { };

		internal DataContextWatcher(DependencyObject source) =>
			BindingOperations.SetBinding(source, ContextProperty, new Binding {Converter = this});

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			DataContextChanged(this, EventArgs.Empty);
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			throw new NotImplementedException();
	}

	public class PropertyChangedWatcher : DependencyObject, INotifyPropertyChanged
	{
		private const string WatchedPropertyName = "WatchedProperty";

		private static readonly DependencyProperty WatchedPropertyProperty =
			DependencyProperty.Register(WatchedPropertyName, typeof(object),
				typeof(PropertyChangedWatcher), new PropertyMetadata(default(object),
					(sender, args) => InvokePropertyChanged((PropertyChangedWatcher) sender)));

		public object GetWatchedProperty() => GetValue(WatchedPropertyProperty);
		public void SetWatchedProperty(object value) => SetValue(WatchedPropertyProperty, value);

		private static void InvokePropertyChanged(PropertyChangedWatcher watcher) =>
			watcher.PropertyChanged?.Invoke(watcher, new PropertyChangedEventArgs(watcher.PropertyName));

		public new event PropertyChangedEventHandler PropertyChanged;

		public string PropertyName { get; }

		internal PropertyChangedWatcher(object source, string path = null, BindingMode mode = BindingMode.Default)
		{
			PropertyName = path?.Split('.').Last();
			var binding = path.IsNot()
				? new Binding {Source = source, Mode = mode}
				: new Binding(path) {Source = source, Mode = mode};
			BindingOperations.SetBinding(this, WatchedPropertyProperty, binding);
		}
	}
}