using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
#if XAMARIN
using Xamarin.Forms;
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
#else
using System.Windows;
using System.Windows.Data;
#endif

namespace Ace
{
	internal class PropertyChangedWatcher : DependencyObject, INotifyPropertyChanged, IValueConverter
	{
		public static readonly DependencyProperty TargetProperty =
#if XAMARIN
			DependencyProperty.Create("Context", typeof(object), typeof(PropertyChangedWatcher), default);
#else
			DependencyProperty.Register("Context", typeof(object), typeof(PropertyChangedWatcher), default);
#endif
		public object Context { get; private set; }
		public object Source { get; private set; }

		public string PropertyName => _propertyChangedEventArgs.PropertyName;
		readonly PropertyChangedEventArgs _propertyChangedEventArgs;

		public event PropertyChangedEventHandler PropertyChanged;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Context = value;
			PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;

		internal PropertyChangedWatcher(object source, string path, Action<PropertyChangedWatcher> propertyChanged = default)
		{
			Source = source;
			if (propertyChanged.Is())
				PropertyChanged += (sender, args) => propertyChanged((PropertyChangedWatcher)sender);
			_propertyChangedEventArgs = new(path?.Split('.').Last());

			this.SetBinding(TargetProperty, new Binding(path)
			{
				Converter = this,
				Source = source,
				Mode = BindingMode.OneWay,
				FallbackValue = Binding.DoNothing
			});
		}
	}
}