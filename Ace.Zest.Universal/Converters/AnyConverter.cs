﻿using System;
using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Aero.Converters
{
    public class AnyConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty OnAnyProperty = DependencyProperty.Register(
            "OnAny", typeof (object), typeof (AnyConverter), new PropertyMetadata(default(object)));

        public object OnAny
        {
            get { return GetValue(OnAnyProperty); }
            set { SetValue(OnAnyProperty, value); }
        }

        public static readonly DependencyProperty OnNotAnyProperty = DependencyProperty.Register(
            "OnNotAny", typeof (object), typeof (AnyConverter), new PropertyMetadata(default(object)));

        public object OnNotAny
        {
            get { return GetValue(OnNotAnyProperty); }
            set { SetValue(OnNotAnyProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            var enumerable = value as IList;
            return enumerable == null || enumerable.Count == 0 ? OnNotAny : OnAny;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
