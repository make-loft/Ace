using System;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace Aero.Markup
{
    public class Case : DependencyObject
    {
        public class Set : List<Case>
        {
        }

        public Type Type
        {
            get { return Key as Type; }
            set { Key = value; }
        }

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof (object), typeof (Case), new PropertyMetadata(default(object)));

        public object Key
        {
            get { return GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof (object), typeof (Case), new PropertyMetadata(default(object)));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }   
}
