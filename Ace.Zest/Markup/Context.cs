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
		private PropertyChangedWatcher _keyPathWatcher, _sourcePathWatcher, _trackedPathWatcher;

		public Context() => Key = default;
		public Context(string key) => Key = key;

		public string Key { get; set; }
#if XAMARIN
		public BindingMode Mode { get; set; }
		[TypeConverter(typeof(PathConverter))] public PropertyPath KeyPath { get; set; }
		[TypeConverter(typeof(PathConverter))] public PropertyPath SourcePath { get; set; }
		[TypeConverter(typeof(PathConverter))] public PropertyPath TrackedPath { get; set; }
#else
		public PropertyPath KeyPath { get; set; }
		public PropertyPath SourcePath { get; set; }
		public PropertyPath TrackedPath { get; set; }
#endif
		public new object Source { get; set; }
		[TypeConverter(typeof(TypeTypeConverter))] public Type StoreKey { get; set; }

		public override object Provide(object targetObject, object targetProperty = null)
		{
			var mediator = new Mediator();

			if (targetObject.Is(out ContextElement element))
			{
#if XAMARIN
				element.BindingContextChanged += Set;

				void Set(object sender, EventArgs args)
				{
					element.BindingContextChanged -= Set;
					mediator.EvokeCanExecuteChanged(element, EventArgs.Empty);
				}
#else
				element.Loaded += Set;

				void Set(object sender, EventArgs args)
				{
					element.Loaded -= Set;
					mediator.EvokeCanExecuteChanged(element, EventArgs.Empty);
				}
#endif
			}

			var source = StoreKey.Is() ? Ace.Store.Get(StoreKey) : Source;

			if (source.IsNot() && element.Is())
			{
				mediator.Set(targetObject, GetCommandEvocator(FindNearestContextObject(element, Key)));
			}

			if (TrackedPath.Is())
			{
				_trackedPathWatcher = new(element, TrackedPath?.Path, w =>
					mediator.Set(element, GetCommandEvocator(source ?? FindNearestContextObject(element, Key))));
			}
			else if (source.Is())
			{
				if (SourcePath.Is())
				{
					_sourcePathWatcher = new(source, SourcePath?.Path, w =>
						mediator.Set(targetObject, GetCommandEvocator(w.Source)));
				}
				else mediator.Set(targetObject, GetCommandEvocator(source));
			}

			if (KeyPath.Is())
			{
				_keyPathWatcher = new(source ?? targetObject, KeyPath?.Path, w =>
					mediator.Set(targetObject, GetCommandEvocator(w.Context)));
			}

			return mediator;
		}

#if XAMARIN
		public static object GetContext(ContextElement element) => element.BindingContext;
#else
		public static object GetContext(ContextElement element) => element.DataContext;
#endif
		private static ContextObject FindNearestContextObject(ContextElement element, string key) =>
			EnumerateContextObjects(element)
			.Where(c => c.CommandEvocators.Any(p => p.Key.Is(out Input.Command c) && c.Name.Is(key))).FirstOrDefault();

		private static ContextObject FindRelativeContextObject(ContextElement element, Type type) =>
			EnumerateContextObjects(element).FirstOrDefault(c => c.GetType() == type);

		private static IEnumerable<ContextObject> EnumerateContextObjects(ContextElement target) =>
			target.EnumerateSelfAndVisualAncestors().OfType<ContextElement>().Select(GetContext).OfType<ContextObject>();

		private CommandEvocator GetCommandEvocator(object target) => 
			target.Is(out ContextObject contextObject) ? contextObject[Ace.Context.Get(Key)] : null;
	}
}