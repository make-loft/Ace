using System;
using System.Collections.Generic;
using System.Linq;
using Ace.Input;
using System.Windows;
using Ace.Evocators;
#if XAMARIN
using Xamarin.Forms;
using ContextElement = Xamarin.Forms.Element;
using TypeTypeConverter = Xamarin.Forms.TypeTypeConverter;
#else
using System.ComponentModel;
using ContextElement = System.Windows.FrameworkElement;
#endif

namespace Ace.Markup
{
	public class Context : Patterns.AMarkupExtension
	{
		public Context() => Key = null;
		public Context(string key) => Key = key;

		public string Key { get; set; }
		public PropertyPath SourcePath { get; set; }
		public bool TrackContextChanges { get; set; } = true;
		[TypeConverter(typeof(TypeTypeConverter))] public Type StoreKey { get; set; }
		[TypeConverter(typeof(TypeTypeConverter))] public Type RelativeContextType { get; set; }
#if XAMARIN
		public BindingMode Mode { get; set; }
#endif

		public override object Provide(object targetObject, object targetProperty = null)
		{
			var mediator = new Mediator();
			var source = StoreKey == null ? null : Ace.Store.Get(StoreKey);
			if (source != null && SourcePath == null)
			{
				mediator.Set(targetObject, GetCommandEvocator(source));
			}
			else if (SourcePath != null)
			{
				var watcher = new PropertyChangedWatcher(source, SourcePath.ToString(), Mode);
				watcher.PropertyChanged += (sender, args) =>
					mediator.Set(targetObject, GetCommandEvocator(watcher.GetWatchedProperty()));
			}
			else if (targetObject is ContextElement element)
			{
#if XAMARIN
				if (TrackContextChanges)
				{
					element.BindingContextChanged += (sender, args) =>
						RefreshMediator(element, FindContextObject(element, RelativeContextType));
				}
				else RefreshMediator(element, FindContextObject(element, RelativeContextType));
#else
				element.Loaded += OnLoadedRefreshMediatorHandler;

				void OnLoadedRefreshMediatorHandler(object sender, RoutedEventArgs args)
				{
					element.Loaded -= OnLoadedRefreshMediatorHandler;
					mediator.Set(element, GetCommandEvocator(FindContextObject(element, RelativeContextType)));
				}

				if (TrackContextChanges)
				{
					element.GetDataContextWatcher().DataContextChanged += (sender, args) =>
						mediator.Set(element, GetCommandEvocator(FindContextObject(element, RelativeContextType)));
				}
#endif
			}

			return mediator;
		}

#if XAMARIN
		public static object GetContext(ContextElement element) => element.BindingContext;
#else
		public static object GetContext(ContextElement element) => element.DataContext;
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

		private CommandEvocator GetCommandEvocator(object target) => 
			target is ContextObject contextObject ? contextObject[Ace.Context.Get(Key)] : null;
	}
}