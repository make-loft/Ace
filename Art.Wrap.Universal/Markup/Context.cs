using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Aero.Markup.Patterns;

namespace Aero.Markup
{
    public class Context : ABindingExtension, ICommand
    {
        public Context() : base(new RelativeSource {Mode = RelativeSourceMode.Self})
        {
        }

        public Context(string key) : base(new RelativeSource {Mode = RelativeSourceMode.Self})
        {
            Key = key;
        }

        public string Key { get; set; }

        //[TypeConverter(typeof (XamlTypeConverter))]
        public string StoreKey { get; set; }

        public override object Convert(object value, Type targetType, object parameter, string culture)
        {
            Key = Key ?? parameter as string;
            var frameworkElement = value as FrameworkElement;
            if (frameworkElement != null)
            {
                var mediator = new Input.Mediator();

                RoutedEventHandler initializeMediatorHandler = null;
                initializeMediatorHandler = (sender, args) =>
                {
                    frameworkElement.Loaded -= initializeMediatorHandler;
                    var context = StoreKey == null
                        ? frameworkElement.GetDataContext<ContextObject>()
                        : new Store {Key = StoreKey}.Convert(null, null, null, culture) as ContextObject;

                    if (context == null) return;
                    var command = Aero.Context.Get(Key);
                    if (command == null) return;
                    var commandBinding = context[command];
                    mediator.Initialize(frameworkElement, commandBinding);
                };

                frameworkElement.Loaded += initializeMediatorHandler;

                return mediator;
            }
            {
                var context = StoreKey == null
                    ? value as ContextObject
                    : new Store {Key = StoreKey}.Convert(null, null, null, culture) as ContextObject;
                if (context == null) return null;
                var command = Aero.Context.Get(Key);
                if (command == null) return null;
                var commandBinding = context[command];
                var mediator = new Input.Mediator(value, commandBinding);
                return mediator;
            }
        }

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public event EventHandler CanExecuteChanged;
    }
}