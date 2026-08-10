using System.ComponentModel;
#if MAUI
//using INotifyPropertyChanging = Microsoft.Maui.Controls.INotifyPropertyChanging;
using PropertyChangingEventHandler = System.ComponentModel.PropertyChangingEventHandler;
#endif

namespace Ace;

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
			PropertyChanging?.Invoke(this, new("Value"));
			_value = value;
			PropertyChanged?.Invoke(this, new("Value"));
		}
	}
}