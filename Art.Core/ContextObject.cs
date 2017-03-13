using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Windows.Input;
using Aero.Evocators;

namespace Aero
{
    [DataContract]
    public class ContextObject : SmartObject, INotifyDataErrorInfo, IDataErrorInfo
    {
        public ContextObject() => Initialize();

        public Dictionary<ICommand, CommandEvocator> CommandEvocators { get; private set; }
        public Dictionary<string, PropertyEvocator> PropertyEvocators { get; private set; }

        public CommandEvocator this[ICommand command] =>
            CommandEvocators.TryGetValue(command, out var evocator)
                ? evocator
                : CommandEvocators[command] = new CommandEvocator(command);

        public PropertyEvocator this[Expression<Func<object>> expression]
        {
            get
            {
                var propertyName = expression.ExtractName();
                return PropertyEvocators.TryGetValue(propertyName, out var evocator)
                    ? evocator
                    : PropertyEvocators[propertyName] = new PropertyEvocator(propertyName);
            }
        }

        public TValue Get<TValue>(Expression<Func<TValue>> expression, TValue defaultValue = default(TValue)) =>
            (TValue) base[expression.ExtractName(), defaultValue];

        public void Set<TValue>(Expression<Func<TValue>> expression, TValue value, bool checkEquals = false)
        {
            if (checkEquals && Equals(Get(expression), value)) return;
            var propertyName = expression.ExtractName();
            RaisePropertyChanging(propertyName);
            base[propertyName] = value;
            RaisePropertyChanged(propertyName);
        }

        public void RaisePropertyChanging<TValue>(Expression<Func<TValue>> expression) =>
            RaisePropertyChanging(expression.ExtractName());

        public void RaisePropertyChanged<TValue>(Expression<Func<TValue>> expression) =>
            RaisePropertyChanged(expression.ExtractName());

        [OnDeserializing]
        public new void Initialize(StreamingContext context = default(StreamingContext))
        {
            CommandEvocators = new Dictionary<ICommand, CommandEvocator>();
            PropertyEvocators = new Dictionary<string, PropertyEvocator>();
            PropertyChanging += OnPropertyChanging;
            PropertyChanged += OnPropertyChanged;
            ErrorsChanged += OnErrorsChanged;
        }

        private void OnPropertyChanging(object sender, PropertyChangingEventArgs args)
        {
            if (PropertyEvocators.TryGetValue(args.PropertyName, out var evocator))
                evocator.EvokePropertyChanging(sender, args);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (PropertyEvocators.TryGetValue(args.PropertyName, out var evocator))
                evocator.EvokePropertyChanged(sender, args);
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
                ? evocator.GetErrors(propertyName)
                : Enumerable.Empty<object>();

        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs args)
        {
            if (PropertyEvocators.TryGetValue(args.PropertyName, out var evocator))
                evocator.EvokeErrorsChanged(sender, args);
        }

        public void RaiseErrorsChanged<TValue>(Expression<Func<TValue>> expression) =>
            ErrorsChanged(this, new DataErrorsChangedEventArgs(expression.ExtractName()));

        public void RaiseErrorsChanged(string propertyName) =>
            ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                var errors = GetErrors(propertyName).Cast<object>().ToList();
                return errors.Any()
                    ? errors.Select(e => (e ?? "").ToString()).Aggregate((x, y) => x + Environment.NewLine + y)
                    : null;
            }
        }

        #endregion
    }
}