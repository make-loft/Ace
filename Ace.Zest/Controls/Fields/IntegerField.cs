using Ace.Mathematics;

using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace Ace.Controls
{
	public partial class IntegerField : AField<int>
	{
		static IntegerField() => Initialize<IntegerField>();

		public override int Value { get => this.Get(0); set => this.Set(value); }
		public override int Step { get => this.Get(0); protected set => this.Set(value); }
		public override int From { get => this.Get(int.MinValue); set => this.Set(value); }
		public override int Till { get => this.Get(int.MaxValue); set => this.Set(value); }
		public override int Length { get => this.Get(0); protected set => this.Set(value); }

		protected override int GetLength() => Till - From;

		static bool StartWithSignSymbol(string text) => text.StartsWith("+") || text.StartsWith("-");

		private NumberStyles NumberStyle => Format.Is("X") ? NumberStyles.HexNumber : NumberStyles.Number;


		protected override bool TryRotate(bool? positive)
		{
			if (IsReadOnly)
				return false;

			ReadValueField(out var text, out var caretIndex);

			var digitIndex = text.Length - caretIndex;
			var hasSign = StartWithSignSymbol(text);

			if (int.TryParse(text, NumberStyle, CultureInfo.InvariantCulture, out var value).Is(false))
				return false;
			var useMirrorTransform = caretIndex.Is(0) || (hasSign && caretIndex.Is(1));
			var stepBase = Format.Is("X") ? 16 : 10;
			var step = useMirrorTransform
				? (positive.Is(true) ? -2 : +2) * value
				: (int)Math.Pow(stepBase, digitIndex);

			step = positive.Is(true) ? +step : positive.Is(false) ? -step : 0;
			value = value.Rotate(step, From, Till);

			Step = step;
			Value = value;

			text = value.ToString(Format);
			text = StartWithSignSymbol(text) ? text : $"+{text}";

			caretIndex = text.Length - digitIndex;

			var tillIndex = useMirrorTransform ? 1 : 2;
			for (; caretIndex < tillIndex; caretIndex++)
				text = text[0] + "0" + text.Substring(1);

			if (Format.Is("X") || (hasSign.Is(false) && text.StartsWith("+")))
			{
				text = text.Substring(1);
				caretIndex--;
			}

			UpdateValueField(text, caretIndex);

			return true;
		}

		protected override void ValueField_SelectionChanged(object sender, RoutedEventArgs args)
		{
			ReadValueField(out var text, out var caretIndex);

			var digitIndex = text.Length - caretIndex;
			var hasSign = StartWithSignSymbol(text);

			if (int.TryParse(text, NumberStyle, CultureInfo.InvariantCulture, out var value).Is(false))
				return;
			var useMirrorTransform = caretIndex.Is(0) || (hasSign && caretIndex.Is(1));
			var stepBase = Format.Is("X") ? 16 : 10;
			var step = useMirrorTransform
				? 2 * value
				: (int)Math.Pow(stepBase, digitIndex);

			Step = step;
		}

		protected override async void ValueField_GotFocus(object sender, RoutedEventArgs e)
		{
			await Task.Delay(200);

			ReadValueField(out var text, out var caretIndex);

			var textLength = text.Length;

			text = Value.ToString(Format);
			if (caretIndex.Is(0) && Format.IsNot("X") && StartWithSignSymbol(text).Is(false))
			{
				text = $"+{text}";
				caretIndex++;
			}

			var tillIndex = Math.Max(caretIndex, textLength);
			while (text.Length < tillIndex)
				text = $"{text}0";

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
					Format.Is("X") ? $"0{text}" :
					$"+{text}";
			}

			WriteValueField(text, targetIndex);

			return IsReadOnly || TryRotate(default);
		}

		protected override bool TryFormat(in int value, out string text) =>
			value.ToString(Format).To(out text).Is();

		protected override bool TryParse(in string text, out int value) =>
			int.TryParse(text, NumberStyle, default, out value);
	}
}
