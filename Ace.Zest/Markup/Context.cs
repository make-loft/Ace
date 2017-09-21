// ReSharper disable RedundantUsingDirective
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Ace.Input;
using TypeConverter = Xamarin.Forms.TypeConverterAttribute;
using TypeTypeConverter = Xamarin.Forms.TypeTypeConverter;

namespace Ace.Markup
{
    public class Context : Patterns.AMarkupExtension
    {
        private readonly Mediator _mediator = new Mediator();

        public Context() => Key = null;

        public Context(string key) => Key = key;

        [TypeConverter(typeof(TypeTypeConverter))]
        public Type StoreKey { get; set; }

        public string Key { get; set; }

        public override object Provide(object targetObject, object targetProperty = null)
        {
#if XAMARIN
            if (targetObject is Xamarin.Forms.BindableObject bindableObject && StoreKey == null)
            {
                bindableObject.BindingContextChanged += (sender, args) =>
                    RefreshMediator(targetObject, bindableObject.BindingContext);
            }
#else
            if (targetObject is FrameworkElement frameworkElement && StoreKey == null)
            {
                void OnLoadedRefreshMediatorHandler(object sender, RoutedEventArgs args)
                {
                    frameworkElement.Loaded -= OnLoadedRefreshMediatorHandler;
                    RefreshMediator(frameworkElement, frameworkElement.DataContext);
                }

                frameworkElement.Loaded += OnLoadedRefreshMediatorHandler;
                if (Mode != BindingMode.OneTime)
                {
                    frameworkElement.GetDataContextWatcher().DataContextChanged += (sender, args) =>
                        RefreshMediator(frameworkElement, frameworkElement.DataContext);
                }
            }
#endif
            else if (StoreKey != null) RefreshMediator(targetObject, Ace.Store.Get(StoreKey));

            return _mediator;
        }

        private void RefreshMediator(object targetObject, object contex)
        {
            var command = Ace.Context.Get(Key);
            var evocator = (contex as ContextObject)?[command];
            _mediator.Initialize(targetObject, evocator);
        }
    }
}