﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Aero.Markup
{
    public class Smart : Patterns.ABindingExtension
    {
        public Smart()
            : base(null)
        {
            Mode = BindingMode.TwoWay;
            Path = new PropertyPath(SmartObject.SmartPropertyName);
        }

        public Smart(string set)
            : this()
        {
            Set = set;
        }

        public Smart(string key, string defaultValue)
            : this()
        {
            Key = key;
            DefaultValue = defaultValue;
        }

        public string Set
        {
            set { Initialize(value); }
        }

        public string Key { get; set; }
        public object DefaultValue { get; set; }
        public bool Segregate { get; set; }
        public new IValueConverter Converter { get; set; }
        public SmartObject SmartObject { get; private set; }

        public string StoreKey
        {
            get { return Source == null ? null : Source.GetType().Name; }
            set { Source = new Store {Key = value}.Convert(null, null, null, ""); }
        }

        public override object Convert(object value, Type targetType, object parameter, string culture)
        {
            SmartObject = value as SmartObject;
            value = SmartObject == null ? null : SmartObject[Key, DefaultValue, Segregate];
            if (Segregate) value = value.Of<Segregator>().Value;
            return Converter == null ? value : Converter.Convert(value, targetType, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            if (SmartObject == null) return null;
            value = Converter == null ? value : Converter.ConvertBack(value, targetType, parameter, culture);
            if (Segregate)
                SmartObject[Key].Of<Segregator>().Value = value;
            else SmartObject[Key] = value;
            return Segregate ? null : SmartObject;
        }

        private void Initialize(string set = "")
        {
            set = set.Replace("[", "").Replace("]", "");
            var parts = set.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0) Key = parts[0].Trim();
            if (parts.Length > 1) DefaultValue = parts[1].Trim();
            if (parts.Length > 2)
                Segregate = parts[2].Trim().ToLower() == "true" || parts[2].Trim().ToLower() == "segregate";
        }
    }
}