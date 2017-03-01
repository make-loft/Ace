using System;
using Xamarin.Forms.Xaml;

namespace Aero.Markup.Patterns
{
    public abstract class AMarkupExtension : IMarkupExtension
    {
        public IProvideValueTarget Target { get; private set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            Target = (IProvideValueTarget) serviceProvider.GetService(typeof (IProvideValueTarget));
            return Convert();
        }

        public abstract object Convert();
    }
}