using Ace.Extensions;
using Ace.Mathematics;

using System.Threading.Tasks;
using System.Windows;

#if XAMARIN
using Xamarin.Forms;

using Colors = Xamarin.Forms.Color;
#else
using System.Windows.Media;
#endif

using static Ace.Extensions.Colority;

namespace Ace.Controls
{
	public partial class ColorField : AField<Color>
	{
		static ColorField() => Initialize<ColorField>();

		Color DefaultFrom { get; } = FromARGB(0x00, 0x00, 0x00, 0x00);
		Color DefaultTill { get; } = FromARGB(0xFF, 0xFF, 0xFF, 0xFF);
		Color DefaultValue { get; } = Colors.DimGray;
		Color DefaultStep { get; } = Colors.Transparent;
		Color DefaultLength { get; } = Colors.Transparent;

		public override Color From { get => this.Get(DefaultFrom); set => this.Set(value); }
		public override Color Till { get => this.Get(DefaultTill); set => this.Set(value); }
		public override Color Length { get => this.Get(DefaultLength); protected set => this.Set(value); }

		public override Color Value { get => this.Get(DefaultValue); set => this.Set(value); }
		public override Color Step { get => this.Get(DefaultStep); protected set => this.Set(value); }

		public static readonly string DoubleFixedPointFormat = "0." + new string('#', 339);

		protected override bool TryRotate(bool? positive)
		{
			if (IsReadOnly)
				return false;

			ReadValueField(out var text, out var caretIndex);

			if (TryParse(text, out var value).Is(false))
				return false;

			var offset = text.Length - caretIndex;
			var stepA = offset.Is(7) ? 0x10 : offset.Is(6) ? 0x01 : 0x00;
			var stepR = offset.Is(5) ? 0x10 : offset.Is(4) ? 0x01 : 0x00;
			var stepG = offset.Is(3) ? 0x10 : offset.Is(2) ? 0x01 : 0x00;
			var stepB = offset.Is(1) ? 0x10 : offset.Is(0) ? 0x01 : 0x00;

			var step = FromARGB(stepA, stepR, stepG, stepB);

			static int InvertSign(int d, bool? negate) =>
				negate is true ? -d : negate is false ? +d : 0;

			bool? negate = positive is true ? false : positive is false ? true : null;
			
			var valueA = value.GetA().Rotate(InvertSign(stepA, negate), From.GetA(), Till.GetA());
			var valueR = value.GetR().Rotate(InvertSign(stepR, negate), From.GetR(), Till.GetA());
			var valueG = value.GetG().Rotate(InvertSign(stepG, negate), From.GetG(), Till.GetG());
			var valueB = value.GetB().Rotate(InvertSign(stepB, negate), From.GetB(), Till.GetB());

			value = FromARGB(valueA, valueR, valueG, valueB);

			Step = step;
			Value = value;

			text = value.ToString();

			UpdateValueField(text, caretIndex);

			return true;
		}

		protected override async void ValueField_GotFocus(object sender, RoutedEventArgs e)
		{
			await Task.Delay(200);

			ReadValueField(out var text, out var caretIndex);

			Value = text.ToColor();

			text = Value.ToString();

			WriteValueField(text, caretIndex);

			TryRotate(default);
		}

		protected override bool TryMoveCaret(int offset)
		{
			ReadValueField(out var text, out var caretIndex);

			var targetIndex = (caretIndex + offset).Clip(0, text.Length);

			WriteValueField(text, targetIndex);

			return IsReadOnly || TryRotate(default);
		}

		protected override bool TryFormat(in Color value, out string text) =>
			value.To(out text).Is();

		protected override bool TryParse(in string text, out Color value) =>
			text.TryParse(out value, text => text.ToColor());
	}
}
