using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace Ace
{
    [DataContract]
    public class ContextSet<T> : ContextObject, IList<T>, IList, INotifyCollectionChanged
    {
        [DataMember]
        public List<T> PassiveList { get; set; }

        public ContextSet() => Initialize();

        [OnDeserialized]
        public new void Initialize(StreamingContext context = default(StreamingContext))
        {
            PassiveList = PassiveList ?? new List<T>();
            CollectionChanged += (sender, args) => RaisePropertyChanged(() => Count);
        }

        public ContextSet(IEnumerable<T> collection)
        {
            PassiveList = new List<T>(collection);
            CollectionChanged += (sender, args) => RaisePropertyChanged(() => Count);
        }

        public IEnumerator<T> GetEnumerator() => PassiveList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item)
        {
            PassiveList.Add(item);
            CollectionChanged(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, PassiveList.Count - 1));
        }

        public int Add(object value)
        {
            PassiveList.Add((T) value);
            CollectionChanged(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, PassiveList.Count - 1));
            return 1;
        }

        void IList.Clear()
        {
            PassiveList.Clear();
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(object value) => PassiveList.Contains((T) value);

        public int IndexOf(object value) => PassiveList.IndexOf((T) value);

        public void Insert(int index, object value)
        {
            PassiveList.Insert(index, (T) value);
            CollectionChanged(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, PassiveList.Count - 1));
        }

        public void Remove(object value)
        {
            var index = PassiveList.IndexOf((T) value);
            PassiveList.Remove((T) value);
            CollectionChanged(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
        }

        void IList.RemoveAt(int index)
        {
            var value = PassiveList[index];
            PassiveList.RemoveAt(index);
            CollectionChanged(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
        }

        public void Clear()
        {
            PassiveList.Clear();
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool IsFixedSize => ((IList) PassiveList).IsFixedSize;

        bool IList.IsReadOnly => ((IList) PassiveList).IsReadOnly;

        public T this[int index]
        {
            get => PassiveList[index];
            set => PassiveList[index] = value;
        }

        object IList.this[int index]
        {
            get => PassiveList[index];
            set
            {
                var oldValue = PassiveList[index];
                PassiveList[index] = (T) value;
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace, value, oldValue, index));
            }
        }

        void ICollection<T>.Clear()
        {
            PassiveList.Clear();
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item) => PassiveList.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => PassiveList.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            var index = PassiveList.IndexOf(item);
            var result = PassiveList.Remove(item);
            CollectionChanged(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            return result;
        }

        public void CopyTo(Array array, int index) => ((ICollection) PassiveList).CopyTo(array, index);

        int ICollection.Count => PassiveList.Count;

        public bool IsSynchronized => ((ICollection) PassiveList).IsSynchronized;

        public object SyncRoot => ((ICollection) PassiveList).SyncRoot;

        int ICollection<T>.Count => PassiveList.Count;

        bool ICollection<T>.IsReadOnly => ((ICollection<T>) PassiveList).IsReadOnly;

        public int IndexOf(T item) => PassiveList.IndexOf(item);

        public void Insert(int index, T item)
        {
            PassiveList.Insert(index, item);
            CollectionChanged(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        void IList<T>.RemoveAt(int index)
        {
            var item = PassiveList[index];
            PassiveList.RemoveAt(index);
            CollectionChanged(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        T IList<T>.this[int index]
        {
            get => PassiveList[index];
            set
            {
                PassiveList[index] = value;
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace));
            }
        }

        public int Count => PassiveList.Count;

        public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, args) => { };
    }
}