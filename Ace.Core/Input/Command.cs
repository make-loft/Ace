﻿using System;
using System.Windows.Input;

namespace Ace.Input
{
	public class Command : ICommand
	{
		private const string UsingMessage =
			"Please, use 'Context.Mediator' class or 'ContextObject.GetMediator' method for execute.";

		public string Name { get; internal set; }
		public Command(string name) => Name = name;
		public override string ToString() => "[" + Name + "]";
		public void EvokeCanExecuteChanged() => CanExecuteChanged(this, EventArgs.Empty);
		bool ICommand.CanExecute(object parameter) => throw new Exception(UsingMessage);
		void ICommand.Execute(object parameter) => throw new Exception(UsingMessage);
		public event EventHandler CanExecuteChanged = (sender, args) => { };
	}
}