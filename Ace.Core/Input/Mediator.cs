using System;
using System.Diagnostics;
using System.Windows.Input;
using Ace.Evocators;

namespace Ace.Input
{
	public class Mediator : ICommand
	{
		private class WeakListener : IDisposable
		{
			private readonly ICommand _command;
			private readonly WeakReference _weakMediator;

			public WeakListener(ICommand command, Mediator mediator)
			{
				_command = command;
				_weakMediator = new WeakReference(mediator);
				_command.CanExecuteChanged += CommandOnCanExecuteChanged;
			}

			private void CommandOnCanExecuteChanged(object sender, EventArgs eventArgs)
			{
				if (_weakMediator.Target is Mediator mediator)
					mediator.EvokeCanExecuteChanged(sender, eventArgs);
				else _command.CanExecuteChanged -= CommandOnCanExecuteChanged;
			}

			public void Dispose() => _command.CanExecuteChanged -= CommandOnCanExecuteChanged;
		}

		private ICommand _command;
		private WeakReference _weakSender;
		private WeakReference _weakEvocator;
		private WeakListener _weakListener;
		private bool _canExecuteState = true;

		~Mediator() => _weakListener?.Dispose();

		public Mediator() { }

		public Mediator(object sender, CommandEvocator evocator) => Set(sender, evocator);

		public void Set(object sender, CommandEvocator evocator)
		{
			if (evocator.IsNot()) Debug.WriteLine($"Can not get command evocator for {sender}");
			_weakListener?.Dispose();
			_weakSender = sender.Is() ? new(sender) : null;
			_weakEvocator = evocator.Is() ? new(evocator) : null;
			_command = evocator?.Command;
			_weakListener = _command.Is() ? new(_command, this) : null;

			EvokeCanExecuteChanged(sender, EventArgs.Empty);
		}

		public void SetSender(object sender) => _weakSender = new(sender);

		public bool CanExecute(object parameter)
		{
			if (_weakEvocator.IsNot()) return true;
			if (_weakEvocator.Target.To(out CommandEvocator evocator).IsNot() || _command.IsNot())
				return _canExecuteState = false;

			var sender = _weakSender.Target;
			var args = new CanExecuteEventArgs(evocator.Command, parameter, false, evocator.HasExecuted());
			if (evocator.HasPreviewCanExecute())
			{
				evocator.EvokePreviewCanExecute(sender, args);
				args.Handled = args.Handled || args.CanExecute;
			}

			if (evocator.HasCanExecute() && !args.Handled) evocator.EvokeCanExecute(sender, args);
			return _canExecuteState = args.CanExecute;
		}

		public void Execute(object parameter)
		{
			if (_weakEvocator.IsNot()) return;
			if (_weakEvocator.Target.To(out CommandEvocator evocator).IsNot() || _command.IsNot())
				return;

			var sender = _weakSender.Target;
			var args = new ExecutedEventArgs(_command, parameter, false);
			if (evocator.HasPreviewExecuted())
			{
				evocator.EvokePreviewExecuted(sender, args);
				args.Handled = true;
			}
			else if (evocator.HasExecuted() && _canExecuteState) evocator.EvokeExecuted(sender, args);

			EvokeCanExecuteChanged(sender, EventArgs.Empty);
		}

		public void EvokeCanExecuteChanged(object o, EventArgs args) => CanExecuteChanged(o, args);

		public event EventHandler CanExecuteChanged = (sender, args) => { };
	}
}