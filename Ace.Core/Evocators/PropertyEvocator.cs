using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Ace.Evocators
{
	public class PropertyEvocator<TPropertyChanging, TPropertyChanged, TErrorsChanged>
		where TPropertyChanging : EventArgs where TPropertyChanged : EventArgs where TErrorsChanged : EventArgs
	{
		//public event EventHandler<TPropertyChanging> PropertyChanging = (sender, args) => { };
		public event EventHandler<TPropertyChanging> Changing = (sender, args) => { };
		//public event EventHandler<TPropertyChanged> PropertyChanged = (sender, args) => { };
		public event EventHandler<TPropertyChanged> Changed = (sender, args) => { };
		public event EventHandler<TErrorsChanged> ErrorsChanged = (sender, args) => { };
		public event Func<string, object> ValidationRules = propertyName => null;

		public void EvokeChanging(object sender, TPropertyChanging args) => Changing(sender, args);
		public void EvokeChanged(object sender, TPropertyChanged args) => Changed(sender, args);
		public void EvokeErrorsChanged(object sender, TErrorsChanged args) => ErrorsChanged(sender, args);

		public IEnumerable<object> GetErrors(string propertyName) =>
			ValidationRules.GetInvocationList().OfType<Func<string, object>>()
				.Select(validationHandler => validationHandler(propertyName));
	}

	public class PropertyEvocator : 
		PropertyEvocator<PropertyChangingEventArgs, PropertyChangedEventArgs, DataErrorsChangedEventArgs>
	{
		public PropertyEvocator(string propertyName) => Name = propertyName;
		public string Name { get; }
	}
}