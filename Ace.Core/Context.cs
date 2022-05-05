using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Ace.Input;

namespace Ace
{
	public static class Context
	{
		public static ref TValue Notify<TValue>(
			this ref TValue value,
			SmartObject smartObject,
			[CallerMemberName]string propertyName = null) where TValue : struct
		{
			smartObject.EvokePropertyChanged(propertyName);
			return ref value;
		}

		public static TValue Notify<TValue>(
			this TValue value,
			SmartObject smartObject,
			[CallerMemberName]string propertyName = null) where TValue : class
		{
			smartObject.EvokePropertyChanged(propertyName);
			return value;
		}

		private static readonly Dictionary<string, Command> Container = new();

		public static Command Get(string key) =>
			Container.TryGetValue(key, out var value) ? value : Container[key] = new Command(key);

		public static void UpdateCanExecuteState()
		{
			foreach (var command in Container.Values) command.EvokeCanExecuteChanged();
		}

		public static Command Make => Get("Make");
		public static Command Login => Get("Login");
		public static Command Logout => Get("Logout");
		public static Command Refresh => Get("Refresh");
		public static Command Enter => Get("Exit");
		public static Command Exit => Get("Exit");
		public static Command New => Get("New");
		public static Command Open => Get("Open");
		public static Command Save => Get("Save");
		public static Command Dry => Get("Dry");
		public static Command Rate => Get("Rate");
		public static Command Buy => Get("Buy");
		public static Command Back => Get("Back");
		public static Command Next => Get("Next");
		public static Command Cut => Get("Cut");
		public static Command Copy => Get("Copy");
		public static Command Paste => Get("Paste");
		public static Command Delete => Get("Delete");
		public static Command Zoom => Get("Zoom");
		public static Command Move => Get("Move");
		public static Command Rotate => Get("Rotate");
		public static Command Navigate => Get("Navigate");
		public static Command Activate => Get("Activate");

		public class Set
		{
			public static Command Add => Get("Set.Add");
			public static Command Remove => Get("Set.Remove");
			public static Command Clear => Get("Set.Clear");
			public static Command Create => Get("Set.Create");
			public static Command Delete => Get("Set.Delete");
		}

		public static Mediator GetMediator(this ContextObject contextObject, string commandKey, object sender) =>
			new(sender, contextObject[Get(commandKey)]);

		public static Mediator GetMediator(this ContextObject contextObject, string commandKey) =>
			new(contextObject, contextObject[Get(commandKey)]);

		public static Mediator GetMediator(this ContextObject contextObject, ICommand command, object sender) =>
			new(sender, contextObject[command]);

		public static Mediator GetMediator(this ContextObject contextObject, ICommand command) =>
			new(contextObject, contextObject[command]);

		public static Mediator GetMediator(this ICommand command, ContextObject contextObject, object sender) =>
			new(sender, contextObject[command]);

		public static Mediator GetMediator(this ICommand command, ContextObject contextObject) =>
			new(contextObject, contextObject[command]);

		public static SmartSet<T> ToSet<T>(this IEnumerable<T> collection) => new(collection);
	}
}