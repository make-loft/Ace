using Xamarin.Forms;

namespace Ace.Controls
{
	public class Data
	{
		public static BindableProperty ContextProperty = BindableProperty.CreateAttached("Context", typeof(object), typeof(BindableObject), default,
			propertyChanged: (s, o, n) => s.BindingContext = n is Markup.Patterns.AMarkupExtension ext ? ext.Provide(ContextProperty) : n);

		public static void SetContext(BindableObject bindable, object value) => bindable.SetValue(ContextProperty, value);
		public static object GetContext(BindableObject bindable) => bindable.GetValue(ContextProperty);
	}

	public static class Ext
	{
		public static BindableProperty ToolTipProperty = BindableProperty.CreateAttached("ToolTip", typeof(object), typeof(BindableObject), default);

		public static void SetToolTip(BindableObject bindable, object value) => bindable.SetValue(ToolTipProperty, value);
		public static object GetToolTip(BindableObject bindable) => bindable.GetValue(ToolTipProperty);
	}

	public enum AligmentOptions { Default, Center, From, Till, Stretch };

	public static class Alignment
	{
		public static BindableProperty YProperty = BindableProperty.CreateAttached("Y", typeof(AligmentOptions), typeof(Alignment), default(AligmentOptions),
			propertyChanged: (s, o, n) =>
			{
				s.To<View>().VerticalOptions = n.To<AligmentOptions>().ToLayoutOptions();
			});

		public static BindableProperty XProperty = BindableProperty.CreateAttached("X", typeof(AligmentOptions), typeof(Alignment), default(AligmentOptions),
			propertyChanged: (s, o, n) =>
			{
				s.To<View>().HorizontalOptions = n.To<AligmentOptions>().ToLayoutOptions();
			});

		public static void SetY(BindableObject bindable, AligmentOptions value) => bindable.SetValue(YProperty, value);
		public static void SetX(BindableObject bindable, AligmentOptions value) => bindable.SetValue(XProperty, value);
		public static object GetY(BindableObject bindable) => bindable.GetValue(YProperty);
		public static object GetX(BindableObject bindable) => bindable.GetValue(XProperty);

		public static LayoutOptions ToLayoutOptions(this AligmentOptions aligmentOptions) => aligmentOptions switch
		{
			AligmentOptions.Default => LayoutOptions.Center,
			AligmentOptions.Center => LayoutOptions.Center,
			AligmentOptions.From => LayoutOptions.Start,
			AligmentOptions.Till => LayoutOptions.End,
			AligmentOptions.Stretch => LayoutOptions.Fill,
			_ => throw new System.NotImplementedException(),
		};
	}

	public static class Size
	{
		public static BindableProperty XProperty = BindableProperty.CreateAttached("X", typeof(double), typeof(Size), 0d,
			propertyChanged: (s, o, n) =>
			{
				s.To<View>().WidthRequest = (double)n;
			});
		public static BindableProperty YProperty = BindableProperty.CreateAttached("Y", typeof(double), typeof(Size), 0d,
			propertyChanged: (s, o, n) =>
			{
				s.To<View>().HeightRequest = (double)n;
			});

		public static void SetX(BindableObject bindable, double value) => bindable.SetValue(XProperty, value);
		public static void SetY(BindableObject bindable, double value) => bindable.SetValue(YProperty, value);
		public static double GetX(BindableObject bindable) => (double)bindable.GetValue(XProperty);
		public static double GetY(BindableObject bindable) => (double)bindable.GetValue(YProperty);

	}
}
