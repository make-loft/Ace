using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Security;

namespace System.Windows.Media;

public static class VisualTreeHelper
{
	public static Element GetParent(Element element) => element.Parent;

	public static int GetChildrenCount(Element element) => GetChildrenCount(GetContent(element));

	public static Element GetChild(Element element, int index) => GetChildren(GetContent(element), index);

	private static int GetChildrenCount(object content) => content is IList<View> listedContent
		? listedContent.Count
		: content is null
			? 0
			: 1
		;

	private static Element GetChildren(object content, int index) => content is IList<View> listedContent
		? listedContent[index]
		: content as Element
		;

	private static PropertyInfo GetContentProperty(Type type)
		=> type.GetRuntimeProperty("Children") ?? type.GetRuntimeProperty("Content");

	private static object GetContent(this Element element)
		=> GetContentProperty(element.GetType())?.GetValue(element);
}

public static class BrushBrushExtensions
{
	public static Brush Clone(this Brush brush) => brush switch
	{
		SolidColorBrush b => b.Clone(),
		LinearGradientBrush b => b.Clone(),
		RadialGradientBrush b => b.Clone(),
		Brush b => b,
	};

	public static SolidColorBrush Clone(this SolidColorBrush value) => new(value.Color);
	public static LinearGradientBrush Clone(this LinearGradientBrush value) => new()
	{
		GradientStops = value.GradientStops,
		StartPoint = value.StartPoint,
		EndPoint = value.EndPoint,
	};

	public static RadialGradientBrush Clone(this RadialGradientBrush value) => new()
	{
		GradientStops = value.GradientStops,
		Radius = value.Radius,
	};
}

public sealed class ColorConverter : ComponentModel.TypeConverter
{
	//
	// Сводка:
	//     Определяет, может ли объект быть преобразован из заданного типа в экземпляр System.Windows.Media.Color.
	//
	//
	// Параметры:
	//   td:
	//     Описывает контекстную информацию типа.
	//
	//   t:
	//     Тип источника, для которого определяется возможность преобразования.
	//
	// Возврат:
	//     true, если тип можно преобразовать в System.Windows.Media.Color; в противном
	//     случае — false.
	public override bool CanConvertFrom(ITypeDescriptorContext td, Type t)
	{
		if (t == typeof(string))
		{
			return true;
		}

		return false;
	}

	//
	// Сводка:
	//     Определяет, может ли экземпляр System.Windows.Media.Color быть преобразован в
	//     другой тип.
	//
	// Параметры:
	//   context:
	//     Описывает контекстную информацию типа.
	//
	//   destinationType:
	//     Требуемый тип, для которого System.Windows.Media.Color проверяет возможность
	//     преобразования.
	//
	// Возврат:
	//     Значение true, если System.Windows.Media.Color может быть преобразован в destinationType;
	//     в противном случае — false.
	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (destinationType == typeof(InstanceDescriptor))
		{
			return true;
		}

		return base.CanConvertTo(context, destinationType);
	}

	//
	// Сводка:
	//     Пытается преобразовать строку в System.Windows.Media.Color.
	//
	// Параметры:
	//   value:
	//     Строка, преобразуемая в System.Windows.Media.Color.
	//
	// Возврат:
	//     Объект System.Windows.Media.Color, представляющий преобразованный текст.
	public new static object ConvertFromString(string value) => default;

	//
	// Сводка:
	//     Пытается преобразовать заданный объект в System.Windows.Media.Color.
	//
	// Параметры:
	//   td:
	//     Описывает контекстную информацию типа.
	//
	//   ci:
	//     Региональные особенности, которые следует учитывать при выполнении преобразования.
	//
	//
	//   value:
	//     Преобразуемый объект.
	//
	// Возврат:
	//     Объект System.Windows.Media.Color, созданный в результате преобразования value.
	public override object ConvertFrom(ITypeDescriptorContext td, CultureInfo ci, object value) => default;

	//
	// Сводка:
	//     Пытается преобразовать System.Windows.Media.Color в заданный тип.
	//
	// Параметры:
	//   context:
	//     Описывает контекстную информацию типа.
	//
	//   culture:
	//     Определяет объект System.Globalization.CultureInfo преобразуемого типа.
	//
	//   value:
	//     Преобразуемый объект System.Windows.Media.Color.
	//
	//   destinationType:
	//     Тип, в который преобразуется данный System.Windows.Media.Color.
	//
	// Возврат:
	//     Объект, созданный в результате преобразования данного объекта System.Windows.Media.Color.
	[SecurityCritical]
	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType != null && value is Color)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				MethodInfo method = typeof(Color).GetMethod("FromArgb", new Type[4]
				{
					typeof(byte),
					typeof(byte),
					typeof(byte),
					typeof(byte)
				});
				Color color = (Color)value;
#if MAUI
				return new InstanceDescriptor(method, new object[4] { color.Alpha, color.Red, color.Green, color.Blue });
#else
				return new InstanceDescriptor(method, new object[4] { color.A, color.R, color.G, color.B });
#endif
			}

			if (destinationType == typeof(string))
			{
				return ((Color)value).ToString();
			}
		}

		return base.ConvertTo(context, culture, value, destinationType);
	}

	//
	// Сводка:
	//     Инициализирует новый экземпляр System.Windows.Media.ColorConverter.
	public ColorConverter()
	{
	}
}
