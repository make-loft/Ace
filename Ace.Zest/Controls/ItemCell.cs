#if XAMARIN
using Xamarin.Forms;

using Property = Xamarin.Forms.BindableProperty;
#else
using System.Windows.Controls;

using Property = System.Windows.DependencyProperty;
#endif

namespace Ace.Controls
{
	public partial class ItemCell : Border
	{
		public static Property ActiveCellProperty
			= Type<ItemCell>.Create(c => c.ActiveCell,
				args => args.Sender.Use(c => c.IsActive = c.ActiveCell.Is(c)));

		public static Property IsActiveProperty
			= Type<ItemCell>.Create(c => c.IsActive);

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
