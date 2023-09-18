using Ace.Mathematics;

using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace Ace.Controls
{
	public partial class NumericField : AField<double>
	{
		static NumericField() => Initialize<NumericField>();

		public override double Value { get => this.Get(0d); set => this.Set(value); }
		public override double Step { get => this.Get(0d); protected set => this.Set(value); }
		public override double From { get => this.Get(double.NegativeInfinity); set => this.Set(value); }
		public override double Till { get => this.Get(double.PositiveInfinity); set => this.Set(value); }
		public override double Length { get => this.Get(0d); protected set => this.Set(value); }

		protected override double GetLength() => Till - From;

		static bool StartWithSignSymbol(string text) => text.StartsWith("+") || text.StartsWith("-");

		public static readonly string DoubleFixedPointFormat = "0." + new string('#', 339);

		protected override bool TryRotate(bool? positive)
		{
			if (IsReadOnly)
				return false;

			ReadValueField(out var text, out var caretIndex);

			var containsPoint = text.Contains(".");
			text = containsPoint.Is(true) ? text : $"{text}.";

			var pointIndex = text.IndexOf(".");
			var digitIndex = pointIndex - caretIndex;
			var hasSign = StartWithSignSymbol(text);
			if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var value).Is(false))
				return false;
			var useMirrorTransform = caretIndex.Is(0) || (hasSign && caretIndex.Is(1));
			var step = useMirrorTransform
				? 2d * value
				: Math.Pow(10, digitIndex);

			step = positive.Is(true) ? +step : positive.Is(false) ? -step : 0d;
			value = value.Rotate(step, From, Till);

			Step = step;
			Value = value;

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

			UpdateValueField(text, caretIndex);

			return true;
		}

		int _floatLength;

		protected override void ValueField_SelectionChanged(object sender, RoutedEventArgs e)
		{
			ReadValueField(out var text, out var caretIndex);

			var containsPoint = text.Contains(".");
			text = containsPoint.Is(true) ? text : $"{text}.";

			var pointIndex = text.IndexOf(".");
			var digitIndex = pointIndex - caretIndex;
			var hasSign = StartWithSignSymbol(text);
			if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var value).Is(false))
				return;
			var useMirrorTransform = caretIndex.Is(0) || (hasSign && caretIndex.Is(1));
			var step = useMirrorTransform
				? 2d * value
				: Math.Pow(10, digitIndex);

			step = From < Till ? +step : -step;
			Step = step;
		}

		protected override async void ValueField_GotFocus(object sender, RoutedEventArgs e)
		{
			await Task.Delay(200);

			ReadValueField(out var text, out var caretIndex);

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

			WriteValueField(text, caretIndex);

			TryRotate(default);
		}

		protected override bool TryMoveCaret(int offset)
		{
			ReadValueField(out var text, out var caretIndex);

			var targetIndex = caretIndex + offset;

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

			WriteValueField(text, targetIndex);

			return IsReadOnly || TryRotate(default);
		}

		protected override bool TryFormat(in double value, out string text) =>
			value.ToString(Format).To(out text).Is();

		protected override bool TryParse(in string text, out double value) =>
			double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
	}
}
