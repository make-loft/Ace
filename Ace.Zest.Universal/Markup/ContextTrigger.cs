using System;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace Aero.Markup
{
    public class ContextTrigger : DependencyObject
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof (ICommand), typeof (ContextTrigger), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof (object), typeof (ContextTrigger), new PropertyMetadata(default(object)));

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

		public bool UseEventArgsAsCommandParameter { get; set; }

        public string EventName { get; set; }

        internal object Element { get; set; }

        public void ExecuteCommand(object sender, EventArgs eventArgs)
        {
            var mediator = Command as Input.Mediator;
            if (mediator != null) mediator.SetSender(sender);
            var parameter = UseEventArgsAsCommandParameter ? eventArgs : CommandParameter;
            Command.Execute(parameter);
        }
    }
}
