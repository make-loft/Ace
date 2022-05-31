﻿// ReSharper disable RedundantUsingDirective
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

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
	public abstract class AMarkupExtension : MarkupExtension
	{
		public abstract object Provide(object targetObject, object targetProperty = default);

		public object ProvideValue(IProvideValueTarget service) =>
			Provide(service.TargetObject, service.TargetProperty);

		public override object ProvideValue(IServiceProvider provider) =>
			Provide((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget)));
	}
#endif
}