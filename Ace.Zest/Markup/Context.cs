using System;
using Ace.Input;
using TypeConverter = Xamarin.Forms.TypeConverterAttribute;
using System.Reflection;
#if XAMARIN
using DependencyObject = Xamarin.Forms.Element;
using TypeTypeConverter = Xamarin.Forms.TypeTypeConverter;
#else
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
#endif

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

#if XAMARIN
		private static object GetDataContext(Xamarin.Forms.BindableObject dependencyObject) =>
			dependencyObject?.BindingContext;
#else
		private static object GetDataContext(DependencyObject dependencyObject) =>
			(dependencyObject as FrameworkElement)?.DataContext;
#endif
		private void RefreshMediator(object targetObject, object context = null)
		{
			var command = Ace.Context.Get(Key);

			for (var visual = targetObject as DependencyObject; visual != null; visual = visual.GetVisualParent())
			{
				if (context != null && ((ContextObject)context).CommandEvocators.TryGetValue(command, out var evocator) ||
					GetDataContext(visual) is ContextObject co && co.CommandEvocators.TryGetValue(command, out evocator))
				{
					_mediator.Initialize(targetObject, evocator);
					return;
				}
			}
		}
	}
}