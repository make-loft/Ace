using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using Args = System.Collections.Specialized.NotifyCollectionChangedEventArgs;

namespace Ace
{
	[DataContract]
	public class SmartSet<T> : SmartObject, IList<T>, IList, INotifyCollectionChanged
	{
		public SmartSet() => Initialize();	
		public SmartSet(IEnumerable<T> collection) => Initialize(collection);
		[OnDeserialized] public void OnDeserialized(StreamingContext context) => Initialize();
		
		public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, args) => { };
		public void RaiseCollectionChanged(object sender, Args args) => CollectionChanged(sender, args);
		public void RaiseCollectionChanged() => CollectionChanged(this, new Args(NotifyCollectionChangedAction.Reset));
		
		private void Initialize(IEnumerable<T> collection = null)
		{
			Source = Source ?? (collection == null ? new List<T>() : new List<T>(collection));
			CollectionChanged = null;
			CollectionChanged += (sender, args) => RaisePropertyChanged("Count");
		}
			
		[DataMember]
		public List<T> Source { get; set; }
	
		public int Count => Source.Count;
		public object SyncRoot => ((IList) Source).SyncRoot;
		public bool IsReadOnly => ((IList) Source).IsReadOnly;
		public bool IsFixedSize => ((IList) Source).IsFixedSize;
		public bool IsSynchronized => ((IList) Source).IsSynchronized;
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();		
		public IEnumerator<T> GetEnumerator() => Source.GetEnumerator();
		public void CopyTo(T[] array, int arrayIndex) => Source.CopyTo(array, arrayIndex);
		public void CopyTo(Array array, int index) => ((ICollection) Source).CopyTo(array, index);
		public bool Contains(T value) => Source.Contains(value);
		public bool Contains(object value) => Source.Contains((T) value);
		public int IndexOf(T value) => Source.IndexOf(value);	
		public int IndexOf(object value) => Source.IndexOf((T) value);
		public void Add(T value) => Do(NotifyCollectionChangedAction.Add, value);
		public int Add(object value) => Do(NotifyCollectionChangedAction.Add, (T)value);
		public void Insert(int index, T value) => Do(NotifyCollectionChangedAction.Add, value, index);
		public void Insert(int index, object value) => Do(NotifyCollectionChangedAction.Add, (T)value, index);
		public bool Remove(T value) => Do(NotifyCollectionChangedAction.Remove, value) == 1;
		public void Remove(object value) => Do(NotifyCollectionChangedAction.Remove, (T)value);
		public void RemoveAt(int index) => Do(NotifyCollectionChangedAction.Remove, Source[index], index);
		public void Clear() => Do(NotifyCollectionChangedAction.Reset, default(T));
		
		public T this[int index]
		{
			get => Source[index];
			set => Do(NotifyCollectionChangedAction.Replace, value, index);
		}

		object IList.this[int index]
		{
			get => this[index];
			set => this[index] = (T) value;
		}

		private int Do(NotifyCollectionChangedAction action, T value, int index = -1)
		{
			switch (action)
			{
				case NotifyCollectionChangedAction.Add:
					if (index < 0) Source.Add(value);
					else Source.Insert(index, value);
					CollectionChanged(this, new Args(action, value, index));
					break;
				case NotifyCollectionChangedAction.Remove:
					index = index < 0 ? Source.IndexOf(value) : index;
					if (index < 0) return 0;
					Source.RemoveAt(index);
					CollectionChanged(this, new Args(action, value, index));
					break;
				case NotifyCollectionChangedAction.Replace:
					var oldValue = Source[index];
					Source[index] = value;
					CollectionChanged(this, new Args(action, value, oldValue, index));
					break;
				case NotifyCollectionChangedAction.Move:
					break;
				case NotifyCollectionChangedAction.Reset:
					Source.Clear();
					CollectionChanged(this, new Args(action));
					break;
			}
			
			return 1;
		}
	}
}