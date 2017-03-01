using System.ComponentModel;
using System.Resources;

namespace Aero.Markup
{
    public partial class Localizing
    {
        public class Manager : INotifyPropertyChanged
        {
            private ResourceManager _source;

            public ResourceManager Source
            {
                get { return _source; }
                set
                {
                    _source = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Source"));
                }
            }

            public string Get(string key, string stringFormat = null)
            {
                if (string.IsNullOrWhiteSpace(key)) return key;
                var localizedValue = _source == null ? ":" + key + ":" : _source.GetString(key) ?? ":" + key + ":";
                return string.IsNullOrEmpty(stringFormat)
                    ? localizedValue
                    : string.Format(stringFormat, localizedValue);
            }

            public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };
        }
    }
}