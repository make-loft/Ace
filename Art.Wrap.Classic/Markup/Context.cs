using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Aero.Evocators;
using Aero.Input;

namespace Aero.Markup
{
    public class Context : Patterns.ABindingExtension
    {
        public Context() : base(new RelativeSource {Mode = RelativeSourceMode.Self})
        {
        }

        public Context(string key) : base(new RelativeSource {Mode = RelativeSourceMode.Self})
        {
            Key = key;
        }

        [TypeConverter(typeof (XamlTypeConverter))]
        public Type StoreKey { get; set; }

        public string Key { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var frameworkElement = value as FrameworkElement;
            if (frameworkElement != null)
            {
                var mediator = new Mediator();

                RoutedEventHandler onLoadedRefreshMediatorHandler = null;
                onLoadedRefreshMediatorHandler = (sender, args) =>
                {
                    frameworkElement.Loaded -= onLoadedRefreshMediatorHandler;
                    RefreshMediator(mediator, frameworkElement, culture);
                };

                frameworkElement.Loaded += onLoadedRefreshMediatorHandler;
                if (Mode != BindingMode.OneTime)
                {
#if WINDOWS_PHONE
                    frameworkElement.GetWatcher().DataContextChanged += (sender, args) =>
                        RefreshMediator(mediator, frameworkElement, culture);
#else
                    frameworkElement.DataContextChanged += (sender, args) => // only for 4.5 framework
                        RefreshMediator(mediator, frameworkElement, culture);
#endif
                }

                return mediator;
            }

            var evocator = GetCommandEvocator(value, culture);
            return new Mediator(value, evocator);
        }

        private void RefreshMediator(Mediator mediator, DependencyObject element, CultureInfo culture)
        {
            var context = element.GetDataContext<ContextObject>();
            var commandBinding = GetCommandEvocator(context, culture);
            mediator.Initialize(element, commandBinding);
        }

        private CommandEvocator GetCommandEvocator(object dataContext, CultureInfo culture)
        {
            var context = StoreKey == null
                ? dataContext as ContextObject
                : new Store {Key = StoreKey}.Convert(null, null, null, culture) as ContextObject;
            if (context == null) return null;
            var command = Aero.Context.Get(Key);
            return command == null ? null : context[command];
        }
    }
}