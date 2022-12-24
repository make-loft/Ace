using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Xamarin.Forms;

using Property = Xamarin.Forms.BindableProperty;

namespace Ace.Controls
{
	public abstract class AView<TView> : ContentView
	{
		private readonly static Dictionary<string, Property> NameToProperty = new();

		public static Property Create<TValue>(string name, Type valueType, Type viewType, TValue defaultValue = default) =>
			NameToProperty[name] = Property.Create(name, valueType, viewType, defaultValue);

		public static Property Create<TValue>(Expression<Func<TView, TValue>> func, TValue defaultValue = default) =>
			Property.Create(func.UnboxMemberName(), TypeOf<TValue>.Raw, TypeOf<TView>.Raw, defaultValue);

		public TValue Get<TValue>(Expression<Func<TView, TValue>> func) => Get<TValue>(func.UnboxMemberName());

		public TValue Get<TValue>(string name) => Get<TValue>(NameToProperty[name]);

		public TValue Get<TValue>(Property property) => (TValue)GetValue(property);

		public TValue Set<TValue>(Expression<Func<TView, TValue>> func, TValue value) => Set(func.UnboxMemberName(), value);

		public TValue Set<TValue>(string name, TValue value) => Set(NameToProperty[name], value);

		public TValue Set<TValue>(Property property, TValue value) => value.Use(v => SetValue(property, v));
	}
}
