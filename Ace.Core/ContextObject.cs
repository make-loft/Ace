using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;

using Ace.Evocators;

namespace Ace;

[DataContract]
public class ContextObject : SmartObject, INotifyDataErrorInfo, IDataErrorInfo
{
	public ContextObject() => Initialize();

	public Dictionary<ICommand, CommandEvocator> CommandEvocators { get; private set; }
	public Dictionary<string, PropertyEvocator> PropertyEvocators { get; private set; }

	public CommandEvocator this[ICommand command] => GetEvocator(command);
	public PropertyEvocator this[Expression<Func<object>> expression] => GetEvocator(expression.UnboxMemberName());

	public CommandEvocator GetEvocator(ICommand command) => CommandEvocators.TryGetValue(command, out var evocator)
		? evocator
		: CommandEvocators[command] = new(command)
		;

	public PropertyEvocator GetEvocator(string propertyName) => PropertyEvocators.TryGetValue(propertyName, out var evocator)
		? evocator
		: PropertyEvocators[propertyName] = new(propertyName)
		;

	[OnDeserializing]
	public void Initialize(StreamingContext context = default)
	{
		CommandEvocators = [];
		PropertyEvocators = [];
		PropertyChanging += (sender, args) => GetEvocator(args.PropertyName).EvokeChanging(new(sender, args.PropertyName));
		PropertyChanged += (sender, args) => GetEvocator(args.PropertyName).EvokeChanged(new(sender, args.PropertyName));
		ErrorsChanged += (sender, args) => GetEvocator(args.PropertyName).EvokeErrorsChanged(new(sender, args.PropertyName));
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

	public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

	public IEnumerable GetErrors(string propertyName) => PropertyEvocators.TryGetValue(propertyName, out var evocator)
		? evocator.GetErrors(propertyName).Where(e => e.Is())
		: Enumerable.Empty<object>()
		;

	public void EvokeErrorsChanged<TValue>(Expression<Func<TValue>> expression)
		=> ErrorsChanged(this, new(expression.UnboxMemberName()));

	public void EvokeErrorsChanged(string propertyName) => ErrorsChanged(this, new(propertyName));

	string IDataErrorInfo.this[string propertyName] => GetErrors(propertyName)
		.Cast<object>()
		.Select(e => e.ToString())
		.Aggregate((string) default, (x, y) => $"{x}{(x.Is() ? Environment.NewLine : null)}{y}")
		;

	#endregion
}