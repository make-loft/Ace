namespace Ace.Controls;

#if XAMARIN
public class ContentControl : Xamarin.Forms.ContentView
{
	public object ToolTip { get; set; }
}
#else
public class ContentControl : System.Windows.Controls.ContentControl
{
}
#endif
