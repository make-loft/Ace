using Xamarin.Forms;

namespace Ace.Controls
{
	public partial class ItemCell : Frame
	{
		public static BindableProperty ActiveCellProperty = BindableProperty.Create(nameof(ActiveCell), typeof(ItemCell), typeof(ItemCell),
			propertyChanged: (s, o, n) => s.To(out ItemCell cell).With(cell.IsActive = cell.ActiveCell.Is(cell)));

		public static BindableProperty IsActiveProperty = BindableProperty.Create(nameof(IsActive), typeof(bool), typeof(ItemCell));
		public ItemCell ActiveCell
		{
			get => GetValue(ActiveCellProperty).To<ItemCell>();
			set => SetValue(ActiveCellProperty, value);
		}

		public bool IsActive
		{
			get => GetValue(IsActiveProperty).To<bool>();
			set => SetValue(IsActiveProperty, value);
		}
	}
}
