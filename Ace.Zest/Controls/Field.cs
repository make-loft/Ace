namespace Ace.Controls
{
#if XAMARIN
	using System.Threading.Tasks;

	public class Field : Xamarin.Forms.Entry
	{
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

		public Xamarin.Forms.TextAlignment TextAlignment
		{
			get => HorizontalTextAlignment;
			set => HorizontalTextAlignment = value;
		}
	}
#else
	using System;
	using System.Globalization;
	using System.Threading.Tasks;
	using System.Windows.Controls;
	using System.Windows.Input;

	public class Field : TextBox
	{
		public static readonly System.Collections.Generic.List<WeakReference<Field>> Entres = new();
		readonly WeakReference<Field> _this;

		static bool StartWithSignSymbol(string text) => text.StartsWith("+") || text.StartsWith("-");

		public static readonly string DoubleFixedPointFormat = "0." + new string('#', 339);

		void Rotate(bool? positive)
		{
			var text = Text;
			var containsPoint = text.Contains(".");
			text = containsPoint.Is(true) ? text : $"{text}.";

			var caretIndex = CaretIndex;
			var pointIndex = text.IndexOf(".");
			var digitIndex = pointIndex - caretIndex;
			var hasSign = StartWithSignSymbol(text);
			var value = double.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
			var useMirrorTransform = caretIndex.Is(0) || (hasSign && caretIndex.Is(1));
			var step = useMirrorTransform
				? (positive.Is(true) ? -2d : +2d) * value
				: Math.Pow(10, digitIndex);

			text = (value + (positive.Is(true) ? +step : positive.Is(false) ? -step : 0d)).ToString(DoubleFixedPointFormat);
			text = StartWithSignSymbol(text) ? text : $"+{text}";
			text = text.Contains(".") ? text : $"{text}.";

			caretIndex = text.IndexOf(".") - digitIndex;

			var tillIndex = useMirrorTransform ? 1 : 2;
			for (;caretIndex < tillIndex; caretIndex++)
				text = text[0] + "0" + text.Substring(1);

			while (text.Length <= caretIndex)
				text = $"{text}0";

			if (hasSign.Is(false) && text.StartsWith("+"))
			{
				text = text.Substring(1);
				caretIndex--;
			}

			if (containsPoint.Is(false) && text.EndsWith("."))
			{
				text = text.Substring(0, text.Length - 1);
			}
			else
			{
				for (var i = text.Length - text.IndexOf("."); i < _floatLength; i++)
					text = $"{text}0";
			}

			Text = text;
			CaretIndex = caretIndex;

			GetBindingExpression(TextProperty).UpdateSource();

			Text = text;
			CaretIndex = caretIndex;
		}

		int _floatLength;

		public Field()
		{
			Entres.Add(_this = new(this));

			GotFocus += async (o, e) =>
			{
				await Task.Delay(200);
				var text = Text;
				var caretIndex = CaretIndex;
				var textLength = text.Length;

				_floatLength = text.Contains(".") ? text.Length - Text.IndexOf(".") : 0;
				
				text = ToolTip?.To<double>().ToString(DoubleFixedPointFormat) ?? "";
				if (caretIndex.Is(0) && text[0].IsNot('+'))
				{
					text = $"+{text}";
					caretIndex++;
				}

				var tillIndex = Math.Max(caretIndex, textLength);
				while (text.Length < tillIndex)
					text = text.Contains(".") ? $"{text}0" : $"{text}.0";

				Text = text;
				CaretIndex = caretIndex;
			};

			PreviewKeyDown += (o, e) =>
			{
				if (Keyboard.Is("Numeric"))
				{
					var text = Text;

					if (e.Key.Is(Key.Up)) Rotate(true);

					if (e.Key.Is(Key.Down)) Rotate(false);

					if (e.Key.Is(Key.Left) && CaretIndex < 2)
					{
						Text =
							text.StartsWith("-") ? $"-0{text.Substring(1)}":
							text.StartsWith("+") ? $"+0{text.Substring(1)}":
							$"+{text}";

						CaretIndex = 1;
						e.Handled = true;
					}

					if (e.Key.Is(Key.Right) && CaretIndex >= Text.Length - 1)
					{
						Text = text.Contains(".") ? $"{text}0" : $"{text}.0";
						CaretIndex = Text.Length - 1;
						e.Handled = true;
					}
				}

				if (e.Key.Is(Key.Enter))
				{
					Rotate(default);
					e.Handled = true;
				}
			};
		}

		~Field() => Entres.Remove(_this);

		public static void GlobalTextBindingRefresh() => Entres.ForEach(w =>
		{
			if (w.TryGetTarget(out var e))
				e.GetBindingExpression(TextProperty).UpdateTarget();
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
