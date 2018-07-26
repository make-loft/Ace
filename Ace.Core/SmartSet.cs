using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using Action = System.Collections.Specialized.NotifyCollectionChangedAction;
using Args = System.Collections.Specialized.NotifyCollectionChangedEventArgs;

namespace Ace
{
	[DataContract]
	public class SmartSet<T> : SmartSet<T, List<T>>
	{
		public SmartSet() => Initialize();
		public SmartSet(IEnumerable<T> collection) => Initialize(new List<T>(collection));
	}

	[DataContract]
	public class SmartSet<T, TList> : SmartObject, IList<T>, IList, INotifyCollectionChanged
		where TList: class , IList<T>, new()
	{
		public SmartSet() => Initialize();
		public SmartSet(TList collection) => Initialize(collection);
		[OnDeserialized] public void OnDeserialized(StreamingContext context) => Initialize();
		
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event NotifyCollectionChangedEventHandler CollectionChangeCompleted;
		public void RaiseCollectionChanged(object sender, Args args) => CollectionChanged?.Invoke(sender, args);
		public void RaiseCollectionChanged() => CollectionChanged?.Invoke(this, new Args(Action.Reset));
		
		protected void Initialize(TList collection = null)
		{
			Source = Source ?? collection ?? new TList();
			CollectionChanged = null;
			CollectionChanged += (sender, args) => RaisePropertyChanged("Count");
		}
			
		[DataMember]
		public TList Source { get; set; }
	
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
		public void Add(T value) => Change(Action.Add, value);
		public int Add(object value) => Change(Action.Add, (T)value);
		public void Insert(int index, T value) => Change(Action.Add, value, index);
		public void Insert(int index, object value) => Change(Action.Add, (T)value, index);
		public bool Remove(T value) => Change(Action.Remove, value) == 1;
		public void Remove(object value) => Change(Action.Remove, (T)value);
		public void RemoveAt(int index) => Change(Action.Remove, Source[index], index);
		public void Clear() => Change(Action.Reset, default(T));
		
		public T this[int index]
		{
			get => Source[index];
			set => Change(Action.Replace, value, index);
		}

		object IList.this[int index]
		{
			get => this[index];
			set => this[index] = (T) value;
		}

		private int Change(Action action, T value, int index = -1)
		{
			Args args = null;
			switch (action)
			{
				case Action.Add:
					if (index < 0) Source.Add(value);
					else Source.Insert(index, value);
					index = index < 0 ? Source.Count - 1 : index;
					args = new Args(action, new[] {value}, index);
					break;
				case Action.Remove:
					index = index < 0 ? Source.IndexOf(value) : index;
					if (index < 0) return 0;
					Source.RemoveAt(index);
					args = new Args(action, new[] {value}, index);
					break;
				case Action.Replace:
					var oldValue = Source[index];
					Source[index] = value;
					args = new Args(action, new[] {value}, new[] {oldValue}, index);
					break;
				//case Action.Move:
					//break;
				case Action.Reset:
					Source.Clear();
					args = new Args(action);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(action), action.ToString());
			}

			CollectionChanged?.Invoke(this, args);
			CollectionChangeCompleted?.Invoke(this, args);
			return 1;
		}
	}
}