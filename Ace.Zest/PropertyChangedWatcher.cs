using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Ace
{
    public static class WatcherSugar
    {
#if WINDOWS_PHONE
        public static DataContextWatcher GetDataContextWatcher(this FrameworkElement element)
        {
            var expression = element.GetBindingExpression(DataContextWatcher.ContextProperty);
            return expression == null
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
                typeof(PropertyChangedWatcher), new PropertyMetadata(default(object), (o, args) =>
                {
                    var watcher = (PropertyChangedWatcher) o;
                    watcher.PropertyChanged(watcher, new PropertyChangedEventArgs(watcher.PropertyName));
                }));

        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        public string PropertyName { get; }

        internal PropertyChangedWatcher(object source, string path)
        {
            PropertyName = path?.Split('.').Last();
            var binding = path == null ? new Binding {Source = source} : new Binding(path) {Source = source};
            BindingOperations.SetBinding(this, WatchedPropertyProperty, binding);
        }
    }
}