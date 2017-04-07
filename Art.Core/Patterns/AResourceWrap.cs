using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Art.Patterns
{
    public abstract class AResourceWrap<TKey, TValue, TCulture, TManager> : INotifyPropertyChanged
    {
        public readonly ObservableCollection<TManager> MergedManagers =
            new ObservableCollection<TManager>();

        public virtual string ActivePath
        {
            get { return "ActiveManager"; }
        }

        private TManager _activeManager;

        public TManager ActiveManager
        {
            get { return _activeManager; }
            set
            {
                _activeManager = value;
                if (!ManualActivate) Activate();
            }
        }

        public bool ManualActivate { get; set; }

        public abstract TValue this[TKey key] { get; }

        public abstract TValue this[TKey key, TCulture culture] { get; }

        public void Activate()
        {
            PropertyChanged(this, new PropertyChangedEventArgs(ActivePath));
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };
    }
}