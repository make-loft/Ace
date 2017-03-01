using System;
using System.Diagnostics;
using System.Globalization;

namespace Aero.Specific
{
    public class Core
    {
        public static Core Aid = new Core();

        public string Localize(string key, string stringFormat = null)
        {
            return LocalizationSource.Wrap[key];
        }

        public void Exit()
        {
            
        }

        public void Trace(Exception exception)
        {
            Debug.WriteLine(exception);
        }

        public object GetCurrentCulture()
        {
            return null;// Thread.CurrentThread.CurrentCulture;
        }

        public void SetInvariantCulture()
        {
            SetCurrentCulture(CultureInfo.InvariantCulture);
        }

        public void SetCurrentCulture(object culture)
        {
            //Thread.CurrentThread.CurrentCulture = (CultureInfo)culture;
        }
    }
}