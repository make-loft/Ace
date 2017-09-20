// ReSharper disable RedundantUsingDirective
using System;
using System.Windows;
using System.Windows.Markup;
using Ace.Converters.Patterns;
using ContentProperty = Xamarin.Forms.ContentPropertyAttribute;

namespace Ace.Markup
{
    [ContentProperty("Value")]
    public class Case : DependencyObject, ICase
    {
        public Type KeyType
        {
            get => Key as Type;
            set => Key = value;
        }

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(object), typeof(Case),
                new PropertyMetadata(CaseSet.UndefinedObject));

        public object Key
        {
            get => GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(Case),
                new PropertyMetadata(CaseSet.UndefinedObject));

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}