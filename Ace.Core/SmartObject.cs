using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Ace
{
	[DataContract]
	public class SmartObject : INotifyPropertyChanging, INotifyPropertyChanged
	{
		public static object UndefinedSmartValue = null;
		private Dictionary<string, object> _smartContainer;
		protected Dictionary<string, object> SmartContainer => New.Lazy(ref _smartContainer);

		public SmartObject Smart
		{
			get => this;
			set => EvokePropertyChanged(this.Is(value) ? nameof(Smart) : throw new Exception("Wrong context"));
		}

		public object this[string key]
		{
			get => SmartContainer.TryGetValue(key, out var value) ? value : UndefinedSmartValue;
			set
			{
				EvokeSmartPropertyChanging(key);
				EvokePropertyChanging(nameof(Smart));
				SmartContainer[key] = value;
				EvokeSmartPropertyChanged(key);
				EvokePropertyChanged(nameof(Smart));
			}
		}

		public object this[string key, object defaultValue] // {Binding Path='Smart[Test,1]'}
		{
			get => SmartContainer.TryGetValue(key, out var value) ? value : this[key] = defaultValue;
			set => this[key] = value;
		}

		public object this[string key, object defaultValue, bool segregate] => // {Binding Path='Smart[Test,1,True].Value'}
			this[key, segregate ? new Segregator { Value = defaultValue } : defaultValue];

		[DataMember]
		public virtual Dictionary<string, object> SmartState
		{
			get => GetSmartProperties(SmartContainer, GetType())?.ToDictionary();
			set => value?.ForEach(pair => this[pair.Key] = pair.Value);
		}

		protected static IEnumerable<KeyValuePair<string, object>> GetSmartProperties(
			Dictionary<string, object> smartContainer, Type smartType, BindingFlags bindingFlags = Member.DefaultFlags) =>
			smartContainer?.Where(p => !smartType.EnumerateMember(p.Key, bindingFlags).Any(m => m is PropertyInfo));

		#region Notification Core

		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangingEventHandler SmartPropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyChangedEventHandler SmartPropertyChanged;

		public void EvokePropertyChanging(string propertyName) =>
			PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

		public void EvokePropertyChanged(string propertyName) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public void EvokeSmartPropertyChanging(string propertyName) =>
			SmartPropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

		public void EvokeSmartPropertyChanged(string propertyName) =>
			SmartPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public void EvokePropertyChanging<TValue>(Expression<Func<TValue>> expression) =>
			EvokePropertyChanging(expression.UnboxMemberName());

		public void EvokePropertyChanged<TValue>(Expression<Func<TValue>> expression) =>
			EvokePropertyChanged(expression.UnboxMemberName());

		public TValue Get<TValue>(Expression<Func<TValue>> expression, TValue defaultValue = default) =>
			(TValue) this[expression.UnboxMemberName(), defaultValue];

		public void Set<TValue>(Expression<Func<TValue>> expression, TValue value, bool matching = false)
		{
			var key = expression.UnboxMemberName();
			if (matching && this[key].Is(value)) return;
			EvokePropertyChanging(key);
			this[key] = value;
			EvokePropertyChanged(key);
		}

		#endregion
	}
}