using System;
using System.Windows.Input;

namespace Ace.Evocators;

public abstract class CommandEventArgs : EventArgs
{
	protected CommandEventArgs(object sender, ICommand command, object parameter, bool handled)
	{
		Sender = sender;
		Command = command;
		Parameter = parameter;
		Handled = handled;
	}

	public object Sender { get; }
	public ICommand Command { get; }
	public object Parameter { get; }
	public bool Handled { get; set; }
}

public class ExecutedEventArgs : CommandEventArgs
{
	public ExecutedEventArgs(object sender, ICommand command, object parameter, bool handled) :
		base(sender, command, parameter, handled)
	{ }
}

public class CanExecuteEventArgs : CommandEventArgs
{
	public CanExecuteEventArgs(object sender, ICommand command, object parameter, bool handled, bool canExecute) :
		base(sender, command, parameter, handled) => CanExecute = canExecute;

	public bool CanExecute { get; set; }
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

public class CommandEvocator : CommandEvocator<ExecutedEventArgs, CanExecuteEventArgs>
{
	public CommandEvocator(ICommand command) => Command = command;
	public ICommand Command { get; }
}