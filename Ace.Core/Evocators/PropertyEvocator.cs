using System;
using System.Collections.Generic;
using System.Linq;

namespace Ace.Evocators;

public class PropertyArgs(object sender, string propertyName) : EventArgs
{
	public object Sender { get; } = sender;
	public string PropertyName { get; } = propertyName;
}

public class PropertyEvocator<TPropertyChanging, TPropertyChanged, TErrorsChanged>
	where TPropertyChanging : EventArgs
	where TPropertyChanged : EventArgs
	where TErrorsChanged : EventArgs
{
	public event Action<TPropertyChanging> Changing;
	public event Action<TPropertyChanged> Changed;
	public event Action<TErrorsChanged> ErrorsChanged;
	public event Func<string, object> ValidationRules = propertyName => null;

	public void EvokeChanging(TPropertyChanging args) => Changing?.Invoke(args);
	public void EvokeChanged(TPropertyChanged args) => Changed?.Invoke(args);
	public void EvokeErrorsChanged(TErrorsChanged args) => ErrorsChanged?.Invoke(args);

	public IEnumerable<object> GetErrors(string propertyName) => ValidationRules.GetInvocationList()
		.OfType<Func<string, object>>()
		.Select(validationHandler => validationHandler(propertyName))
		;
}

public class PropertyEvocator(string propertyName)
		: PropertyEvocator<PropertyArgs, PropertyArgs, PropertyArgs>
{
	public string Name { get; } = propertyName;
}