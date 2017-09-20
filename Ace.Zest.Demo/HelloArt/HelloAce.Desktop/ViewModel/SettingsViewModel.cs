using System.Runtime.Serialization;
using Ace;
using HelloAce.Languages;

namespace HelloAce.ViewModel
{
    [DataContract]
    public class SettingsViewModel : ContextObject, IExposable
    {
        [DataMember]
        public string Language
        {
            get { return Get(() => Language); }
            set { Set(() => Language, value); }
        }

        public ContextSet<string> Languages { get; set; }

        public void Expose()
        {
            Languages = new ContextSet<string> {"Russian", "English"};

            this[Context.Get("SetLanguage")].CanExecute += (sender, args) =>
                args.CanExecute = Language != args.Parameter.ToString();

            this[Context.Get("SetLanguage")].Executed += (sender, args) =>
                Language = args.Parameter.ToString();

            this[() => Language].PropertyChanged += (sender, args) =>
            {
                LocalizationSource.Wrap.ActiveManager = Language == "Russian"
                    ? Russian.ResourceManager
                    : English.ResourceManager;

                Context.Get("SetLanguage").RaiseCanExecuteChanged();
            };

            RaisePropertyChanged(() => Language);
        }
    }
}