#if XAMARIN
using Xamarin.Forms;
using Property = Xamarin.Forms.BindableProperty;
#else
using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Property = System.Windows.DependencyProperty;

#endif

namespace Ace.Controls
{
	public partial class NumericField
	{
		public NumericField() => InitializeComponent();

		public static Property FromProperty = Type<NumericField>.Create(v => v.From, double.NegativeInfinity);
		public static Property TillProperty = Type<NumericField>.Create(v => v.Till, double.PositiveInfinity);
		public static Property ValueProperty = Type<NumericField>.Create(v => v.Value);
		public static Property FormatProperty = Type<NumericField>.Create(v => v.Format, changed: args =>
			args.Sender.ValueField.GetBindingExpression(Field.TextProperty).UpdateTarget());
		public static Property IsReadOnlyProperty = Type<NumericField>.Create(v => v.IsReadOnly, changed: args =>
			args.Sender.ValueField.IsReadOnly = args.NewValue);

		public double From { get => this.Get(0d); set => this.Set(value); }
		public double Till { get => this.Get(0d); set => this.Set(value); }
		public double Value { get => this.Get(0d); set => this.Set(value); }
		public string Format { get => this.Get(""); set => this.Set(value); }
		public bool IsReadOnly { get => this.Get(false); set => this.Set(value); }

		static bool StartWithSignSymbol(string text) => text.StartsWith("+") || text.StartsWith("-");

		public static readonly string DoubleFixedPointFormat = "0." + new string('#', 339);

		bool TryRotate(bool? positive)
		{
			if (IsReadOnly)
				return false;

			var text = ValueField.Text;
			var containsPoint = text.Contains(".");
			text = containsPoint.Is(true) ? text : $"{text}.";

			var caretIndex = ValueField.CaretIndex;
			var pointIndex = text.IndexOf(".");
			var digitIndex = pointIndex - caretIndex;
			var hasSign = StartWithSignSymbol(text);
			if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var value).Is(false))
				return false;
			var useMirrorTransform = caretIndex.Is(0) || (hasSign && caretIndex.Is(1));
			var step = useMirrorTransform
				? (positive.Is(true) ? -2d : +2d) * value
				: Math.Pow(10, digitIndex);

			step = From < Till ? +step : -step;
			var stepTooltip = step.ToString(DoubleFixedPointFormat);
			FromButton.ToolTip = StartWithSignSymbol(stepTooltip) ? stepTooltip : $"-{stepTooltip}";
			TillButton.ToolTip = StartWithSignSymbol(stepTooltip) ? stepTooltip : $"+{stepTooltip}";

			value += positive.Is(true) ? +step : positive.Is(false) ? -step : 0d;

			var from = From < Till ? From : Till;
			var till = From < Till ? Till : From;

			if (double.IsInfinity(from) || double.IsInfinity(till))
			{
				value = value < from ? from : value;
				value = value > till ? till : value;
			}
			else
			{
				value = value < from ? value + (till - from) : value;
				value = value > till ? value - (till - from) : value;
			}

			text = value.ToString(DoubleFixedPointFormat);
			text = StartWithSignSymbol(text) ? text : $"+{text}";
			text = text.Contains(".") ? text : $"{text}.";

			caretIndex = text.IndexOf(".") - digitIndex;

			var tillIndex = useMirrorTransform ? 1 : 2;
			for (; caretIndex < tillIndex; caretIndex++)
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

			ValueField.Text = text;
			ValueField.CaretIndex = caretIndex;

			ValueField.GetBindingExpression(Field.TextProperty).UpdateSource();

			ValueField.Text = text;
			ValueField.CaretIndex = caretIndex;

			return true;
		}

		int _floatLength;

		private async void ValueField_GotFocus(object sender, RoutedEventArgs e)
		{
			await Task.Delay(200);
			var text = ValueField.Text;
			var caretIndex = ValueField.CaretIndex;
			var textLength = text.Length;

			_floatLength = text.Contains(".") ? text.Length - text.IndexOf(".") : 0;

			text = Value.ToString(DoubleFixedPointFormat);
			if (caretIndex.Is(0) && StartWithSignSymbol(text).Is(false))
			{
				text = $"+{text}";
				caretIndex++;
			}

			var tillIndex = Math.Max(caretIndex, textLength);
			while (text.Length < tillIndex)
				text = text.Contains(".") ? $"{text}0" : $"{text}.0";

			ValueField.Text = text;
			ValueField.CaretIndex = caretIndex;

			TryRotate(default);
		}

		private static readonly Key[] HandleKeys = { Key.Left, Key.Right };

		private void ValueField_KeyUp(object sender, KeyEventArgs e) => TryRotate(default);

		private void ValueField_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = e.Key switch
			{
				Key.Up => TryRotate(true),
				Key.Down => TryRotate(false),
				Key.Left => TryMoveCaret(-1),
				Key.Right => TryMoveCaret(+1),
				_ => false
			};

			e.Handled &= HandleKeys.Contains(e.Key);
		}

		private bool TryMoveCaret(int offset)
		{
			var text = ValueField.Text;
			var targetIndex = ValueField.CaretIndex + offset;

			for (; targetIndex <= 0; targetIndex++)
			{
				text =
					text.StartsWith("-") ? $"-0{text.Substring(1)}" :
					text.StartsWith("+") ? $"+0{text.Substring(1)}" :
					$"+{text}";
			}

			if (_floatLength.Is(0) && targetIndex < text.Length)
			{
				text = text.EndsWith(".") ? text.Substring(0, text.Length - 1) : text;
				text = text.EndsWith(".0") ? text.Substring(0, text.Length - 2) : text;
			}

			if (targetIndex > text.Length && text.Contains(".").Is(false))
			{
				text = $"{text}.0";
			}

			while (targetIndex > text.Length)
			{
				text = $"{text}0";
				targetIndex++;
			}

			ValueField.Text = text;
			ValueField.CaretIndex = targetIndex;

			return IsReadOnly || TryRotate(default);
		}

		private void FromButton_Click(object sender, RoutedEventArgs e) => Click(false);

		private void TillButton_Click(object sender, RoutedEventArgs e) => Click(true);

		private void Click(bool positive)
		{
			if (Keyboard.Modifiers.Is(ModifierKeys.None))
				TryRotate(positive);
			else TryMoveCaret(positive ? +1 : -1);
		}

		private void Skip_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) =>
			e.Handled = true;

		private object FormatConverter_Convert(Markup.Patterns.ConvertArgs args) => IsKeyboardFocusWithin
			? args.Value
			: args.Value.To<double>().ToString(Format);

		private object FormatConverter_ConvertBack(Markup.Patterns.ConvertArgs args) =>
			double.TryParse(args.Value.To<string>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var value)
			? value
			: System.Windows.Data.Binding.DoNothing;
	}
}
