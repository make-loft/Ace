// ReSharper disable RedundantUsingDirective
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Ace.Input;
using TypeConverter = Xamarin.Forms.TypeConverterAttribute;
using TypeTypeConverter = Xamarin.Forms.TypeTypeConverter;
using System.Reflection;

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
                        RefreshMediator(targetObject);
            }
#else
            if (targetObject is FrameworkElement frameworkElement && StoreKey == null)
            {
                void OnLoadedRefreshMediatorHandler(object sender, RoutedEventArgs args)
                {
                    frameworkElement.Loaded -= OnLoadedRefreshMediatorHandler;
                    RefreshMediator(frameworkElement);
                }

                frameworkElement.Loaded += OnLoadedRefreshMediatorHandler;
                if (Mode != BindingMode.OneTime)
                {
                    frameworkElement.GetDataContextWatcher().DataContextChanged += (sender, args) =>
                        RefreshMediator(frameworkElement);
                }
            }
#endif
            else if (StoreKey != null) RefreshMediator(targetObject, Ace.Store.Get(StoreKey));

            return _mediator;
        }

        private void RefreshMediator(object targetObject, object context = null)
        {
            var command = Ace.Context.Get(Key);
            var targetElement = targetObject as Xamarin.Forms.BindableObject;
            while (targetElement != null)
            {
                if ((context != null && ((ContextObject)context).CommandEvocators.TryGetValue(command, out var evocator)) ||
                    ((targetElement.BindingContext is ContextObject co) && co.CommandEvocators.TryGetValue(command, out evocator)))
                {
                    _mediator.Initialize(targetObject, evocator);
                    return;
                }
                else targetElement = targetElement.GetType().
                        GetRuntimeProperty("Parent")?.GetValue(targetElement) as Xamarin.Forms.BindableObject;
            }
        }
    }
}