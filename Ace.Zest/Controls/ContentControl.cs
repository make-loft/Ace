namespace Ace.Controls;

#if MAUI
public class ContentControl : Microsoft.Maui.Controls.ContentView
{
	public object ToolTip { get; set; }
}
#endif
#if XAMARIN
public class ContentControl : Xamarin.Forms.ContentView
{
	public object ToolTip { get; set; }
}
#endif
#if DESKTOP
public class ContentControl : System.Windows.Controls.ContentControl
{
}
#endif
