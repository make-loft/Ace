using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Aero
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
            set
            {
                if (Equals(this, value)) RaisePropertyChanged(SmartPropertyName);
            }
            get => this;
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

        public object this[string key, object defaultValue] /* {Binding Path='Smart[Test,1]'} */
        {
            get => SmartContainer.TryGetValue(key, out var value) ? value : this[key] = defaultValue;
            set => this[key] = value;
        }

        public object this[string key, object defaultValue, bool segregate] /* {Binding Path='Smart[Test,1,True].Value'} */
        {
            get => this[key, segregate ? new Segregator {Value = defaultValue} : defaultValue];
            set => this[key] = value;
        }

        [DataMember]
        public Dictionary<string, object> SmartState
        {
            get
            {
                var thisType = GetType();
                return SmartContainer.Where(p => thisType.GetProperty(p.Key) == null)
                    .ToDictionary(p => p.Key, p => p.Value is ValueType ? p.Value.ToString() : p.Value);
            }
            set => value?.ForEach(pair => this[pair.Key] = pair.Value);
        }

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