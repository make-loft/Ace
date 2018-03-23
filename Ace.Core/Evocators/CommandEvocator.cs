using System;
using System.Windows.Input;

namespace Ace.Evocators
{
	public abstract class CommandEventArgs : EventArgs
	{
		protected CommandEventArgs(ICommand command, object parameter, bool handled)
		{
			Command = command;
			Parameter = parameter;
			Handled = handled;
		}

		public ICommand Command { get; }
		public object Parameter { get; }
		public bool Handled { get; set; }
	}

	public class ExecutedEventArgs : CommandEventArgs
	{
		public ExecutedEventArgs(ICommand command, object parameter, bool handled) :
			base(command, parameter, handled)
		{
		}
	}

	public class CanExecuteEventArgs : CommandEventArgs
	{
		public CanExecuteEventArgs(ICommand command, object parameter, bool handled, bool canExecute = true) :
			base(command, parameter, handled) => CanExecute = canExecute;

		public bool CanExecute { get; set; }
	}

	public class CommandEvocator<TE, TC> where TE : EventArgs where TC : EventArgs
	{
		public event EventHandler<TE> Executed;
		public event EventHandler<TC> CanExecute;
		public event EventHandler<TE> PreviewExecuted;
		public event EventHandler<TC> PreviewCanExecute;

		public void EvokeExecuted(object sender, TE args) => Executed?.Invoke(sender, args);
		public void EvokeCanExecute(object sender, TC args) => CanExecute?.Invoke(sender, args);
		public void EvokePreviewExecuted(object sender, TE args) => PreviewExecuted?.Invoke(sender, args);
		public void EvokePreviewCanExecute(object sender, TC args) => PreviewCanExecute?.Invoke(sender, args);

		public bool HasExecuted() => Executed != null;
		public bool HasCanExecute() => CanExecute != null;
		public bool HasPreviewExecuted() => PreviewExecuted != null;
		public bool HasPreviewCanExecute() => PreviewCanExecute != null;
	}

	public class CommandEvocator : CommandEvocator<ExecutedEventArgs, CanExecuteEventArgs>
	{
		public CommandEvocator(ICommand command) => Command = command;
		public ICommand Command { get; }
	}
}