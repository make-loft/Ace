using Ace;
using Ace.Input;

// ReSharper disable once CheckNamespace

namespace System.Windows.Input
{
    public class ApplicationCommands
    {
        public static Command Cut => Get("Cut");
        public static Command Copy => Get("Copy");
        public static Command Paste => Get("Paste");
        public static Command Undo => Get("Undo");
        public static Command Redo => Get("Redo");
        public static Command Delete => Get("Delete");
        public static Command Find => Get("Find");
        public static Command Replace => Get("Replace");
        public static Command Help => Get("Help");
        public static Command SelectAll => Get("SelectAll");
        public static Command New => Get("New");
        public static Command Open => Get("Open");
        public static Command Save => Get("Save");
        public static Command SaveAs => Get("SaveAs");
        public static Command Print => Get("Print");
        public static Command CancelPrint => Get("CancelPrint");
        public static Command PrintPreview => Get("PrintPreview");
        public static Command Close => Get("Close");
        public static Command Properties => Get("Properties");
        public static Command ContextMenu => Get("ContextMenu");
        public static Command CorrectionList => Get("CorrectionList");
        public static Command Stop => Get("Stop");
        public static Command NotACommand => Get("NotACommand");
        public static Command Last => Get("Last");

        public static Command Get(string key = null) => Context.Get(key);
    }

    public class MediaCommands
    {
        public static Command Play => Get("Play");
        public static Command Pause => Get("Pause");
        public static Command Stop => Get("Stop");
        public static Command Record => Get("Record");
        public static Command NextTrack => Get("NextTrack");
        public static Command PreviousTrack => Get("PreviousTrack");
        public static Command FastForward => Get("FastForward");
        public static Command Rewind => Get("Rewind");
        public static Command ChannelUp => Get("ChannelUp");
        public static Command ChannelDown => Get("ChannelDown");
        public static Command TogglePlayPause => Get("TogglePlayPause");
        public static Command IncreaseVolume => Get("IncreaseVolume");
        public static Command DecreaseVolume => Get("DecreaseVolume");
        public static Command MuteVolume => Get("MuteVolume");
        public static Command IncreaseTreble => Get("IncreaseTreble");
        public static Command DecreaseTreble => Get("DecreaseTreble");
        public static Command IncreaseBass => Get("IncreaseBass");
        public static Command DecreaseBass => Get("DecreaseBass");
        public static Command BoostBass => Get("BoostBass");
        public static Command IncreaseMicrophoneVolume => Get("IncreaseMicrophoneVolume");
        public static Command DecreaseMicrophoneVolume => Get("DecreaseMicrophoneVolume");
        public static Command MuteMicrophoneVolume => Get("MuteMicrophoneVolume");
        public static Command ToggleMicrophoneOnOff => Get("ToggleMicrophoneOnOff");
        public static Command Select => Get("Select");
        public static Command Last => Get("Last");

        public static Command Get(string key = null) => Context.Get(key);
    }

    public class NavigationCommands
    {
        public static Command BrowseBack => Get("BrowseBack");
        public static Command BrowseForward => Get("BrowseForward");
        public static Command BrowseHome => Get("BrowseHome");
        public static Command BrowseStop => Get("BrowseStop");
        public static Command Refresh => Get("Refresh");
        public static Command Favorites => Get("Favorites");
        public static Command Search => Get("Search");
        public static Command IncreaseZoom => Get("IncreaseZoom");
        public static Command DecreaseZoom => Get("DecreaseZoom");
        public static Command Zoom => Get("Zoom");
        public static Command NextPage => Get("NextPage");
        public static Command PreviousPage => Get("PreviousPage");
        public static Command FirstPage => Get("FirstPage");
        public static Command LastPage => Get("LastPage");
        public static Command GoToPage => Get("GoToPage");
        public static Command NavigateJournal => Get("NavigateJournal");
        public static Command Last => Get("Last");

        public static Command Get(string key = null) => Context.Get(key);
    }
}