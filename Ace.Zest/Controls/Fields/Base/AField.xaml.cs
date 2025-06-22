using System.Windows;
using System.Linq;
using System.Windows.Input;
using Ace.Markup.Patterns;

#if XAMARIN
using Binding = Xamarin.Forms.Binding;
#else
using System.Windows.Data;
#endif

namespace Ace.Controls
{
	public abstract partial class AField
	{
		internal AField() => InitializeComponent();

		public string Format { get => this.Get(default(string)); set => this.Set(value); }
		public bool IsReadOnly { get => this.Get(false); set => this.Set(value); }

		protected virtual void ValueField_GotFocus(object sender, RoutedEventArgs args) { }

		protected virtual bool TryMoveCaret(int offset) => default;
		protected virtual bool TryRotate(bool? positive) => default;

		public bool Handle(Key key) => key switch
		{
			Key.Up => TryRotate(true),
			Key.Down => TryRotate(false),
			Key.Left => TryMoveCaret(-1),
			Key.Right => TryMoveCaret(+1),
			_ => false
		};

		public static Key Modify(Key key) => key switch
		{
			Key.Up => Key.Left,
			Key.Down => Key.Right,
			Key.Left => Key.Down,
			Key.Right => Key.Up,
			_ => key
		};

		public ModifierKeys[] AllowedModifiers { get; set; } =
			{ ModifierKeys.Control, ModifierKeys.Shift, ModifierKeys.Alt };

		public bool IsModifiedMode(ModifierKeys modifiers) => AllowedModifiers
			.Any(m => modifiers.HasFlag(m));

		private void ValueField_PreviewMouseWheel(object sender, MouseWheelEventArgs args)
		{
			if (ValueField.IsKeyboardFocused.IsNot(true))
				return;

			var key = IsModifiedMode(Keyboard.Modifiers)
				?
				(
					args.Delta > +0 ? Key.Left :
					args.Delta < -0 ? Key.Right :
					Key.Enter
				)
				:
				(
					args.Delta > +0 ? Key.Up :
					args.Delta < -0 ? Key.Down :
					Key.Enter
				)
				;

			args.Handled = Handle(key);
		}

		private static readonly Key[] HandleKeys = { Key.Up, Key.Down, Key.Left, Key.Right, Key.Enter };

		protected virtual void ValueField_PreviewKeyDown(object sender, KeyEventArgs args)
		{
			var key = IsModifiedMode(Keyboard.Modifiers)
				? Modify(args.Key)
				: args.Key
				;

			args.Handled = Handle(key);

			var isKeyHandled = HandleKeys.Contains(key);
			if (isKeyHandled)
				TryRotate(default);

			args.Handled &= isKeyHandled;
		}

		protected virtual void Click(bool positive)
		{
			if (Keyboard.Modifiers.Is(ModifierKeys.None))
				TryRotate(positive);
			else TryMoveCaret(positive ? +1 : -1);
		}

		protected void ReadValueField(out string text, out int caretIndex)
		{
			ValueField.Text.To(out text);
			ValueField.CaretIndex.To(out caretIndex);
		}

		protected void WriteValueField(in string text, in int caretIndex)
		{
			ValueField.Text = text;
			ValueField.CaretIndex = caretIndex;
		}

		protected void UpdateValueField(in string text, in int caretIndex)
		{
			WriteValueField(in text, in caretIndex);
#if !XAMARIN
			ValueField.GetBindingExpression(Field.TextProperty)?.UpdateSource();
#endif
			WriteValueField(in text, in caretIndex);
		}

		protected virtual void FromButton_Click(object sender, RoutedEventArgs args) => Click(false);

		protected virtual void TillButton_Click(object sender, RoutedEventArgs args) => Click(true);

		protected void Skip_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args) =>
			args.Handled = true;

		protected virtual object ToolTipConverter_Convert(ConvertArgs args) => args.Value;
		
		protected virtual object FormatConverter_Convert(ConvertArgs args) => args.Value;

		protected virtual object FormatConverter_ConvertBack(ConvertArgs args) => args.Value;

		protected virtual void ValueField_SelectionChanged(object sender, RoutedEventArgs args) { }
	}

	public abstract class AField<TValue> : AField
	{
		protected static void Initialize<TField>() where TField : AField<TValue>, new()
		{
			Type<TField>.Create(v => v.Value);
#if !XAMARIN
			Type<TField>.When(v => v.Format).Changed += args =>
				args.Sender.ValueField.GetBindingExpression(Field.TextProperty)?.UpdateTarget();
#endif
			Type<TField>.When(v => v.From).Changed += args =>
				args.Sender.To(out var v).Length = v.GetLength();

			Type<TField>.When(v => v.Till).Changed += args =>
				args.Sender.To(out var v).Length = v.GetLength();

			Type<TField>.CreateProperties(TypeOf<AField>.Raw);
		}

		public abstract TValue Value { get; set; }
		public abstract TValue Step { get; protected set; }
		public abstract TValue From { get; set; }
		public abstract TValue Till { get; set; }
		public abstract TValue Length { get; protected set; }

		protected virtual TValue GetLength() => default;

		protected virtual bool TryFormat(in TValue value, out string text) => value.Is(out text);
		protected virtual bool TryParse(in string text, out TValue value) => text.Is(out value);

		protected override object ToolTipConverter_Convert(ConvertArgs args) => args.Value;

		protected override object FormatConverter_Convert(ConvertArgs args) =>
			TryFormat(args.Value.To<TValue>(), out var text) ? text : Binding.DoNothing;

		protected override object FormatConverter_ConvertBack(ConvertArgs args) =>
			TryParse(args.Value.To<string>(), out var value) ? value : Binding.DoNothing;
	}
}
