using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Ace.Evocators
{
	public class PropertyEvocator<TPropertyChanging, TPropertyChanged, TErrorsChanged>
		where TPropertyChanging : EventArgs where TPropertyChanged : EventArgs where TErrorsChanged : EventArgs
	{
		public event EventHandler<TPropertyChanging> PropertyChanging = (sender, args) => { };
		public event EventHandler<TPropertyChanged> PropertyChanged = (sender, args) => { };
		public event EventHandler<TErrorsChanged> ErrorsChanged = (sender, args) => { };
		public event Func<string, object> ValidationRules = propertyName => null;

		public void EvokePropertyChanging(object sender, TPropertyChanging args) => PropertyChanging(sender, args);
		public void EvokePropertyChanged(object sender, TPropertyChanged args) => PropertyChanged(sender, args);
		public void EvokeErrorsChanged(object sender, TErrorsChanged args) => ErrorsChanged(sender, args);

		public IEnumerable<object> GetErrors(string propertyName) =>
			ValidationRules.GetInvocationList().OfType<Func<string, object>>()
				.Select(validationHandler => validationHandler(propertyName));
	}

	public class PropertyEvocator : 
		PropertyEvocator<PropertyChangingEventArgs, PropertyChangedEventArgs, DataErrorsChangedEventArgs>
	{
		public PropertyEvocator(string propertyName) => PropertyName = propertyName;
		public string PropertyName { get; }
	}
}