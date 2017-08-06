using System;
using System.Windows.Input;

namespace Art.Input
{
    public class Command : ICommand
    {
        private const string UsingMessage =
            "Please, use 'Context.Mediator' class or 'ContextObject.GetMediator' method for execute.";

        public string Name { get; internal set; }
        public Command(string name) => Name = name;
        public new string ToString() => "[" + Name + "]";
        public void RaiseCanExecuteChanged() => CanExecuteChanged(this, EventArgs.Empty);
        bool ICommand.CanExecute(object parameter) => throw new Exception(UsingMessage);
        void ICommand.Execute(object parameter) => throw new Exception(UsingMessage);
        public event EventHandler CanExecuteChanged = (sender, args) => { };
    }
}