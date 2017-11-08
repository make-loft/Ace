using System;
using System.Linq;
using System.Reflection;
using System.Windows;
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
    public static class Attacher
    {
        public static readonly TargetProperty ContextProperty = RegisterAttached(
            "Context", typeof(ContextObject), typeof(Attacher), new PropertyMetadata(default(ContextObject),
                (o, args) => RoutedCommandsAdapter.SetCommandBindings(o, args.NewValue))).Unbox();

        public static void SetContext(Target element, ContextObject value) => element.SetValue(ContextProperty, value);
        public static ContextObject GetContext(Target element) => (ContextObject)element.GetValue(ContextProperty);

        //  see why 'ContextTriggersInternal': http://stackoverflow.com/questions/1448899/attached-property-of-type-list
        public static readonly TargetProperty ContextTriggersProperty = RegisterAttached(
            "ContextTriggersInternal", typeof(object), typeof(Attacher), new PropertyMetadata(null, (o, args) =>
                ((Set)args.NewValue).Cast<ContextTrigger>().ForEach(t => Subscribe(o, t)))).Unbox();

        //public static void SetContextTriggers(UIElement element, Set value) =>
        //    element.SetValue(ContextTriggersProperty, value);

        public static Set GetContextTriggers(Target element)
        {
            var contextTriggers = (Set)element.GetValue(ContextTriggersProperty);
            if (contextTriggers != null) return contextTriggers;
            contextTriggers = new Set();
            contextTriggers.CollectionChanged += (sender, args) =>
                args.NewItems.Cast<ContextTrigger>().ForEach(t => Subscribe(element, t));
            element.SetValue(ContextTriggersProperty, contextTriggers);
            return contextTriggers;
        }

        private static readonly MethodInfo MethodInfo =
            typeof(ContextTrigger).GetRuntimeMethod("ExecuteCommand", new[] { typeof(object), typeof(EventArgs) });

        public static void Subscribe(object element, ContextTrigger contextTrigger)
        {
            var eventInfo = element.GetType().GetRuntimeEvent(contextTrigger.EventName);
            if (eventInfo == null)
                throw new Exception($"Can not create Context Trigger for '{contextTrigger.EventName}' at '{element}'.");
            var handler = MethodInfo.CreateDelegate(eventInfo.EventHandlerType, contextTrigger);

            eventInfo.AddEventHandler(element, handler);
            contextTrigger.Element = element;
        }
    }
}