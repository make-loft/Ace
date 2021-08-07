// ReSharper disable RedundantUsingDirective
using System;
using System.Globalization;
using System.Windows.Data;

namespace Ace.Markup.Patterns
{
#if XAMARIN
	using Xamarin.Forms.Xaml;
	
	public abstract class AMarkupExtension : IMarkupExtension
	{
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			var targets = (IProvideValueTarget) serviceProvider.GetService(typeof(IProvideValueTarget));
			return Provide(targets.TargetObject, targets.TargetProperty);
		}

		public abstract object Provide(object targetObject, object targetProperty = default);
	}
#else
	public abstract class AMarkupExtension : ABindingExtension
	{
		protected AMarkupExtension() : base(new RelativeSource {Mode = RelativeSourceMode.Self}) =>
			Mode = BindingMode.OneTime;

		public abstract object Provide(object targetObject, object targetProperty = default);

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
			Provide(value);
	}
#endif
}