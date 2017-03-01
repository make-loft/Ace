using System.Windows;
using Aero;
using Aero.Specific;

namespace HelloAero
{
    public partial class App
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            //LocalizationSource.Wrap.ActiveManager = English.ResourceManager;
            Memory.ActiveBox = new Memory(new KeyFileStorage());
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Store.Snapshot();
        }
    }
}
