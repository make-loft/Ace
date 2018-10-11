#if !NETSTANDARD
using System.Collections;

// ReSharper disable once CheckNamespace

namespace System.ComponentModel
{
	public interface IDataErrorInfo
	{
		string Error { get; }

		string this[string columnName] { get; }
	}

	//public interface INotifyDataErrorInfo
	//{
	//	bool HasErrors { get; }

	//	event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

	//	IEnumerable GetErrors(string propertyName);
	//}

	//public sealed class DataErrorsChangedEventArgs : EventArgs
	//{
	//	public string PropertyName { get; }

	//	public DataErrorsChangedEventArgs(string propertyName)
	//	{
	//		PropertyName = propertyName;
	//	}
	//}

	public class PropertyChangingEventArgs : EventArgs
	{
		public virtual string PropertyName { get; private set; }

		public PropertyChangingEventArgs(string propertyName)
		{
			PropertyName = propertyName;
		}
	}

	public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs e);

	public interface INotifyPropertyChanging
	{
		event PropertyChangingEventHandler PropertyChanging;
	}
}
#endif