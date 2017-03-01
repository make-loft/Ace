using Aero;
using Aero.Input;

// ReSharper disable once CheckNamespace

namespace System.Windows.Input
{
    public class ApplicationCommands
    {
        public static Command Cut { get { return Get("Cut"); } }
        public static Command Copy { get { return Get("Copy"); } }
        public static Command Paste { get { return Get("Paste"); } }
        public static Command Undo { get { return Get("Undo"); } }
        public static Command Redo { get { return Get("Redo"); } }
        public static Command Delete { get { return Get("Delete"); } }
        public static Command Find { get { return Get("Find"); } }
        public static Command Replace { get { return Get("Replace"); } }
        public static Command Help { get { return Get("Help"); } }
        public static Command SelectAll { get { return Get("SelectAll"); } }
        public static Command New { get { return Get("New"); } }
        public static Command Open { get { return Get("Open"); } }
        public static Command Save { get { return Get("Save"); } }
        public static Command SaveAs { get { return Get("SaveAs"); } }
        public static Command Print { get { return Get("Print"); } }
        public static Command CancelPrint { get { return Get("CancelPrint"); } }
        public static Command PrintPreview { get { return Get("PrintPreview"); } }
        public static Command Close { get { return Get("Close"); } }
        public static Command Properties { get { return Get("Properties"); } }
        public static Command ContextMenu { get { return Get("ContextMenu"); } }
        public static Command CorrectionList { get { return Get("CorrectionList"); } }
        public static Command Stop { get { return Get("Stop"); } }
        public static Command NotACommand { get { return Get("NotACommand"); } }
        public static Command Last { get { return Get("Last"); } }

        public static Command Get(string key = null)
        {
            return Context.Get(key);
        }
    }

    public class MediaCommands
    {
        public static Command Play { get { return Get("Play"); } }
        public static Command Pause { get { return Get("Pause"); } }
        public static Command Stop { get { return Get("Stop"); } }
        public static Command Record { get { return Get("Record"); } }
        public static Command NextTrack { get { return Get("NextTrack"); } }
        public static Command PreviousTrack { get { return Get("PreviousTrack"); } }
        public static Command FastForward { get { return Get("FastForward"); } }
        public static Command Rewind { get { return Get("Rewind"); } }
        public static Command ChannelUp { get { return Get("ChannelUp"); } }
        public static Command ChannelDown { get { return Get("ChannelDown"); } }
        public static Command TogglePlayPause { get { return Get("TogglePlayPause"); } }
        public static Command IncreaseVolume { get { return Get("IncreaseVolume"); } }
        public static Command DecreaseVolume { get { return Get("DecreaseVolume"); } }
        public static Command MuteVolume { get { return Get("MuteVolume"); } }
        public static Command IncreaseTreble { get { return Get("IncreaseTreble"); } }
        public static Command DecreaseTreble { get { return Get("DecreaseTreble"); } }
        public static Command IncreaseBass { get { return Get("IncreaseBass"); } }
        public static Command DecreaseBass { get { return Get("DecreaseBass"); } }
        public static Command BoostBass { get { return Get("BoostBass"); } }
        public static Command IncreaseMicrophoneVolume { get { return Get("IncreaseMicrophoneVolume"); } }
        public static Command DecreaseMicrophoneVolume { get { return Get("DecreaseMicrophoneVolume"); } }
        public static Command MuteMicrophoneVolume { get { return Get("MuteMicrophoneVolume"); } }
        public static Command ToggleMicrophoneOnOff { get { return Get("ToggleMicrophoneOnOff"); } }
        public static Command Select { get { return Get("Select"); } }
        public static Command Last { get { return Get("Last"); } }

        public static Command Get(string key = null)
        {
            return Context.Get(key);
        }
    }

    public class NavigationCommands
    {
        public static Command BrowseBack { get { return Get("BrowseBack"); } }
        public static Command BrowseForward { get { return Get("BrowseForward"); } }
        public static Command BrowseHome { get { return Get("BrowseHome"); } }
        public static Command BrowseStop { get { return Get("BrowseStop"); } }
        public static Command Refresh { get { return Get("Refresh"); } }
        public static Command Favorites { get { return Get("Favorites"); } }
        public static Command Search { get { return Get("Search"); } }
        public static Command IncreaseZoom { get { return Get("IncreaseZoom"); } }
        public static Command DecreaseZoom { get { return Get("DecreaseZoom"); } }
        public static Command Zoom { get { return Get("Zoom"); } }
        public static Command NextPage { get { return Get("NextPage"); } }
        public static Command PreviousPage { get { return Get("PreviousPage"); } }
        public static Command FirstPage { get { return Get("FirstPage"); } }
        public static Command LastPage { get { return Get("LastPage"); } }
        public static Command GoToPage { get { return Get("GoToPage"); } }
        public static Command NavigateJournal { get { return Get("NavigateJournal"); } }
        public static Command Last { get { return Get("Last"); } }

        public static Command Get(string key = null)
        {
            return Context.Get(key);
        }
    }
}