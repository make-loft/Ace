using Ace.Extensions;

using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ace.Presentation
{
	public class Grid : Xamarin.Forms.Grid { }
	public class Switch : Xamarin.Forms.Switch { }
	public class Slider : Xamarin.Forms.Slider { }

	//public class DataTemplate : Xamarin.Forms.DataTemplate { }
	//public class ControlTemplate : Xamarin.Forms.ControlTemplate { }
	public class ResourceDictionary : Xamarin.Forms.ResourceDictionary { }
	public class SolidColorBrush : Xamarin.Forms.SolidColorBrush
	{
		public double Opacity { get; set; }
	}
	public class LinearGradientBrush : Xamarin.Forms.LinearGradientBrush
	{
		public double Opacity { get; set; }
		public new GradientStopCollection GradientStops
		{
			set => value.ForEach(base.GradientStops.Add);
		}
	}

	public class RadialGradientBrush : Xamarin.Forms.RadialGradientBrush
	{
		public double RadiusX { get => Radius; set => Radius = value; }
		public double RadiusY { get => Radius; set => Radius = value; }
		public double Opacity { get; set; }
		public new GradientStopCollection GradientStops
		{
			set => value.ForEach(base.GradientStops.Add);
		}
	}
	public class GradientStopCollection : List<GradientStop> { } //Xamarin.Forms.GradientStopCollection { }
	public class GradientStop : Xamarin.Forms.GradientStop { }

	public class GeometryDrawing
	{
		public Brush Brush { get; set; }
		public string Geometry { get; set; }
	}
	public class DrawingBrush : Brush
	{
		public override bool IsEmpty => default;

		public string TileMode { get; set; }
		public string Viewport { get; set; }
		public string ViewportUnits { get; set; }
		public double Opacity { get; set; }
		public GeometryDrawing Drawing { get; set; }
	}


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