using System;
using System.Runtime.CompilerServices;

using Ace;
using Ace.Input;

namespace System.Windows.Input;

//[TypeConverter(typeof(KeyConverter))]
//[ValueSerializer(typeof(KeyValueSerializer))]
public enum Key
{
	None, Left, Right, Up, Down, Enter
}

//[TypeConverter(typeof(ModifierKeysConverter))]
//[ValueSerializer(typeof(ModifierKeysValueSerializer))]
[Flags]
public enum ModifierKeys
{
	None = 0x0,
	Alt = 0x1,
	Control = 0x2,
	Shift = 0x4,
	Windows = 0x8
}

public static class Keyboard
{
	public static ModifierKeys Modifiers { get; set; }
}

public class MouseWheelEventArgs : EventArgs
{
	public bool Handled { get; set; }
	public int Delta { get; set; }
}

public class ApplicationCommands
{
	public static Command Cut => Get();
	public static Command Copy => Get();
	public static Command Paste => Get();
	public static Command Undo => Get();
	public static Command Redo => Get();
	public static Command Delete => Get();
	public static Command Find => Get();
	public static Command Replace => Get();
	public static Command Help => Get();
	public static Command SelectAll => Get();
	public static Command New => Get();
	public static Command Open => Get();
	public static Command Save => Get();
	public static Command SaveAs => Get();
	public static Command Print => Get();
	public static Command CancelPrint => Get();
	public static Command PrintPreview => Get();
	public static Command Close => Get();
	public static Command Properties => Get();
	public static Command ContextMenu => Get();
	public static Command CorrectionList => Get();
	public static Command Stop => Get();
	public static Command NotACommand => Get();
	public static Command Last => Get();

	public static Command Get([CallerMemberName] string key = null) => Context.Get(key);
}

public class MediaCommands
{
	public static Command Play => Get();
	public static Command Pause => Get();
	public static Command Stop => Get();
	public static Command Record => Get();
	public static Command NextTrack => Get();
	public static Command PreviousTrack => Get();
	public static Command FastForward => Get();
	public static Command Rewind => Get();
	public static Command ChannelUp => Get();
	public static Command ChannelDown => Get();
	public static Command TogglePlayPause => Get();
	public static Command IncreaseVolume => Get();
	public static Command DecreaseVolume => Get();
	public static Command MuteVolume => Get();
	public static Command IncreaseTreble => Get();
	public static Command DecreaseTreble => Get();
	public static Command IncreaseBass => Get();
	public static Command DecreaseBass => Get();
	public static Command BoostBass => Get();
	public static Command IncreaseMicrophoneVolume => Get();
	public static Command DecreaseMicrophoneVolume => Get();
	public static Command MuteMicrophoneVolume => Get();
	public static Command ToggleMicrophoneOnOff => Get();
	public static Command Select => Get();
	public static Command Last => Get();

	public static Command Get([CallerMemberName] string key = null) => Context.Get(key);
}

public class NavigationCommands
{
	public static Command BrowseBack => Get();
	public static Command BrowseForward => Get();
	public static Command BrowseHome => Get();
	public static Command BrowseStop => Get();
	public static Command Refresh => Get();
	public static Command Favorites => Get();
	public static Command Search => Get();
	public static Command IncreaseZoom => Get();
	public static Command DecreaseZoom => Get();
	public static Command Zoom => Get();
	public static Command NextPage => Get();
	public static Command PreviousPage => Get();
	public static Command FirstPage => Get();
	public static Command LastPage => Get();
	public static Command GoToPage => Get();
	public static Command NavigateJournal => Get();
	public static Command Last => Get();

	public static Command Get([CallerMemberName] string key = null) => Context.Get(key);
}