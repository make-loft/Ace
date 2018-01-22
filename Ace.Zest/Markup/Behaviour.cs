﻿using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Ace.Adapters;
using static System.Windows.DependencyProperty;
#if XAMARIN
using Target = Xamarin.Forms.BindableObject;
using TargetProperty = Xamarin.Forms.BindableProperty;
#else
using Target = System.Windows.DependencyObject;
using TargetProperty = System.Windows.DependencyProperty;
#endif

namespace Ace.Markup
{
	public static class Behaviour
	{
		public static readonly TargetProperty ContextProperty = RegisterAttached(
			"Context", typeof(ContextObject), typeof(Behaviour), new PropertyMetadata(default(ContextObject),
				(o, args) => RoutedCommandsAdapter.SetCommandBindings(o, args.NewValue))).Unbox();

		public static void SetContext(Target element, ContextObject value) => element.SetValue(ContextProperty, value);
		public static ContextObject GetContext(Target element) => (ContextObject) element.GetValue(ContextProperty);

		//  see why 'ContextTriggersInternal': http://stackoverflow.com/questions/1448899/attached-property-of-type-list
		public static readonly TargetProperty ContextTriggersProperty = RegisterAttached(
			"ContextTriggersInternal", typeof(object), typeof(Behaviour), new PropertyMetadata(null, (o, args) =>
				((Set) args.NewValue).Cast<ContextTrigger>().ForEach(t => Subscribe(o, t)))).Unbox();

		//public static void SetContextTriggers(UIElement element, Set value) =>
		//	element.SetValue(ContextTriggersProperty, value);

		public static Set GetContextTriggers(Target element)
		{
			var contextTriggers = (Set) element.GetValue(ContextTriggersProperty);
			if (contextTriggers != null) return contextTriggers;
			contextTriggers = new Set();
			contextTriggers.CollectionChanged += (sender, args) =>
				args.NewItems.Cast<ContextTrigger>().ForEach(t => Subscribe(element, t));
			element.SetValue(ContextTriggersProperty, contextTriggers);
			return contextTriggers;
		}

		private static readonly MethodInfo MethodInfo =
			typeof(ContextTrigger).GetRuntimeMethod("ExecuteCommand", new[] {typeof(object), typeof(EventArgs)});

		public static void Subscribe(object element, ContextTrigger contextTrigger)
		{
			var eventInfo = element.GetType().GetRuntimeEvent(contextTrigger.EventName);
			if (eventInfo == null)
				throw new Exception($"Can not create Context Trigger for '{contextTrigger.EventName}' at '{element}'.");
			var handler = MethodInfo.CreateDelegate(eventInfo.EventHandlerType, contextTrigger);

			eventInfo.AddEventHandler(element, handler);
			contextTrigger.Element = element;
		}

		public static readonly DependencyProperty UpdateHeaderOnLanguageChangeProperty = RegisterAttached(
			"UpdateHeaderOnLanguageChange", typeof(object), typeof(Behaviour),
			new PropertyMetadata(default(object), null, CoerceValueCallback));
		
		private static object CoerceValueCallback(DependencyObject dependencyObject, object baseValue)
		{
			BindingOperations.GetBindingExpression(dependencyObject, HeaderedItemsControl.HeaderProperty)?.UpdateTarget();
			return baseValue;
		}

		public static readonly DependencyProperty DragMoveProperty =
			RegisterAttached("DragMove", typeof(bool), typeof(Behaviour), new PropertyMetadata(default(bool), DragMoveChangedCallback));

		private static void DragMoveChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs args)
		{
			var element = o as FrameworkElement ?? throw new ArgumentException("Expected FrameworkElement");
			var lastPosition = new Point();
			
			if (args.NewValue.Is(true))
				element.MouseMove += OnWindowOnPreviewMouseMove;
			else if (args.NewValue.Is(false))
				element.MouseMove -= OnWindowOnPreviewMouseMove;		
			
			void OnWindowOnPreviewMouseMove(object sender, MouseEventArgs e)
			{
				var window = sender as Window ?? o.EnumerateVisualAncestors().OfType<Window>().FirstOrDefault();
				if (window == null) return;
				var currentPosition = e.GetPosition(element);
				if (e.LeftButton == MouseButtonState.Pressed && lastPosition != currentPosition) window.DragMove();
				lastPosition = currentPosition;
			}
		}

		public static void SetUpdateHeaderOnLanguageChange(DependencyObject element, object value) =>
			element.SetValue(UpdateHeaderOnLanguageChangeProperty, value);

		public static object GetUpdateHeaderOnLanguageChange(DependencyObject element) =>
			element.GetValue(UpdateHeaderOnLanguageChangeProperty);

		public static bool GetDragMove(UIElement element) { return (bool) element.GetValue(DragMoveProperty); }

		public static void SetDragMove(UIElement element, bool value) { element.SetValue(DragMoveProperty, value); }
	}
}