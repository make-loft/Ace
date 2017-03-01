using System;
using System.Linq;
using System.Reflection;
//using System.Reflection;
using Windows.UI.Xaml;

namespace Aero.Markup
{
    public static class Attacher
    {
        public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached(
            "Context", typeof (ContextObject), typeof (Attacher), new PropertyMetadata(default(ContextObject)));

        public static void SetContext(DependencyObject element, ContextObject value)
        {
            element.SetValue(ContextProperty, value);
        }

        public static ContextObject GetContext(DependencyObject element)
        {
            return (ContextObject) element.GetValue(ContextProperty);
        }

        public static readonly DependencyProperty ContextTriggersProperty = DependencyProperty.RegisterAttached(
            "ContextTriggers", typeof (Set), typeof (Attacher), new PropertyMetadata(default(Set), (o, args) =>
            {
                var element = o.Of<UIElement>();
                var contextTriggers = args.NewValue.Of<Set>().OfType<ContextTrigger>().ToList();

                foreach (var contextTrigger in contextTriggers)
                {
                    var eventInfo = element.GetType().GetRuntimeEvent(contextTrigger.EventName);
                    var eventHandlerMethodInfo = typeof(ContextTrigger).GetRuntimeMethod("ExecuteCommand", new Type[0]);
                    var eventDelegate = eventHandlerMethodInfo.CreateDelegate(eventInfo.EventHandlerType, contextTrigger);
                    //new EventHandler((o, eventArgs) => eventHandlerMethodInfo.); Delegate.CreateDelegate(eventInfo.EventHandlerType, contextTrigger,
                    //eventHandlerMethodInfo);
                    eventInfo.AddEventHandler(element, eventDelegate);
                    contextTrigger.Element = element;
                }
            }));

        public static void SetContextTriggers(UIElement element, Set value)
        {
            element.SetValue(ContextTriggersProperty, value);
        }

        public static Set GetContextTriggers(DependencyObject element)
        {
            return (Set) element.GetValue(ContextTriggersProperty);
        }
    }
}