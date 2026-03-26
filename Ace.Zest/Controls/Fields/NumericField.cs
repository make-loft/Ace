using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

using Ace.Mathematics;

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

		protected static readonly string StrictDecimalFormat = "0." + new string('#', 339);

		protected override double GetLength() => Till - From;

		protected static bool StartWithSignSymbol(string text) => text.StartsWith("+") || text.StartsWith("-");

		protected override bool TryRotate(bool? positive)
		{
			if (IsReadOnly)
				return false;

			ReadValueField(out var text, out var caretIndex);

			if (TryParse(text, out var value).Is(false))
				return false;

			var hasSign = StartWithSignSymbol(text);
			var hasPoint = text.Contains(DecimalSeparator);

			var pointIndex = hasPoint ? text.IndexOf(DecimalSeparator) : text.Length;
			var digitIndex = pointIndex - caretIndex;

			var useMirrorTransform = caretIndex.Is(0) || (hasSign && caretIndex.Is(1));
			var step = GetStep(StepBase, positive, digitIndex, value, useMirrorTransform);

			step = positive.Is(true) ? +step : positive.Is(false) ? -step : 0d;
			value = value.Rotate(step, From, Till);

			Step = step;
			Value = value;

			text = FormatIsIntegral
				? value.To<long>().ToString(Format)
				: value.ToString(StrictDecimalFormat)
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

			if (hasPoint.Is(false) && text.EndsWith(DecimalSeparator))
			{
				text = text.Substring(0, text.Length - DecimalSeparator.Length);
			}
			else
			{
				for (var i = text.Length - text.IndexOf(DecimalSeparator); i < GotFocusFloatLength; i++)
					text = $"{text}0";
			}

			UpdateValueField(text, caretIndex);

			return true;
		}

		protected static double GetStep(double stepBase, bool? positive, int digitIndex, double value, bool useMirrorTransform) =>
			useMirrorTransform
				? (positive.Is(true) ? +2d : -2d) * value
				: Math.Pow(stepBase, digitIndex)
				;

		protected int GotFocusFloatLength;

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
			var step = GetStep(StepBase, positive: true, digitIndex, value, useMirrorTransform);
			step = From < Till ? +step : -step;
			Step = step;
		}

		protected int LostFocusCaretIndex = -1;
		protected override void ValueField_LostFocus(object sender, RoutedEventArgs e)
		{
			ReadValueField(out var text, out LostFocusCaretIndex);
			if (TryParse(text, out _).Is(false))
				WriteValueField(text, LostFocusCaretIndex);
			TryRotate(default);

			TryFormat(Value, out text);

			AcceptUpdateFlag = true;
			WriteValueField(text, LostFocusCaretIndex);
			AcceptUpdateFlag = false;
		}

		protected override async void ValueField_GotFocus(object sender, RoutedEventArgs args)
		{
			await Task.Delay(128);

			ReadValueField(out var text, out var caretIndex);
			caretIndex = LostFocusCaretIndex < 0 ? caretIndex : LostFocusCaretIndex;

			var textLength = text.Length;
			if (FormatIsIntegral)
			{
				text = Value.To<long>().ToString(Format);
			}
			else
			{
				GotFocusFloatLength = text.Contains(DecimalSeparator) ? text.Length - text.IndexOf(DecimalSeparator) : 0;

				text = Value.ToString(StrictDecimalFormat);
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

		private bool AcceptUpdateFlag;

		internal override void Update(object value)
		{
			if (AcceptUpdateFlag.Is(true))
				return;

			var isValidNumber = TryParse(value.To<string>(), out var number);
			if (isValidNumber.Not())
				throw new FormatException();
			if (number.IsOutOfRange(From, Till))
				throw new ArgumentOutOfRangeException();

			AcceptUpdateFlag = true;
			ReadValueField(out var text, out var caretIndex);

			Value = number;

			WriteValueField(value.To<string>(), caretIndex);
			AcceptUpdateFlag = false;
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
