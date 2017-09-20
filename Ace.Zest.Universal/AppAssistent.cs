using System;
using System.Diagnostics;
using System.Globalization;
using Windows.Globalization;
using Windows.UI.Xaml;
using Aero.Markup;

namespace Aero
{
    public class AppAssistent
    {
        public string Localize(string key, string stringFormat = null)
        {
            return Localizing.ActiveManager.Get(key, stringFormat);
        }

        public void Exit()
        {
            Application.Current.Exit();
        }

        public void Trace(Exception exception)
        {
            Debug.WriteLine(exception);
        }

        public object GetCurrentCulture()
        {
            
            return ApplicationLanguages.PrimaryLanguageOverride;
        }

        public void SetInvariantCulture()
        {
            
        }

        public void SetCurrentCulture(object culture)
        {
            ApplicationLanguages.PrimaryLanguageOverride = (string) culture;
        }
    }
}