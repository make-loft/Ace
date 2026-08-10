#if XAMARIN

namespace Ace.Presentation;

public class Grid : Xamarin.Forms.Grid { }
public class Switch : Xamarin.Forms.Switch { }
public class Slider : Xamarin.Forms.Slider { }
public class Picker : Xamarin.Forms.Picker { }
public class Button : Xamarin.Forms.Button { }

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

	public object ProvideValue(IServiceProvider serviceProvider)
		=> new Xamarin.Forms.Xaml.ReferenceExtension() { Name = Name }.ProvideValue(serviceProvider);
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

	object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		=> this.To<IMarkupExtension<BindingBase>>().ProvideValue(serviceProvider);

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

	object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		=> this.To<IMarkupExtension<BindingBase>>().ProvideValue(serviceProvider);

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

#endif
#if MAUI
namespace Ace.Presentation;

public class Grid : Microsoft.Maui.Controls.Grid { }
public class Switch : Microsoft.Maui.Controls.Switch { }
public class Slider : Microsoft.Maui.Controls.Slider { }
public class Picker : Microsoft.Maui.Controls.Picker { }
public class Button : Microsoft.Maui.Controls.Button { }

//public class DataTemplate : Xamarin.Forms.DataTemplate { }
//public class ControlTemplate : Xamarin.Forms.ControlTemplate { }
public class ResourceDictionary : Microsoft.Maui.Controls.ResourceDictionary { }
public class SolidColorBrush : Microsoft.Maui.Controls.SolidColorBrush
{
	public double Opacity { get; set; }
}
public class LinearGradientBrush : Microsoft.Maui.Controls.LinearGradientBrush
{
	public double Opacity { get; set; }
	public new GradientStopCollection GradientStops
	{
		set => value.ForEach(base.GradientStops.Add);
	}
}

public class RadialGradientBrush : Microsoft.Maui.Controls.RadialGradientBrush
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
public class GradientStop : Microsoft.Maui.Controls.GradientStop { }

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
			return new Microsoft.Maui.Controls.Xaml.StaticResourceExtension() { Key = Key }.ProvideValue(serviceProvider);
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
			return new Microsoft.Maui.Controls.Xaml.DynamicResourceExtension() { Key = Key }.ProvideValue(serviceProvider);
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

	public object ProvideValue(IServiceProvider serviceProvider)
		=> new Microsoft.Maui.Controls.Xaml.ReferenceExtension() { Name = Name }.ProvideValue(serviceProvider);
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

	object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		=> this.To<IMarkupExtension<BindingBase>>().ProvideValue(serviceProvider);

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

	object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		=> this.To<IMarkupExtension<BindingBase>>().ProvideValue(serviceProvider);

	public BindingBase ProvideValue(IServiceProvider serviceProvider = default) => new Microsoft.Maui.Controls.Xaml.TemplateBindingExtension
	{
		Path = Path,
		Mode = Mode,
		Converter = Converter,
		ConverterParameter = ConverterParameter,
		StringFormat = StringFormat,
	}
	.To<IMarkupExtension<BindingBase>>().ProvideValue(serviceProvider);
}
#endif