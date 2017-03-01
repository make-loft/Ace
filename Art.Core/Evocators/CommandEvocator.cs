using System;
using System.Windows.Input;

namespace Aero.Evocators
{
    public class ExecutedEventArgs : EventArgs
    {
        public ExecutedEventArgs(ICommand command, object parameter)
        {
            Command = command;
            Parameter = parameter;
        }

        public ICommand Command { get; private set; }
        public object Parameter { get; private set; }
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

        public ICommand Command { get; private set; }
        public object Parameter { get; private set; }
        public bool CanExecute { get; set; }
        public bool Handled { get; set; }
    }

    public class CommandEvocator
    {
        public CommandEvocator(ICommand command)
        {
            Command = command;
        }

        public ICommand Command { get; private set; }
        public event EventHandler<ExecutedEventArgs> Executed;
        public event EventHandler<CanExecuteEventArgs> CanExecute;
        public event EventHandler<ExecutedEventArgs> PreviewExecuted;
        public event EventHandler<CanExecuteEventArgs> PreviewCanExecute;

        public void EvokeExecuted(object sender, ExecutedEventArgs args)
        {
            var handler = Executed;
            if (handler != null) handler(sender, args);
        }

        public void EvokeCanExecute(object sender, CanExecuteEventArgs args)
        {
            args.CanExecute = Executed != null;
            var handler = CanExecute;
            if (handler != null) handler(sender, args);
        }

        public void EvokePreviewExecuted(object sender, ExecutedEventArgs args)
        {
            var handler = PreviewExecuted;
            if (handler != null) handler(sender, args);
        }

        public void EvokePreviewCanExecute(object sender, CanExecuteEventArgs args)
        {
            args.CanExecute = PreviewExecuted != null;
            var handler = PreviewCanExecute;
            if (handler != null) handler(sender, args);
        }
    }
}