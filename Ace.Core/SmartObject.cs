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
		protected Dictionary<string, object> SmartContainer => _smartContainer.OrNew(ref _smartContainer);

		public const string SmartPropertyName = "Smart";

		public SmartObject Smart
		{
			get => this;
			set => RaisePropertyChanged(this.Is(value) ? SmartPropertyName : throw new Exception("Wrong context"));
		}

		public object this[string key]
		{
			get => SmartContainer.TryGetValue(key, out var value) ? value : UndefinedSmartValue;
			set
			{
				RaiseSmartPropertyChanging(key);
				RaisePropertyChanging(SmartPropertyName);
				SmartContainer[key] = value;
				RaiseSmartPropertyChanged(key);
				RaisePropertyChanged(SmartPropertyName);
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

		public void RaisePropertyChanging(string propertyName) =>
			PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

		public void RaisePropertyChanged(string propertyName) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public void RaiseSmartPropertyChanging(string propertyName) =>
			SmartPropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

		public void RaiseSmartPropertyChanged(string propertyName) =>
			SmartPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public void RaisePropertyChanging<TValue>(Expression<Func<TValue>> expression) =>
			RaisePropertyChanging(expression.UnboxMemberName());

		public void RaisePropertyChanged<TValue>(Expression<Func<TValue>> expression) =>
			RaisePropertyChanged(expression.UnboxMemberName());

		public TValue Get<TValue>(Expression<Func<TValue>> expression, TValue defaultValue = default(TValue)) =>
			(TValue) this[expression.UnboxMemberName(), defaultValue];

		public void Set<TValue>(Expression<Func<TValue>> expression, TValue value, bool matching = false)
		{
			var propertyName = expression.UnboxMemberName();
			if (matching && this[propertyName].Is(value)) return;
			RaisePropertyChanging(propertyName);
			this[propertyName] = value;
			RaisePropertyChanged(propertyName);
		}

		#endregion
	}
}