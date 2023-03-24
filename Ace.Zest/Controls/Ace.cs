#if XAMARIN
using Xamarin.Forms;
using View = Xamarin.Forms.View;
using Property = Xamarin.Forms.BindableProperty;
using Panel = Xamarin.Forms.Layout<Xamarin.Forms.View>;
#else
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using View = System.Windows.FrameworkElement;
using Property = System.Windows.DependencyProperty;
using BindableObject = System.Windows.DependencyObject;
#endif
using System.Windows;
using System.Linq.Expressions;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Specialized;

namespace Ace.Controls
{
	public struct ChangeArgs<TSender, TValue>
	{
		public ChangeArgs(TSender sender, DependencyPropertyChangedEventArgs args)
			: this(sender, (TValue)args.OldValue, (TValue)args.NewValue) { }

		public ChangeArgs(TSender sender, TValue oldValue, TValue newValue)
		{
			Sender = sender;

			OldValue = oldValue;
			NewValue = newValue;
		}

		public TSender Sender { get; }

		public TValue OldValue { get; }
		public TValue NewValue { get; }
	}

	public static class New
	{
		public static View CreateView(this DataTemplate template, object context) =>
			template.LoadContent().To(out View c).With(c.SetContext(context));

		public static View CreateView(this DataTemplate template) => (View)template.LoadContent();

#if XAMARIN
		public static View LoadContent(this DataTemplate template) => (View)template.CreateContent();

		public static object GetContext(this View view) => view.BindingContext;
		public static object SetContext(this View view, object value) => view.BindingContext = value;

		public static object SetLengthX(this View view, double value) => view.WidthRequest = value;
		public static object SetLengthY(this View view, double value) => view.HeightRequest = value;

		public static object SetAligmentX(this View view, AligmentOptions value) => view.HorizontalOptions = value switch
		{
			AligmentOptions.Default => LayoutOptions.Center,
			AligmentOptions.Center => LayoutOptions.Center,
			AligmentOptions.From => LayoutOptions.Start,
			AligmentOptions.Till => LayoutOptions.End,
			AligmentOptions.Stretch => LayoutOptions.Fill,
			_ => throw new NotImplementedException(),
		};

		public static object SetAligmentY(this View view, AligmentOptions value) => view.VerticalOptions = value switch
		{
			AligmentOptions.Default => LayoutOptions.Center,
			AligmentOptions.Center => LayoutOptions.Center,
			AligmentOptions.From => LayoutOptions.Start,
			AligmentOptions.Till => LayoutOptions.End,
			AligmentOptions.Stretch => LayoutOptions.Fill,
			_ => throw new NotImplementedException(),
		};

		public static void ContextChanged<TView>(this TView element, Action<ChangeArgs<TView, object>> onContextChanged) where TView : View =>
			element.BindingContextChanged += (o, e) => onContextChanged(new(element, default, element.BindingContext));
#else

		public static void ContextChanged<TView>(this TView element, Action<ChangeArgs<TView, object>> onContextChanged) where TView : FrameworkElement =>
			element.DataContextChanged += (o, e) => onContextChanged(new(element, e));

		public static object GetContext(this View view) => view.DataContext;
		public static object SetContext(this View view, object value) => view.DataContext = value;

		public static object SetLengthX(this View view, double value) => view.Width = value;
		public static object SetLengthY(this View view, double value) => view.Height = value;

		public static object SetAligmentX(this View view, AligmentOptions value) => view.HorizontalAlignment = value switch
		{
			AligmentOptions.Default => HorizontalAlignment.Center,
			AligmentOptions.Center => HorizontalAlignment.Center,
			AligmentOptions.From => HorizontalAlignment.Left,
			AligmentOptions.Till => HorizontalAlignment.Right,
			AligmentOptions.Stretch => HorizontalAlignment.Stretch,
			_ => throw new NotImplementedException(),
		};

		public static object SetAligmentY(this View view, AligmentOptions value) => view.VerticalAlignment = value switch
		{
			AligmentOptions.Default => VerticalAlignment.Center,
			AligmentOptions.Center => VerticalAlignment.Center,
			AligmentOptions.From => VerticalAlignment.Top,
			AligmentOptions.Till => VerticalAlignment.Bottom,
			AligmentOptions.Stretch => VerticalAlignment.Stretch,
			_ => throw new NotImplementedException(),
		};
#endif
	}

