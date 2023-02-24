using System;
using System.Collections.Generic;
using System.Linq.Expressions;
#if XAMARIN
using Xamarin.Forms;

using Property = Xamarin.Forms.BindableProperty;
#else
using View = System.Windows.FrameworkElement;
using Property = System.Windows.DependencyProperty;
#endif

namespace Ace.Controls
{
	public abstract class AView<TView> : ContentView where TView : View
	{
		private readonly static Dictionary<string, Property> NameToProperty = new();

		public static Property Create<TValue>(Expression<Func<TView, TValue>> func, TValue defaultValue = default) =>
			NameToProperty[func.UnboxMemberName()] = Type<TView>.Create(func, defaultValue);

		public TValue Get<TValue>(Expression<Func<TView, TValue>> func) => (TValue)GetValue(NameToProperty[func.UnboxMemberName()]);

		public TValue Get<TValue>(Property property) => (TValue)GetValue(property);

		public TValue Set<TValue>(Expression<Func<TView, TValue>> func, TValue value) => Set(NameToProperty[func.UnboxMemberName()], value);

		public TValue Set<TValue>(Property property, TValue value) => value.Use(v => SetValue(property, v));
	}
}
