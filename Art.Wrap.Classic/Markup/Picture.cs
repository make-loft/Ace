using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aero.Markup
{
    public class Picture : Base.BindingExtension
    {
        public Picture()
        {
            Width = Height = 16;
        }

        public ImageSource Key { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Image {Source = Key, Width = Width, Height = Height};
        }
    }
}
