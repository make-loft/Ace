using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Art.Evocators
{
    public class PropertyEvocator
    {
        public PropertyEvocator(string propertyName) => PropertyName = propertyName;
        public event EventHandler<PropertyChangingEventArgs> PropertyChanging = (sender, args) => { };
        public event EventHandler<PropertyChangedEventArgs> PropertyChanged = (sender, args) => { };
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = (sender, args) => { };
        public event Func<string, object> ValidationRules = propertyName => null;
        public string PropertyName { get; }

        public void EvokePropertyChanging(object sender, PropertyChangingEventArgs args) =>
            PropertyChanging(sender, new PropertyChangingEventArgs(PropertyName));

        public void EvokePropertyChanged(object sender, PropertyChangedEventArgs args) =>
            PropertyChanged(sender, new PropertyChangedEventArgs(PropertyName));

        public void EvokeErrorsChanged(object sender, DataErrorsChangedEventArgs args) =>
            ErrorsChanged(sender, new DataErrorsChangedEventArgs(PropertyName));

        public IEnumerable<object> GetErrors(string propertyName) =>
            ValidationRules.GetInvocationList().OfType<Func<string, object>>()
                .Select(validationHandler => validationHandler(propertyName));
    }
}