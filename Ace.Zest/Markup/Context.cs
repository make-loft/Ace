using System;
using System.Collections.Generic;
using System.Linq;
using Ace.Input;
using System.Windows;
#if XAMARIN
using Xamarin.Forms;
using ContextElement = Xamarin.Forms.Element;
using TypeTypeConverter = Xamarin.Forms.TypeTypeConverter;
#else
using System.ComponentModel;
using System.Windows.Data;
using ContextElement = System.Windows.FrameworkElement;
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

		[TypeConverter(typeof(TypeTypeConverter))]
		public Type RelativeContextType { get; set; }

		public string Key { get; set; }

		public PropertyPath SourcePath { get; set; }
#if XAMARIN
		public BindingMode Mode { get; set; }
#endif

		public override object Provide(object targetObject, object targetProperty = null)
		{
			var source = StoreKey == null ? null : Ace.Store.Get(StoreKey);
			if (source != null && SourcePath == null)
			{
				RefreshMediator(targetObject, source as ContextObject);
			}
			else if (SourcePath != null)
			{
				var watcher = new PropertyChangedWatcher(source, SourcePath.ToString(), Mode);
				watcher.PropertyChanged += (sender, args) =>
				   RefreshMediator(targetObject, watcher.GetWatchedProperty() as ContextObject);
			}
#if XAMARIN
			if (targetObject is ContextElement element)
			{
				element.BindingContextChanged += (sender, args) =>
					RefreshMediator(element, FindContextObject(element, RelativeContextType));
			}
#else
			if (targetObject is FrameworkElement element)
			{
				element.Loaded += OnLoadedRefreshMediatorHandler;
				void OnLoadedRefreshMediatorHandler(object sender, RoutedEventArgs args)
				{
					element.Loaded -= OnLoadedRefreshMediatorHandler;
					RefreshMediator(element, FindContextObject(element, RelativeContextType));
				}

				if (Mode != BindingMode.OneTime)
				{
					element.GetDataContextWatcher().DataContextChanged += (sender, args) =>
						RefreshMediator(element, FindContextObject(element, RelativeContextType));
				}
			}
#endif

			return _mediator;
		}

#if XAMARIN
		public static object GetContext(ContextElement element) => element.BindingContext;
#else
		public static object GetContext(FrameworkElement element) => element.DataContext;
#endif

		private static ContextObject FindContextObject(ContextElement element, Type type) =>
			type == null ? FindNearestContextObject(element) : FindRelativeContextObject(element, type);

		private static ContextObject FindNearestContextObject(ContextElement element) =>
			EnumerateContextObjects(element).FirstOrDefault();

		private static ContextObject FindRelativeContextObject(ContextElement element, Type type) =>
			EnumerateContextObjects(element).FirstOrDefault(c => c.GetType() == type);

		// ReSharper disable once RedundantEnumerableCastCall
		private static IEnumerable<ContextObject> EnumerateContextObjects(ContextElement target) =>
			target.EnumerateVisualAncestors().OfType<ContextElement>().Select(GetContext).OfType<ContextObject>();

		private void RefreshMediator(object targetObject, ContextObject targetContext) =>
			_mediator.Initialize(targetObject, targetContext?[Ace.Context.Get(Key)]);
	}
}