	public static class Type<TOwner>
	{
#if XAMARIN
		public static Property Create<TProperty>(Expression<Func<TOwner, TProperty>> func) =>
			Property.Create(func.UnboxMemberName(), TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw);

		public static Property Create<TProperty>(Expression<Func<TOwner, TProperty>> func, TProperty defaultValue) =>
			Property.Create(func.UnboxMemberName(), TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw, defaultValue);

		public static Property Create<TProperty>(Expression<Func<TOwner, TProperty>> func, Action<ChangeArgs<TOwner, TProperty>> changed, TProperty defaultValue = default) =>
			Property.Create(func.UnboxMemberName(), TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw, defaultValue,
				propertyChanged: (s, o, n) => changed(new((TOwner)(object)s, (TProperty)o, (TProperty)n)));

		public static Property Attach<TProperty>(string name) =>
			Property.CreateAttached(name, TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw, default(TProperty));
		public static Property Attach<TProperty>(string name, TProperty defaultValue, Action<ChangeArgs<View, TProperty>> changed = default) =>
			Property.CreateAttached(name, TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw, defaultValue,
				propertyChanged: (s, o, n) => changed(new((View)s, (TProperty)o, (TProperty)n)));
		public static Property Attach<TProperty>(string name, Action<ChangeArgs<View, TProperty>> changed) =>
			Property.CreateAttached(name, TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw, default(TProperty),
				propertyChanged: (s, o, n) => changed(new((View)s, (TProperty)o, (TProperty)n)));

		public static Property Attach<TView, TProperty>(string name, TProperty defaultValue, Action<ChangeArgs<TView, TProperty>> changed = default)
			where TView : View =>
			Property.CreateAttached(name, TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw, defaultValue,
				propertyChanged: (s, o, n) => changed(new((TView)s, (TProperty)o, (TProperty)n)));
		public static Property Attach<TView, TProperty>(string name, Action<ChangeArgs<TView, TProperty>> changed)
			where TView : View =>
			Property.CreateAttached(name, TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw, default(TProperty),
				propertyChanged: (s, o, n) => changed(new((TView)s, (TProperty)o, (TProperty)n)));
#else
		public static Property Create<TProperty>(Expression<Func<TOwner, TProperty>> func) =>
			Property.Register(func.UnboxMemberName(), TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw);

		public static Property Create<TProperty>(Expression<Func<TOwner, TProperty>> func, TProperty defaultValue) =>
			Property.Register(func.UnboxMemberName(), TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw, new(defaultValue));

		public static Property Create<TProperty>(Expression<Func<TOwner, TProperty>> func, Action<ChangeArgs<TOwner, TProperty>> changed, TProperty defaultValue = default) =>
			Property.Register(func.UnboxMemberName(), TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw, new(defaultValue, (s, args) => changed(new((TOwner)(object)s, args))));

		public static Property Attach<TProperty>(string name) =>
			Property.RegisterAttached(name, TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw);
		public static Property Attach<TProperty>(string name, TProperty defaultValue, Action<ChangeArgs<View, TProperty>> changed = default) =>
			Property.RegisterAttached(name, TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw, new(defaultValue, (s, args) => changed(new((View)s, args))));
		public static Property Attach<TProperty>(string name, Action<ChangeArgs<View, TProperty>> changed) =>
			Property.RegisterAttached(name, TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw, new((s, args) => changed(new((View)s, args))));

		public static Property Attach<TView, TProperty>(string name, TProperty defaultValue, Action<ChangeArgs<TView, TProperty>> changed = default)
			where TView : View =>
			Property.RegisterAttached(name, TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw, new(defaultValue, (s, args) => changed(new((TView)s, args))));
		public static Property Attach<TView, TProperty>(string name, Action<ChangeArgs<TView, TProperty>> changed)
			where TView : View =>
			Property.RegisterAttached(name, TypeOf<TProperty>.Raw, TypeOf<TOwner>.Raw, new((s, args) => changed(new((TView)s, args))));
#endif
	}

	public class Ext
	{
		public static Property ToolTipProperty = Type<Ext>.Attach<object>("ToolTip");

		public static void SetToolTip(BindableObject bindable, object value) => bindable.SetValue(ToolTipProperty, value);
		public static object GetToolTip(BindableObject bindable) => bindable.GetValue(ToolTipProperty);
	}

