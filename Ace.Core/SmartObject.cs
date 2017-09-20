using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Ace.Replication;

namespace Ace
{
    [DataContract]
    public class SmartObject : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public static object UndefinedValue = null;
        protected Dictionary<string, object> SmartContainer = new Dictionary<string, object>();
        public event PropertyChangingEventHandler PropertyChanging = (sender, args) => { };
        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        public const string SmartPropertyName = "Smart";

        public SmartObject Smart
        {
            get => this;
            set => RaisePropertyChanged(Equals(value) ? SmartPropertyName : null);
        }

        public object this[string key]
        {
            get => SmartContainer.TryGetValue(key, out var value) ? value : UndefinedValue;
            set
            {
                RaisePropertyChanging(SmartPropertyName);
                SmartContainer[key] = value;
                RaisePropertyChanged(SmartPropertyName);
            }
        }

        public object this[string key, object defaultValue] // {Binding Path='Smart[Test,1]'}
        {
            get => SmartContainer.TryGetValue(key, out var value) ? value : this[key] = defaultValue;
            set => this[key] = value;
        }

        public object this[string key, object defaultValue, bool segregate] // {Binding Path='Smart[Test,1,True].Value'}
        {
            get => this[key, segregate ? new Segregator {Value = defaultValue} : defaultValue];
            set => this[key] = value;
        }

        [DataMember]
        public virtual Dictionary<string, object> SmartState
        {
            get => GetSmartProperties(GetType(), SmartContainer).ToDictionary(p => p.Key, p => p.Value);
            set => value?.ForEach(pair => this[pair.Key] = pair.Value);
        }

        protected static IEnumerable<KeyValuePair<string, object>> GetSmartProperties(
            Type type, Dictionary<string, object> container, BindingFlags bindingFlags = Member.DefaultFlags) =>
            container.Where(p => !type.EnumerateMember(p.Key, bindingFlags).Any(m => m is PropertyInfo));

        public void RaisePropertyChanging(string propertyName) =>
            PropertyChanging(this, new PropertyChangingEventArgs(propertyName));

        public void RaisePropertyChanged(string propertyName) =>
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        [OnDeserializing]
        public void Initialize(StreamingContext context = default(StreamingContext))
        {
            if (PropertyChanging == null) PropertyChanging = (sender, args) => { };
            if (PropertyChanged == null) PropertyChanged = (sender, args) => { };
            if (SmartContainer == null) SmartContainer = new Dictionary<string, object>();
        }
    }
}