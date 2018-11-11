using System.ComponentModel;

namespace Ace
{
	[DataContract]
	public class Segregator : Segregator<object> { }

	[DataContract]
	public class Segregator<TValue> : INotifyPropertyChanging, INotifyPropertyChanged
	{
		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;
		private TValue _value;

		[DataMember]
		public TValue Value
		{
			get => _value;
			set
			{
				PropertyChanging?.Invoke(this, new PropertyChangingEventArgs("Value"));
				_value = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
			}
		}
	}
}