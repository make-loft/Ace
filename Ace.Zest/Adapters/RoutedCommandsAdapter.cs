namespace Ace.Adapters;

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
		if (!frameworkElement.Is(out FrameworkElement element) || !contextObject.Is(out ContextObject context))
			return contextObject;

		void OnEventHandler(object sender, EventArgs args)
		{
			element.DataContext = contextObject;
			element.Initialized -= OnEventHandler;
		}

		context.CommandEvocators.Values.Select(ToRoutedCommandBinding).ForEach(element.CommandBindings.Add);
		element.Initialized += OnEventHandler;
		return contextObject;
	}

	public static CommandBinding ToRoutedCommandBinding(this CommandEvocator evocator)
	{
		void OnExecuted(object sender, ExecutedRoutedEventArgs args)
		{
			var e = new ExecutedEventArgs(sender, args.Command, args.Parameter, args.Handled);
			evocator.EvokeExecuted(e);
			args.Handled = args.Handled;
		}

		void OnCanExecute(object sender, CanExecuteRoutedEventArgs args)
		{
			var e = new CanExecuteEventArgs(sender, args.Command, args.Parameter, args.Handled, evocator.HasExecuted());
			evocator.EvokeCanExecute(e);
			args.CanExecute = args.CanExecute;
			args.Handled = args.Handled;
		}

		var commandBinding = new CommandBinding(evocator.Command);
		commandBinding.Executed += OnExecuted;
		commandBinding.CanExecute += OnCanExecute;
		commandBinding.PreviewExecuted += OnExecuted;
		commandBinding.PreviewCanExecute += OnCanExecute;

		return commandBinding;
	}
}
#endif
