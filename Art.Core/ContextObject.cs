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
        public ContextObject()
        {
            Initialize();
        }

        public Dictionary<ICommand, CommandEvocator> CommandEvocators { get; private set; }
        public Dictionary<string, PropertyEvocator> PropertyEvocators { get; private set; }

        public CommandEvocator this[ICommand command]
        {
            get
            {
                CommandEvocator evocator;
                if (CommandEvocators.TryGetValue(command, out evocator)) return evocator;
                return CommandEvocators[command] = new CommandEvocator(command);
            }
        }

        public PropertyEvocator this[Expression<Func<object>> expression]
        {
            get
            {
                PropertyEvocator evocator;
                var propertyName = Member.ExtractName(expression);
                if (PropertyEvocators.TryGetValue(propertyName, out evocator)) return evocator;
                return PropertyEvocators[propertyName] = new PropertyEvocator(propertyName);
            }
        }

        public TValue Get<TValue>(Expression<Func<TValue>> expression, TValue defaultValue = default(TValue))
        {
            var propertyName = Member.ExtractName(expression);
            return (TValue) base[propertyName, defaultValue];
        }

        public void Set<TValue>(Expression<Func<TValue>> expression, TValue value, bool checkEquals = false)
        {
            if (checkEquals && Equals(Get(expression), value)) return;
            var propertyName = Member.ExtractName(expression);
            RaisePropertyChanging(propertyName);
            base[propertyName] = value;
            RaisePropertyChanged(propertyName);
        }

        public void RaisePropertyChanging<TValue>(Expression<Func<TValue>> expression)
        {
            var propertyName = Member.ExtractName(expression);
            RaisePropertyChanging(propertyName);
        }

        public void RaisePropertyChanged<TValue>(Expression<Func<TValue>> expression)
        {
            var propertyName = Member.ExtractName(expression);
            RaisePropertyChanged(propertyName);
        }

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
            PropertyEvocator evocator;
            var propertyName = args.PropertyName;
            if (PropertyEvocators.TryGetValue(propertyName, out evocator))
                evocator.EvokePropertyChanging(sender, args);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            PropertyEvocator evocator;
            var propertyName = args.PropertyName;
            if (PropertyEvocators.TryGetValue(propertyName, out evocator))
                evocator.EvokePropertyChanged(sender, args);
        }

        #region Validation Core

        public virtual string Error
        {
            get { return Get(() => Error); }
            protected set { Set(() => Error, value); }
        }

        public virtual bool HasErrors
        {
            get { return Get(() => HasErrors); }
            protected set { Set(() => HasErrors, value); }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = (sender, args) => { };

        public IEnumerable GetErrors(string propertyName)
        {
            PropertyEvocator evocator;
            return PropertyEvocators.TryGetValue(propertyName, out evocator)
                ? evocator.GetErrors(propertyName)
                : Enumerable.Empty<object>();
        }

        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs args)
        {
            PropertyEvocator evocator;
            var propertyName = args.PropertyName;
            if (PropertyEvocators.TryGetValue(propertyName, out evocator))
                evocator.EvokeErrorsChanged(sender, args);
        }

        public void RaiseErrorsChanged<TValue>(Expression<Func<TValue>> expression)
        {
            var propertyName = Member.ExtractName(expression);
            ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

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