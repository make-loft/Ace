using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ace.Presentation
{
	public class Grid : Xamarin.Forms.Grid { }
	public class Switch : Xamarin.Forms.Switch { }
	public class Slider : Xamarin.Forms.Slider { }

	public class DataTemplate : Xamarin.Forms.DataTemplate { }
	public class ControlTemplate : Xamarin.Forms.ControlTemplate { }
	public class ResourceDictionary : Xamarin.Forms.ResourceDictionary { }
	public class SolidColorBrush : Xamarin.Forms.SolidColorBrush { }
	public class LinearGradientBrush : Xamarin.Forms.LinearGradientBrush { }
	public class RadialGradientBrush : Xamarin.Forms.RadialGradientBrush { }
	public class GradientStop : Xamarin.Forms.GradientStop { }

	[ContentProperty(nameof(Key))]
	public class StaticResourceExtension : IMarkupExtension
	{
		public string Key { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			try
			{
				return new Xamarin.Forms.Xaml.StaticResourceExtension() { Key = Key }.ProvideValue(serviceProvider);
			}
			catch
			{
				return Application.Current.Resources[Key];
			}
		}
	}

	[ContentProperty(nameof(Key))]
	public class DynamicResourceExtension : IMarkupExtension
	{
		public string Key { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			try
			{
				return new Xamarin.Forms.Xaml.DynamicResourceExtension() { Key = Key }.ProvideValue(serviceProvider);
			}
			catch
			{
				return Application.Current.Resources[Key];
			}
		}
	}

	[ContentProperty(nameof(Name))]
	public class ReferenceExtension : IMarkupExtension
	{
		public string Name { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider) =>
			new Xamarin.Forms.Xaml.ReferenceExtension() { Name = Name }.ProvideValue(serviceProvider);
	}

	[AcceptEmptyServiceProvider]
	[ContentProperty(nameof(Path))]
	public class BindingExtension : IMarkupExtension<BindingBase>
	{
		public string Path { get; set; }
		public BindingMode Mode { get; set; }
		public object Source { get; set; }
		public IValueConverter Converter { get; set; }
		public object ConverterParameter { get; set; }
		public object FallbackValue { get; set; }
		public string StringFormat { get; set; }

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
			this.To<IMarkupExtension<BindingBase>>().ProvideValue(serviceProvider);

		public BindingBase ProvideValue(IServiceProvider serviceProvider = default) => new Binding
		//public BindingBase ProvideValue(IServiceProvider serviceProvider = default) => new Xamarin.Forms.Xaml.BindingExtension
		{
			Path = Path,
			Mode = Mode,
			Source = Source,
			Converter = Converter,
			ConverterParameter = ConverterParameter,
			FallbackValue = FallbackValue,
			StringFormat = StringFormat,
		}
		//.To<IMarkupExtension<BindingBase>>().ProvideValue(serviceProvider)
		;
	}

	[AcceptEmptyServiceProvider]
	[ContentProperty(nameof(Path))]
	public class TemplateBindingExtension : IMarkupExtension<BindingBase>
	{
		public string Path { get; set; }
		public BindingMode Mode { get; set; }
		public object Source { get; set; }
		public IValueConverter Converter { get; set; }
		public object ConverterParameter { get; set; }
		public object FallbackValue { get; set; }
		public string StringFormat { get; set; }

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
			this.To<IMarkupExtension<BindingBase>>().ProvideValue(serviceProvider);

		public BindingBase ProvideValue(IServiceProvider serviceProvider = default) => new Xamarin.Forms.Xaml.TemplateBindingExtension
		{
			Path = Path,
			Mode = Mode,
			Converter = Converter,
			ConverterParameter = ConverterParameter,
			StringFormat = StringFormat,
		}
		.To<IMarkupExtension<BindingBase>>().ProvideValue(serviceProvider);
	}
}