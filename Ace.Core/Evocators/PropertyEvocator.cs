using System;
using System.Collections.Generic;
using System.Linq;

namespace Ace.Evocators
{
	public class PropertyArgs : EventArgs
	{
		public object Sender { get; }
		public string PropertyName { get; }
		public PropertyArgs(object sender, string propertyName)
		{
			Sender = sender;
			PropertyName = propertyName;
		}
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

		public IEnumerable<object> GetErrors(string propertyName) =>
			ValidationRules.GetInvocationList().OfType<Func<string, object>>()
				.Select(validationHandler => validationHandler(propertyName));
	}

	public class PropertyEvocator
		: PropertyEvocator<PropertyArgs, PropertyArgs, PropertyArgs>
	{
		public PropertyEvocator(string propertyName) => Name = propertyName;
		public string Name { get; }
	}
}