using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Windows.Input;
using Ace.Evocators;

namespace Ace
{
	[DataContract]
	public class ContextObject : SmartObject, INotifyDataErrorInfo, IDataErrorInfo
	{
		public ContextObject() => Initialize();

		public Dictionary<ICommand, CommandEvocator> CommandEvocators { get; private set; }
		public Dictionary<string, PropertyEvocator> PropertyEvocators { get; private set; }

		public CommandEvocator this[ICommand command] => GetEvocator(command);
		public PropertyEvocator this[Expression<Func<object>> expression] => GetEvocator(expression.UnboxMemberName());

		public CommandEvocator GetEvocator(ICommand command) =>
			CommandEvocators.TryGetValue(command, out var evocator)
				? evocator
				: CommandEvocators[command] = new CommandEvocator(command);

		public PropertyEvocator GetEvocator(string propertyName) =>
			PropertyEvocators.TryGetValue(propertyName, out var evocator)
				? evocator
				: PropertyEvocators[propertyName] = new PropertyEvocator(propertyName);

		[OnDeserializing]
		public void Initialize(StreamingContext context = default(StreamingContext))
		{
			CommandEvocators = new Dictionary<ICommand, CommandEvocator>();
			PropertyEvocators = new Dictionary<string, PropertyEvocator>();
			PropertyChanging += (sender, args) => GetEvocator(args.PropertyName).EvokePropertyChanging(sender, args);
			PropertyChanged += (sender, args) => GetEvocator(args.PropertyName).EvokePropertyChanged(sender, args);
			ErrorsChanged += (sender, args) => GetEvocator(args.PropertyName).EvokeErrorsChanged(sender, args);
		}

		#region Validation Core

		public virtual string Error
		{
			get => Get(() => Error);
			protected set => Set(() => Error, value);
		}

		public virtual bool HasErrors
		{
			get => Get(() => HasErrors);
			protected set => Set(() => HasErrors, value);
		}

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = (sender, args) => { };

		public IEnumerable GetErrors(string propertyName) =>
			PropertyEvocators.TryGetValue(propertyName, out var evocator)
				? evocator.GetErrors(propertyName).Where(e => e.Is())
				: Enumerable.Empty<object>();

		public void RaiseErrorsChanged<TValue>(Expression<Func<TValue>> expression) => 
			ErrorsChanged(this, new DataErrorsChangedEventArgs(expression.UnboxMemberName()));

		public void RaiseErrorsChanged(string propertyName) =>
			ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));

		string IDataErrorInfo.this[string propertyName] =>
			GetErrors(propertyName).Cast<object>().Select(e => e.ToString())
				.Aggregate((string) null, (x, y) => $"{x}{(x.Is() ? Environment.NewLine : null)}{y}");

		#endregion
	}
}