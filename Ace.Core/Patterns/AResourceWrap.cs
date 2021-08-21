using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Ace.Patterns
{
	public abstract class AResourceWrap<TKey, TValue, TCulture, TManager> : INotifyPropertyChanged
	{
		public readonly ObservableCollection<TManager> MergedManagers = new();

		public virtual string ActivePath => "ActiveManager";

		private TManager _activeManager;

		public TManager ActiveManager
		{
			get => _activeManager;
			set
			{
				_activeManager = value;
				if (ManualActivate) return;
				Activate();
			}
		}

		public bool ManualActivate { get; set; }

		public abstract TValue this[TKey key] { get; }

		public abstract TValue this[TKey key, TCulture culture] { get; }

		public void Activate() => PropertyChanged(this, new(ActivePath));

		public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };
	}
}