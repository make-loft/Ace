using System.Collections.Generic;
using System.ComponentModel;

namespace Ace.Replication.Models
{
	public class Set : List<object>, INotifyPropertyChanged
	{
		public Set() { }
		public Set(IEnumerable<object> collection) : base(collection) { }

		public event PropertyChangedEventHandler PropertyChanged;

		public void EvokePropertyChanged(string propertyName = "Item[]") =>
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
	}
}