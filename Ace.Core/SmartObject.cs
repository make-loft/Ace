﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Ace.Replication;

namespace Ace
{
	[DataContract]
	public class SmartObject : INotifyPropertyChanging, INotifyPropertyChanged
	{
		public static object UndefinedValue = null;
		private Dictionary<string, object> _smartContainer;
		protected Dictionary<string, object> SmartContainer =>
			_smartContainer ?? (_smartContainer = new Dictionary<string, object>());

		public const string SmartPropertyName = "Smart";

		public SmartObject Smart
		{
			get => this;
			set => RaisePropertyChanged(Equals(value) ? SmartPropertyName : null);
		}

		public object this[string key]
		{
			get => SmartContainer.TryGetValue(key, out var value) ? value : UndefinedValue;
			set
			{
				RaisePropertyChanging(SmartPropertyName);
				SmartContainer[key] = value;
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
			get => GetSmartProperties(SmartContainer, GetType())?.ToDictionary(p => p.Key, p => p.Value);
			set => value?.ForEach(pair => this[pair.Key] = pair.Value);
		}

		protected static IEnumerable<KeyValuePair<string, object>> GetSmartProperties(
			Dictionary<string, object> smartContainer, Type smartType, BindingFlags bindingFlags = Member.DefaultFlags) =>
			smartContainer?.Where(p => !smartType.EnumerateMember(p.Key, bindingFlags).Any(m => m is PropertyInfo));

		#region Notification Core

		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanging(string propertyName) =>
			PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

		public void RaisePropertyChanged(string propertyName) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public void RaisePropertyChanging<TValue>(Expression<Func<TValue>> expression) =>
			RaisePropertyChanging(expression.UnboxMemberName());

		public void RaisePropertyChanged<TValue>(Expression<Func<TValue>> expression) =>
			RaisePropertyChanged(expression.UnboxMemberName());

		public TValue Get<TValue>(Expression<Func<TValue>> expression, TValue defaultValue = default(TValue)) =>
			(TValue)this[expression.UnboxMemberName(), defaultValue];

		public void Set<TValue>(Expression<Func<TValue>> expression, TValue value, bool checkEquals = false)
		{
			if (checkEquals && Equals(Get(expression), value)) return;
			var propertyName = expression.UnboxMemberName();
			RaisePropertyChanging(propertyName);
			this[propertyName] = value;
			RaisePropertyChanged(propertyName);
		}

		#endregion
	}
}