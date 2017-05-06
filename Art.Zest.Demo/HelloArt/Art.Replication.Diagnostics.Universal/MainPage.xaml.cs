using System;
using System.Diagnostics;
using System.Linq;
using Windows.Devices.Enumeration;
using Art.Serialization.Serializers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Art.Replication.Diagnostics.Universal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            Loaded += async (sender, args) =>
            {
                var t = (await DeviceInformation.FindAllAsync()).ToList();
                var w = new Stopwatch();
                w.Start();
                var s = t.CreateSnapshot();
                w.Stop();
                w.Start();
                var j = s.ToString();
                w.Stop();
                var i = s.CreateInstance();
                i = i;
                j = j;
            };
        }
    }
}
