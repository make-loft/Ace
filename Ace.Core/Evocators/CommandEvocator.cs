using System.Windows.Input;

namespace Ace.Evocators;

public abstract class CommandEventArgs(object sender, ICommand command, object parameter, bool handled)
	: EventArgs
{
	public object Sender { get; } = sender;
	public ICommand Command { get; } = command;
	public object Parameter { get; } = parameter;
	public bool Handled { get; set; } = handled;
}

public class ExecutedEventArgs(object sender, ICommand command, object parameter, bool handled)
	: CommandEventArgs(sender, command, parameter, handled)
{
}

public class CanExecuteEventArgs(object sender, ICommand command, object parameter, bool handled, bool canExecute)
	: CommandEventArgs(sender, command, parameter, handled)
{
	public bool CanExecute { get; set; } = canExecute;
}

public class CommandEvocator<TExecutedArgs, TCanExecuteArgs>
	where TExecutedArgs : EventArgs
	where TCanExecuteArgs : EventArgs
{
	public event Action<TExecutedArgs> Executed;
	public event Action<TCanExecuteArgs> CanExecute;
	public event Action<TExecutedArgs> PreviewExecuted;
	public event Action<TCanExecuteArgs> PreviewCanExecute;

	public void EvokeExecuted(TExecutedArgs args) => Executed?.Invoke(args);
	public void EvokeCanExecute(TCanExecuteArgs args) => CanExecute?.Invoke(args);
	public void EvokePreviewExecuted(TExecutedArgs args) => PreviewExecuted?.Invoke(args);
	public void EvokePreviewCanExecute(TCanExecuteArgs args) => PreviewCanExecute?.Invoke(args);

	public bool HasExecuted() => Executed.Is();
	public bool HasCanExecute() => CanExecute.Is();
	public bool HasPreviewExecuted() => PreviewExecuted.Is();
	public bool HasPreviewCanExecute() => PreviewCanExecute.Is();
}

public class CommandEvocator(ICommand command)
	: CommandEvocator<ExecutedEventArgs, CanExecuteEventArgs>
{
	public ICommand Command { get; } = command;
}