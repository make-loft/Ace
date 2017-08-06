using System.ComponentModel;
using System.Runtime.Serialization;

namespace Art
{
    [DataContract]
    public class Segregator : Segregator<object>
    {
    }

    [DataContract]
    public class Segregator<TValue> : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangingEventHandler PropertyChanging = (sender, args) => { };
        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };
        private TValue _value;

        [DataMember]
        public TValue Value
        {
            get => _value;
            set
            {
                PropertyChanging(this, new PropertyChangingEventArgs("Value"));
                _value = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        [OnDeserializing]
        public void Initialize(StreamingContext context = default(StreamingContext))
        {
            PropertyChanging = (sender, args) => { };
            PropertyChanged = (sender, args) => { };
        }
    }
}