using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Art.Markup
{
    public static class Attacher
    {
        public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached(
            "Context", typeof (ContextObject), typeof (Attacher), new PropertyMetadata(default(ContextObject),
                (o, args) => RoutedCommandsAdapter.SetCommandBindings(o, args.NewValue)));

        public static void SetContext(DependencyObject element, ContextObject value)
        {
            element.SetValue(ContextProperty, value);
        }

        public static ContextObject GetContext(DependencyObject element)
        {
            return (ContextObject) element.GetValue(ContextProperty);
        }

        // See: http://stackoverflow.com/questions/1448899/attached-property-of-type-list
        public static readonly DependencyProperty ContextTriggersProperty = DependencyProperty.RegisterAttached(
            "ContextTriggersInternal", typeof (object), typeof (Attacher), new PropertyMetadata(null));

        //public static void SetContextTriggers(UIElement element, Set value)
        //{
        //    element.SetValue(ContextTriggersProperty, value);
        //}

        public static Set GetContextTriggers(UIElement element)
        {
            var contextTriggers = (Set) element.GetValue(ContextTriggersProperty);
            if (contextTriggers != null) return contextTriggers;
            contextTriggers = new Set();
            contextTriggers.CollectionChanged += (sender, args) =>
                args.NewItems.Cast<ContextTrigger>().ForEach(contextTrigger =>
                {
                    var eventInfo = element.GetType().GetEvent(contextTrigger.EventName);
                    var eventHandlerMethodInfo = typeof (ContextTrigger).GetMethod("ExecuteCommand",
                        BindingFlags.Instance | BindingFlags.Public);
                    var eventDelegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, contextTrigger,
                        eventHandlerMethodInfo);
                    eventInfo.AddEventHandler(element, eventDelegate);
                    contextTrigger.Element = element;
                });
            element.SetValue(ContextTriggersProperty, contextTriggers);
            return contextTriggers;
        }
    }
}