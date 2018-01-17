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
		[DataMember]
		public List<T> Source { get; set; }

		public SmartSet() => Initialize();

		[OnDeserialized]
		public void Initialize(StreamingContext context = default(StreamingContext))
		{
			Source = Source ?? new List<T>();
			CollectionChanged = null;
			CollectionChanged += (sender, args) => RaisePropertyChanged("Count");
		}

		public SmartSet(IEnumerable<T> collection)
		{
			Source = new List<T>(collection);
			CollectionChanged = null;
			CollectionChanged += (sender, args) => RaisePropertyChanged("Count");
		}

		public IEnumerator<T> GetEnumerator() => Source.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Add(T item)
		{
			Source.Add(item);
			CollectionChanged(this,
				new Args(NotifyCollectionChangedAction.Add, item, Source.Count - 1));
		}

		public int Add(object value)
		{
			Source.Add((T) value);
			CollectionChanged(this,
				new Args(NotifyCollectionChangedAction.Add, value, Source.Count - 1));
			return 1;
		}

		public void AddRange(IEnumerable<T> collection)
		{
			Source.AddRange(collection);
			CollectionChanged(this, new Args(NotifyCollectionChangedAction.Reset));
		}

		void IList.Clear()
		{
			Source.Clear();
			CollectionChanged(this, new Args(NotifyCollectionChangedAction.Reset));
		}

		public bool Contains(object value) => Source.Contains((T) value);

		public int IndexOf(object value) => Source.IndexOf((T) value);

		public void Insert(int index, object value)
		{
			Source.Insert(index, (T) value);
			CollectionChanged(this,
				new Args(NotifyCollectionChangedAction.Add, value, Source.Count - 1));
		}

		public void Remove(object value)
		{
			var index = Source.IndexOf((T) value);
			Source.Remove((T) value);
			CollectionChanged(this,
				new Args(NotifyCollectionChangedAction.Remove, value, index));
		}

		void IList.RemoveAt(int index)
		{
			var value = Source[index];
			Source.RemoveAt(index);
			CollectionChanged(this,
				new Args(NotifyCollectionChangedAction.Remove, value, index));
		}

		public void Clear()
		{
			Source.Clear();
			CollectionChanged(this, new Args(NotifyCollectionChangedAction.Reset));
		}

		public bool IsFixedSize => ((IList) Source).IsFixedSize;

		bool IList.IsReadOnly => ((IList) Source).IsReadOnly;

		public T this[int index]
		{
			get => Source[index];
			set => Source[index] = value;
		}

		object IList.this[int index]
		{
			get => Source[index];
			set
			{
				var oldValue = Source[index];
				Source[index] = (T) value;
				CollectionChanged(this, new Args(NotifyCollectionChangedAction.Replace, value, oldValue, index));
			}
		}

		void ICollection<T>.Clear()
		{
			Source.Clear();
			CollectionChanged(this, new Args(NotifyCollectionChangedAction.Reset));
		}

		public bool Contains(T item) => Source.Contains(item);

		public void CopyTo(T[] array, int arrayIndex) => Source.CopyTo(array, arrayIndex);

		public bool Remove(T item)
		{
			var index = Source.IndexOf(item);
			var result = Source.Remove(item);
			CollectionChanged(this, new Args(NotifyCollectionChangedAction.Remove, item, index));
			return result;
		}

		public void CopyTo(Array array, int index) => ((ICollection) Source).CopyTo(array, index);

		int ICollection.Count => Source.Count;

		public bool IsSynchronized => ((ICollection) Source).IsSynchronized;

		public object SyncRoot => ((ICollection) Source).SyncRoot;

		int ICollection<T>.Count => Source.Count;

		bool ICollection<T>.IsReadOnly => ((ICollection<T>) Source).IsReadOnly;

		public int IndexOf(T item) => Source.IndexOf(item);

		public void Insert(int index, T item)
		{
			Source.Insert(index, item);
			CollectionChanged(this, new Args(NotifyCollectionChangedAction.Add, item, index));
		}

		void IList<T>.RemoveAt(int index)
		{
			var item = Source[index];
			Source.RemoveAt(index);
			CollectionChanged(this, new Args(NotifyCollectionChangedAction.Remove, item, index));
		}

		T IList<T>.this[int index]
		{
			get => Source[index];
			set
			{
				Source[index] = value;
				CollectionChanged(this, new Args(NotifyCollectionChangedAction.Replace));
			}
		}

		public int Count => Source.Count;

		public void RaiseCollectionChanged() => CollectionChanged(this, new Args(NotifyCollectionChangedAction.Reset));
		public void RaiseCollectionChanged(object sender, Args args) => CollectionChanged(sender, args);

		public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, args) => { };
	}
}