using System;
using System.Windows.Input;

namespace Ace.Evocators
{
	public class ExecutedEventArgs : EventArgs
	{
		public ExecutedEventArgs(ICommand command, object parameter)
		{
			Command = command;
			Parameter = parameter;
		}

		public ICommand Command { get; }
		public object Parameter { get; }
		public bool Handled { get; set; }
	}

	public class CanExecuteEventArgs : EventArgs
	{
		public CanExecuteEventArgs(ICommand command, object parameter)
		{
			Command = command;
			Parameter = parameter;
			CanExecute = true;
		}

		public ICommand Command { get; }
		public object Parameter { get; }
		public bool CanExecute { get; set; }
		public bool Handled { get; set; }
	}

	public class CommandEvocator
	{
		public CommandEvocator(ICommand command) => Command = command;
		public ICommand Command { get; }
		public event EventHandler<ExecutedEventArgs> Executed;
		public event EventHandler<CanExecuteEventArgs> CanExecute;
		public event EventHandler<ExecutedEventArgs> PreviewExecuted;
		public event EventHandler<CanExecuteEventArgs> PreviewCanExecute;

		public void EvokeExecuted(object sender, ExecutedEventArgs args) =>
			Executed?.Invoke(sender, args);

		public void EvokeCanExecute(object sender, CanExecuteEventArgs args)
		{
			args.CanExecute = Executed != null;
			CanExecute?.Invoke(sender, args);
		}

		public void EvokePreviewExecuted(object sender, ExecutedEventArgs args) =>
			PreviewExecuted?.Invoke(sender, args);

		public void EvokePreviewCanExecute(object sender, CanExecuteEventArgs args)
		{
			args.CanExecute = PreviewExecuted != null;
			PreviewCanExecute?.Invoke(sender, args);
		}
	}
}