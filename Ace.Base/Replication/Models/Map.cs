using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Ace.Replication.Models
{
	public class Map : Dictionary<string, object>, INotifyPropertyChanged
	{
		public Map() { }
		public Map(IDictionary<string, object> dictionary) : base(dictionary) { }

		public event PropertyChangedEventHandler PropertyChanged;

		public void EvokePropertyChanged(string propertyName = "Item[]") =>
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

		public new void Add(string key, object value)
		{
			try
			{
				base.Add(key, value);
			}
			catch (Exception exception)
			{
				throw new Exception($"{key} : {value}", exception);
			}
		}
	}
}