	public enum AligmentOptions { Default, Center, From, Till, Stretch };

	public class Alignment
	{
		public static Property YProperty = Type<Alignment>.Attach("Y", AligmentOptions.Default, args => args.Sender.SetAligmentY(args.NewValue));
		public static Property XProperty = Type<Alignment>.Attach("X", AligmentOptions.Default, args => args.Sender.SetAligmentX(args.NewValue));

		public static void SetY(BindableObject bindable, AligmentOptions value) => bindable.SetValue(YProperty, value);
		public static void SetX(BindableObject bindable, AligmentOptions value) => bindable.SetValue(XProperty, value);
		public static object GetY(BindableObject bindable) => bindable.GetValue(YProperty);
		public static object GetX(BindableObject bindable) => bindable.GetValue(XProperty);
	}

	public class Length
	{
		public static Property XProperty = Type<Length>.Attach("X", 0d, args => args.Sender.SetLengthX(args.NewValue));
		public static Property YProperty = Type<Length>.Attach("Y", 0d, args => args.Sender.SetLengthY(args.NewValue));

		public static void SetX(BindableObject bindable, double value) => bindable.SetValue(XProperty, value);
		public static void SetY(BindableObject bindable, double value) => bindable.SetValue(YProperty, value);
		public static double GetX(BindableObject bindable) => (double)bindable.GetValue(XProperty);
		public static double GetY(BindableObject bindable) => (double)bindable.GetValue(YProperty);
	}

	public class Data
	{
		public static Property ContextProperty = Type<Data>.Attach("Context", default(object), args => args.Sender.SetContext(args.NewValue));

		public static void SetContext(BindableObject bindable, object value) => bindable.SetValue(ContextProperty, value);
		public static object GetContext(BindableObject bindable) => bindable.GetValue(ContextProperty);
	}

	public class Children
	{
		public static Property ItemsSourceProperty
			= Type<Children>.Attach<Panel, IEnumerable>("ItemsSource", args =>
			{
				// todo: weak subscription
				void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => UpdateContent(args.Sender);

				if (args.OldValue.Is(out INotifyCollectionChanged oldCollection))
				{
					oldCollection.CollectionChanged -= OnCollectionChanged;
				}

				if (args.NewValue.Is(out INotifyCollectionChanged newCollection))
				{
					newCollection.CollectionChanged += OnCollectionChanged;
				}

				UpdateContent(args.Sender);
			});

		public static Property ItemTemplateProperty
			= Type<Children>.Attach<Panel, DataTemplate>("ItemTemplate", args => UpdateContent(args.Sender));

		static void UpdateContent(Panel panel)
		{
			var itemTemplate = GetItemTemplate(panel);
			var itemsSource = GetItemsSource(panel);
			var children = panel.Children;
			children.Clear();

			if (itemTemplate.IsNot() || itemsSource.IsNot())
				return;

			itemsSource.Cast<object>().Select(itemTemplate.CreateView).ForEach(children.Add);
		}

		public static void SetItemsSource(Panel b, IEnumerable value) => b.SetValue(ItemsSourceProperty, value);
		public static IEnumerable GetItemsSource(Panel b) => (IEnumerable)b.GetValue(ItemsSourceProperty);
		public static void SetItemTemplate(Panel b, DataTemplate value) => b.SetValue(ItemTemplateProperty, value);
		public static DataTemplate GetItemTemplate(Panel b) => (DataTemplate)b.GetValue(ItemTemplateProperty);
	}

	//public class Binding : System.Windows.Data.Binding { }
	//public class TemplateBindingExtension : System.Windows.TemplateBindingExtension { }
	//public class StaticResourceExtension : System.Windows.StaticResourceExtension { }
	//public class DynamicResourceExtension : System.Windows.DynamicResourceExtension { }
	//public class ControlTemplate : System.Windows.Controls.ControlTemplate { }
	//public class DataTemplate : System.Windows.DataTemplate { }

#if XAMARIN
	//public static class RowDefinition
	//{
	//	public static readonly Property MinWidthProperty = Type<ColumnDefinition>.Attach("MinWidth", double.NegativeInfinity);
	//	public static readonly Property MaxWidthProperty = Type<ColumnDefinition>.Attach("MaxWidth", double.PositiveInfinity);
	//}

