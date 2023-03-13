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
	using System.Windows.Controls;

	public class Field : TextBox
	{
		public static readonly System.Collections.Generic.List<WeakReference<Field>> Entres = new();
		readonly WeakReference<Field> _this;

		public Field()
		{
			Entres.Add(_this = new(this));

			KeyDown += (o, e) =>
			{
				if (e.Key != System.Windows.Input.Key.Enter)
					return;

				GetBindingExpression(Field.TextProperty).UpdateSource();
			};
		}

		~Field() => Entres.Remove(_this);

		public static void GlobalTextBindingRefresh() => Entres.ForEach(w =>
		{
			if (w.TryGetTarget(out var e))
				e.GetBindingExpression(Field.TextProperty).UpdateTarget();
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
