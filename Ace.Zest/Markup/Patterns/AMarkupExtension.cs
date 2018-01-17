// ReSharper disable RedundantUsingDirective
using System;
using System.Globalization;
using System.Windows.Data;
using Xamarin.Forms.Xaml;

namespace Ace.Markup.Patterns
{
#if XAMARIN
	public abstract class AMarkupExtension : IMarkupExtension
	{
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			var targets = (IProvideValueTarget) serviceProvider.GetService(typeof(IProvideValueTarget));
			return Provide(targets.TargetObject, targets.TargetProperty);
		}

		public abstract object Provide(object targetObject, object targetProperty = null);
	}
#else
	public abstract class AMarkupExtension : ABindingExtension
	{
		protected AMarkupExtension() : base(new RelativeSource {Mode = RelativeSourceMode.Self}) =>
			Mode = BindingMode.OneTime;

		public abstract object Provide(object targetObject, object targetProperty = null);

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
			Provide(value);
	}
#endif
}