	//public static class ColumnDefinition
	//{
	//	public static readonly Property MinWidthProperty = Type<ColumnDefinition>.Attach("MinWidth", double.NegativeInfinity);
	//	public static readonly Property MaxWidthProperty = Type<ColumnDefinition>.Attach("MaxWidth", double.PositiveInfinity);
	//}

	public class ContentView : Xamarin.Forms.ContentView
	{
		public ControlTemplate Template
		{
			get => ControlTemplate;
			set => ControlTemplate = value;
		}
	}

	public class Title : Label
	{
		public TextAlignment TextAlignment
		{
			get => HorizontalTextAlignment;
			set => HorizontalTextAlignment = value;
		}
	}

	public class Picker : Xamarin.Forms.Picker { }
	public class Button : Xamarin.Forms.Button
	{
		public static Property ContentProperty = Type<Button>.Create(b => b.Content,
			args => args.Sender.Text = args.NewValue.To<string>());
		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}
	}

	public class Border : Frame { }

	public class GridSplitter : Grid { }
	public class RackSplitter : GridSplitter { }
#else

	public enum Orientation { Both, Vertical, Horizontal }
	public class Scroll : ScrollViewer
	{
		public Orientation Orientation { get; set; }
	}

	//public class ResourceDictionary : System.Windows.ResourceDictionary { }

	public class ContentPresenter : System.Windows.Controls.ContentPresenter { }
	public class ContentView : ContentControl { }
	public class Stack : StackPanel
	{
		public double Spacing { get; set; }
	}

	public class RackSplitter : GridSplitter { }
	//public class Frame : Border { }
	//public class Button : System.Windows.Controls.Button { }
	public class Title : TextBlock
	{
		public string FontAttributes { get; set; }
	}
	public class Picker : ComboBox
	{
		public Binding ItemDisplayBinding
		{
			set => DisplayMemberPath = value.Path?.Path;
		}
	}

	public class Grip : Slider
	{
		public static Property FromProperty = Type<Grip>.Create(g => g.From, args => args.Sender.Minimum = args.NewValue);
		public double From
		{
			get => GetValue(FromProperty).To<double>();
			set => SetValue(FromProperty, value);
		}

		public static Property TillProperty = Type<Grip>.Create(g => g.Till, args => args.Sender.Maximum = args.NewValue);
		public double Till
		{
			get => GetValue(TillProperty).To<double>();
			set => SetValue(TillProperty, value);
		}

		public Grip()
		{
			PreviewKeyDown += (o, e) =>
			{
				if (Value == Minimum)
					if (e.Key is Key.Left || e.Key is Key.Down)
						Value = Maximum;
				if (Value == Maximum)
					if (e.Key is Key.Right || e.Key is Key.Up)
						Value = Minimum;
			};

			MouseWheel += (o, e) =>
			{
				var delta = (Maximum - Minimum) / 256;
				Value += e.Delta < 0 ? +delta : e.Delta > 0 ? -delta : 0;
			};
		}
	}

	public class ItemsView : ItemsControl
	{
		public ItemsView()
		{
			DataContextChanged += (o, e) =>
			{
				foreach (var item in Items.OfType<FrameworkElement>())
					item.DataContext = DataContext;
			};
		}

		public static readonly DependencyProperty BindingContextProperty =
			DependencyProperty.Register(nameof(BindingContext), typeof(object), typeof(ItemsView), new PropertyMetadata((o, e) =>
			{
				if (o is ItemsView control) control.SetValue(DataContextProperty, e.NewValue);
			}));

		public object BindingContext
		{
			get => GetValue(BindingContextProperty);
			set => SetValue(BindingContextProperty, value);
		}

		protected override DependencyObject GetContainerForItemOverride() => new ContentControl();

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			if (item is FrameworkElement e && e.DataContext is null) e.DataContext = DataContext;
			return false; // wrap always
		}
	}

	//public class SolidColorBrush : System.Windows.Media.SolidColorBrush { }
	//public class LinearGradientBrush : System.Windows.Media.LinearGradientBrush { }

	//public class Expander : System.Windows.Controls.Expander { }

	//public class Popup : System.Windows.Controls.Primitives.Popup { }

	//public class GroupBox : System.Windows.Controls.GroupBox { }

	//public class Style : System.Windows.Style { }

	//public class Setter : System.Windows.Setter { }
}

namespace Xamarin.Forms
{

#endif
}