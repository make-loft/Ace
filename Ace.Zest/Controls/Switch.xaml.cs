using System.Windows;

namespace Ace.Controls
{
	public partial class Switch
	{
		public Switch()
		{
			InitializeComponent();

			void StateChanged(object sender, RoutedEventArgs e) => IsToggled = IsChecked;

			Checked += StateChanged;
			Unchecked += StateChanged;
		}

		public static readonly DependencyProperty IsToggledProperty = Type<Switch>.Create(s => s.IsToggled,
			args => args.Sender.IsChecked = args.NewValue);

		public bool? IsToggled
		{
			get => GetValue(IsToggledProperty).To<bool?>();
			set => SetValue(IsToggledProperty, value);
		}
	}
}