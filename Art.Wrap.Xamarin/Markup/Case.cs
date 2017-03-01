using System;
using System.Windows;
using Aero.Converters.Patterns;
using Xamarin.Forms;

namespace Aero.Markup
{
    [ContentProperty("Value")]
    public class Case : DependencyObject, ICase
    {
        public Type KeyType
        {
            get { return Key as Type; }
            set { Key = value; }
        }

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(object), typeof(Case), new PropertyMetadata(CaseSet.UndefinedObject));

        public object Key
        {
            get { return GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(Case), new PropertyMetadata(CaseSet.UndefinedObject));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }   
}
