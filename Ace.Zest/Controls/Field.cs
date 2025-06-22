namespace Ace.Controls
{
	using System;
#if XAMARIN
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Input;

	using Xamarin.Forms;

	public class Field : Entry
	{
		public int CaretIndex { get; set; }
		public bool IsKeyboardFocused { get; set; }
		public bool IsReadOnlyCaretVisible { get; set; }
		public Thickness BorderThickness { get; set; }

		public event Action<object, RoutedEventArgs> SelectionChanged;
		public event Action<object, RoutedEventArgs> GotFocus;
		public event Action<object, KeyEventArgs> PreviewKeyDown;
		public event Action<object, MouseWheelEventArgs> PreviewMouseWheel;

		public Field()
		{
			bool isCaptured = false;

			TextChanged += async (o, e) =>
			{
				if (isCaptured || IsEnabled.Not())
					return;

				IsEnabled = false;
				await Task.Delay(2000);
				IsEnabled = true;
			};

			Focused += (o, e) => isCaptured = true;

			Unfocused += async (o, e) =>
			{
				await Task.Delay(1000);
				isCaptured = false;
			};
		}

		public TextAlignment TextAlignment
		{
			get => HorizontalTextAlignment;
			set => HorizontalTextAlignment = value;
		}
	}
#else
	using System;
	using System.Windows.Controls;

	public class Field : TextBox
	{
		public static readonly System.Collections.Generic.List<WeakReference<Field>> Entres = new();
		readonly WeakReference<Field> _this;

		public Field()
		{
			Entres.Add(_this = new(this));
		}

		~Field() => Entres.Remove(_this);

		public static void GlobalTextBindingRefresh() => Entres.ForEach(w =>
		{
			if (w.TryGetTarget(out var e))
				e.GetBindingExpression(TextProperty)?.UpdateTarget();
		});

		public string Keyboard { get; set; }
		public System.Windows.TextAlignment HorizontalTextAligment
		{
			get => TextAlignment;
			set => TextAlignment = value;
		}
	}
#endif
}
