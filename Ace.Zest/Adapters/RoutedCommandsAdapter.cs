using System;
using System.Linq;
using System.Windows;
using Ace.Evocators;

namespace Ace.Adapters
{
#if !DESKTOP
    internal static class RoutedCommandsAdapter
    {
        public static object SetCommandBindings(object frameworkElement, object contextObject) => contextObject;
    }
#else
    public static class RoutedCommandsAdapter
    {
        public static object SetCommandBindings(object frameworkElement, object contextObject)
        {
            if (!(frameworkElement is FrameworkElement element) || !(contextObject is ContextObject context)) return contextObject;
            element.CommandBindings.AddRange(context.CommandEvocators.Values.Select(ToRoutedCommandBinding).ToArray());

            void OnEventHandler(object sender, EventArgs args)
            {
                element.DataContext = contextObject;
                element.Initialized -= OnEventHandler;
            }

            element.Initialized += OnEventHandler;
            return contextObject;
        }

        public static System.Windows.Input.CommandBinding ToRoutedCommandBinding(this CommandEvocator evocator)
        {
            var continueExecution = false;
            var routedCommandBinding = new System.Windows.Input.CommandBinding(evocator.Command);
            routedCommandBinding.Executed += (sender, args) =>
            {
                var e = new ExecutedEventArgs(args.Command, args.Parameter);
                evocator.EvokeExecuted(sender, e);
                args.Handled = e.Handled;
            };

            routedCommandBinding.CanExecute += (sender, args) =>
            {
                if (!args.Handled) continueExecution = true;
                var e = new CanExecuteEventArgs(args.Command, args.Parameter);
                evocator.EvokeCanExecute(sender, e);
                args.Handled = e.Handled;
                args.CanExecute = e.CanExecute;
            };

            routedCommandBinding.PreviewExecuted += (sender, args) =>
            {
                var e = new ExecutedEventArgs(args.Command, args.Parameter);
                evocator.EvokePreviewExecuted(sender, e);
                args.Handled = e.Handled;
                if (!continueExecution) return;
                continueExecution = false;
                evocator.EvokeExecuted(sender, new ExecutedEventArgs(e.Command, e.Parameter));
            };

            routedCommandBinding.PreviewCanExecute += (sender, args) =>
            {
                var e = new CanExecuteEventArgs(args.Command, args.Parameter);
                evocator.EvokePreviewCanExecute(sender, e);
                args.CanExecute = e.CanExecute;
                args.Handled = e.Handled;
            };

            return routedCommandBinding;
        }
    }
#endif
}