using System;
using System.Collections.Generic;
using System.Linq;
using Ace.Input;
using System.Windows;
using Ace.Evocators;
#if XAMARIN
using Xamarin.Forms;
using System.Windows.Data;
using ContextElement = Xamarin.Forms.Element;
#else
using System.ComponentModel;
using ContextElement = System.Windows.FrameworkElement;
#endif

namespace Ace.Markup
{
	public class Context : Patterns.AMarkupExtension
	{
		public Context() => Key = default;
		public Context(string key) => Key = key;

		public string Key { get; set; }
#if XAMARIN
		public BindingMode Mode { get; set; }
		[TypeConverter(typeof(PathConverter))] public PropertyPath SourcePath { get; set; }
		[TypeConverter(typeof(PathConverter))] public PropertyPath TrackedPath { get; set; }
#else
		public PropertyPath SourcePath { get; set; }
		public PropertyPath TrackedPath { get; set; }
#endif
		[TypeConverter(typeof(TypeTypeConverter))] public Type StoreKey { get; set; }
		[TypeConverter(typeof(TypeTypeConverter))] public Type RelativeContextType { get; set; }

		public override object Provide(object targetObject, object targetProperty = null)
		{
			var mediator = new Mediator();
			var source = StoreKey.Is() ? Ace.Store.Get(StoreKey) : null;
			if (source.Is() && SourcePath.IsNot())
			{
				mediator.Set(targetObject, GetCommandEvocator(source));
			}
			else if (SourcePath.Is())
			{
				var watcher = new PropertyChangedWatcher(source, SourcePath?.Path);
				watcher.PropertyChanged += (sender, args) =>
					mediator.Set(targetObject, GetCommandEvocator(watcher.GetTarget()));
				mediator.Set(targetObject, GetCommandEvocator(watcher.GetTarget()));
			}
			else if (targetObject.Is(out ContextElement element))
			{
#if XAMARIN
				element.BindingContextChanged += Set;

				void Set(object sender, EventArgs args)
				{
					element.BindingContextChanged -= Set;
					mediator.Set(element, GetCommandEvocator(FindContextObject(element, RelativeContextType)));
				}
#else
				element.Loaded += Set;

				void Set(object sender, EventArgs args)
				{
					element.Loaded -= Set;
					mediator.Set(element, GetCommandEvocator(FindContextObject(element, RelativeContextType)));
				}
#endif
				if (TrackedPath.Is())
				{
					var watcher = new PropertyChangedWatcher(element, TrackedPath?.Path);
					watcher.PropertyChanged += (sender, args) =>
						mediator.Set(element, GetCommandEvocator(FindContextObject(element, RelativeContextType)));
				}
			}

			return mediator;
		}

#if XAMARIN
		public static object GetContext(ContextElement element) => element.BindingContext;
#else
		public static object GetContext(ContextElement element) => element.DataContext;
#endif

		private static ContextObject FindContextObject(ContextElement element, Type type) =>
			type.Is() ? FindRelativeContextObject(element, type) : FindNearestContextObject(element);

		private static ContextObject FindNearestContextObject(ContextElement element) =>
			EnumerateContextObjects(element).FirstOrDefault();

		private static ContextObject FindRelativeContextObject(ContextElement element, Type type) =>
			EnumerateContextObjects(element).FirstOrDefault(c => c.GetType() == type);

		private static IEnumerable<ContextObject> EnumerateContextObjects(ContextElement target) =>
			target.EnumerateSelfAndVisualAncestors().OfType<ContextElement>().Select(GetContext).OfType<ContextObject>();

		private CommandEvocator GetCommandEvocator(object target) => 
			target.Is(out ContextObject contextObject) ? contextObject[Ace.Context.Get(Key)] : null;
	}
}