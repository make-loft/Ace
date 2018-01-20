// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Ace.Converters.Patterns;
using Ace.Markup;
using ContentProperty = Xamarin.Forms.ContentPropertyAttribute;

namespace Ace.Converters
{	
	[ContentProperty("Cases")]
	public class SwitchConverter : DependencyObject, ICompositeConverter
	{
		public static readonly DependencyProperty DefaultProperty = DependencyProperty.Register(
			"Default", typeof(object), typeof(SwitchConverter), new PropertyMetadata(Case.UndefinedValue));

		public object Default
		{
			get => GetValue(DefaultProperty);
			set => SetValue(DefaultProperty, value);
		}

		public static readonly DependencyProperty DefaultBackProperty = DependencyProperty.Register(
			"DefaultBack", typeof(object), typeof(SwitchConverter), new PropertyMetadata(default(object)));

		public object DefaultBack
		{
			get => GetValue(DefaultBackProperty);
			set => SetValue(DefaultBackProperty, value);
		}

		public List<ICase<object, object>> Cases { get; } = new List<ICase<object, object>>();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var @case = Cases.FirstOrDefault(c => c.MatchByKey(value));
			var activeValue = @case == null ? Default : @case.Value;
			value = activeValue == Case.UndefinedValue ? value : activeValue;
			return PostConverter == null
				? value
				: PostConverter.Convert(value, targetType, PostConverterParameter, culture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var @case = Cases.FirstOrDefault(c => c.MatchByValue(value));
			var activeValue = @case == null ? DefaultBack : @case.Key;
			value = activeValue == Case.UndefinedValue ? value : activeValue;
			return PostConverter == null
				? value
				: PostConverter.Convert(value, targetType, PostConverterParameter, culture);
		}

		public IValueConverter PostConverter { get; set; }
		public object PostConverterParameter { get; set; }
	}
}