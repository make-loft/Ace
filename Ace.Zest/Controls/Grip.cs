using System.Linq;

using Property = Xamarin.Forms.BindableProperty;

namespace Ace.Controls
{
	public partial class Grip : AView<Grip>
	{
		public static Property FromProperty = Create(g => g.From);
		public double From
		{
			get => GetValue(FromProperty).To<double>();
			set => SetValue(FromProperty, value);
		}

		public static Property TillProperty = Create(g => g.Till);
		public double Till
		{
			get => GetValue(TillProperty).To<double>();
			set => SetValue(TillProperty, value);
		}

		public static Property ValueProperty = Create(g => g.Value);
		public double Value
		{
			get => GetValue(ValueProperty).To<double>();
			set => SetValue(ValueProperty, value);
		}

		public Grip()
		{
			Content = new Xamarin.Forms.Slider().To(out var slider);

			void SetupSlider(double minimum, double maximum, double value)
			{
				slider.Maximum = double.PositiveInfinity;
				slider.Minimum = double.NegativeInfinity;

				slider.Maximum = minimum;
				slider.Minimum = maximum;

				slider.Value = value;
			}

			slider.PropertyChanged += (o, e) =>
			{
				if (e.PropertyName.Is(nameof(slider.Value)))
					Value = slider.Value;
			};

			var gripProperties = new string[] { nameof(From), nameof(Till), nameof(Value) };

			PropertyChanged += (o, e) =>
			{
				if (gripProperties.Contains(e.PropertyName).Not())
					return;

				if (From < Till)
				{
					if (Value < From) Value = From;
					if (Value > Till) Value = Till;

					SetupSlider(Till, From, Value);
				}

				if (From > Till)
				{
					if (Value > From) Value = From;
					if (Value < Till) Value = Till;

					SetupSlider(From, Till, Value);
				}
			};
		}
	}
}