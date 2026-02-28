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
		protected string DecimalSeparator => CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

		protected NumberStyles NumberStyle => Format.Is("X") ? NumberStyles.HexNumber : NumberStyles.Number;
		protected double StepBase => Format.Is("X") ? 16d : 10d;

		protected bool FormatIsIntegral => Format.Is() && (Format.Is("X") || Format.StartsWith("D"));

		public static readonly string DoubleFixedPointFormat = "0." + new string('#', 339);

		protected override double GetLength() => Till - From;

		static bool StartWithSignSymbol(string text) => text.StartsWith("+") || text.StartsWith("-");

		protected override bool TryRotate(bool? positive)
		{
			if (IsReadOnly)
				return false;

			ReadValueField(out var text, out var caretIndex);

			if (TryParse(text, out var value).Is(false))
				return false;

			var containsPoint = text.Contains(DecimalSeparator);
			text = containsPoint.Is(true) ? text : text + DecimalSeparator;

			var pointIndex = text.IndexOf(DecimalSeparator);
			var digitIndex = pointIndex - caretIndex;
			var hasSign = StartWithSignSymbol(text);

			var useMirrorTransform = caretIndex.Is(0) || (hasSign && caretIndex.Is(1));
			var step = GetStep(positive, digitIndex, value, useMirrorTransform);

			step = positive.Is(true) ? +step : positive.Is(false) ? -step : 0d;
			value = value.Rotate(step, From, Till);

			Step = step;
			Value = value;

			text = FormatIsIntegral
				? value.To<long>().ToString(Format)
				: value.ToString(DoubleFixedPointFormat)
				;

			if (caretIndex == 0)
			{
				text =
					text.StartsWith("-") ? $"-0{text.Substring(1)}" :
					text.StartsWith("+") ? $"+0{text.Substring(1)}" :
					Format.Is("X") ? $"0{text}" :
					$"+{text}";
			}

			text = StartWithSignSymbol(text) ? text : $"+{text}";
			text = text.Contains(DecimalSeparator) ? text : $"{text}{DecimalSeparator}";

			caretIndex = text.IndexOf(DecimalSeparator) - digitIndex;

			var tillIndex = useMirrorTransform ? 1 : 2;
			for (; caretIndex < tillIndex; caretIndex++)
				text = text[0] + "0" + text.Substring(1);

			while (text.Length <= caretIndex)
				text = $"{text}0";

			if (hasSign.Is(false) && text.StartsWith("+") && caretIndex > 1)
			{
				text = text.Substring(1);
				caretIndex--;
			}

			if (containsPoint.Is(false) && text.EndsWith(DecimalSeparator))
			{
				text = text.Substring(0, text.Length - DecimalSeparator.Length);
			}
			else
			{
				for (var i = text.Length - text.IndexOf(DecimalSeparator); i < _floatLength; i++)
					text = $"{text}0";
			}

			UpdateValueField(text, caretIndex);

			return true;
		}

		private double GetStep(bool? positive, int digitIndex, double value, bool useMirrorTransform) =>
			useMirrorTransform
				? (positive.Is(true) ? -2d : +2d) * value
				: Math.Pow(StepBase, digitIndex)
				;

		int _floatLength;

		protected override void ValueField_SelectionChanged(object sender, RoutedEventArgs args)
		{
			ReadValueField(out var text, out var caretIndex);

			var containsPoint = text.Contains(DecimalSeparator);
			text = containsPoint.Is(true) ? text : text + DecimalSeparator;

			var pointIndex = text.IndexOf(DecimalSeparator);
			var digitIndex = pointIndex - caretIndex;
			var hasSign = StartWithSignSymbol(text);
			if (TryParse(text, out var value).Is(false))
				return;

			var useMirrorTransform = caretIndex.Is(0) || (hasSign && caretIndex.Is(1));
			var step = GetStep(positive: true, digitIndex, value, useMirrorTransform);
			step = From < Till ? +step : -step;
			Step = step;
		}

		protected override void ValueField_LostFocus(object sender, RoutedEventArgs e)
		{
			ReadValueField(out var text, out var caretIndex);
			if (TryParse(text, out _).Is(false))
				WriteValueField(text, caretIndex);
			TryRotate(default);
		}

		protected override async void ValueField_GotFocus(object sender, RoutedEventArgs args)
		{
			await Task.Delay(200);

			ReadValueField(out var text, out var caretIndex);

			var textLength = text.Length;
			if (FormatIsIntegral)
			{
				text = Value.To<long>().ToString(Format);
			}
			else
			{
				_floatLength = text.Contains(DecimalSeparator) ? text.Length - text.IndexOf(DecimalSeparator) : 0;

				text = Value.ToString(DoubleFixedPointFormat);
				if (caretIndex.Is(0) && StartWithSignSymbol(text).Is(false))
				{
					text = $"+{text}";
					caretIndex++;
				}

				var tillIndex = Math.Max(caretIndex, textLength);
				while (text.Length < tillIndex)
					text = text.Contains(DecimalSeparator) ? $"{text}0" : $"{text}{DecimalSeparator}0";
			}

			WriteValueField(text, caretIndex);

			TryRotate(default);
		}

		protected override bool TryMoveCaret(int offset)
		{
			ReadValueField(out var text, out var caretIndex);

			var targetIndex = (caretIndex + offset).Clip(0, text.Length);

			WriteValueField(text, targetIndex);

			if (TryRotate(default))
				return true;

			WriteValueField(text, caretIndex);
			return false;
		}

		protected override bool TryFormat(in double value, out string text) => FormatIsIntegral
			? value.To<long>().ToString(Format).To(out text).Is()
			: value.ToString(Format).To(out text).Is()
			;

		protected override bool TryParse(in string text, out double value) => FormatIsIntegral
			? long.TryParse(text, NumberStyle, CultureInfo.CurrentCulture, out var tmp).With(value = tmp)
			: double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out value)
			;
	}
}
