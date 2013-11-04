using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using ARDrone2Client.Common;
using ARDrone2Client.Common.Configuration;
using ARDrone2Client.Common.Input;
using ARDrone2Client.Common.Navigation;
using ARDrone2Client.Common.ViewModel;

namespace ARDrone2.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConfigurationPage : ARDrone2.Sample.Common.LayoutAwarePage
    {
        private DroneClient _droneClient;
        public ConfigurationPage()
        {
            this.InitializeComponent();
            _droneClient = DroneClient.Instance;
            this.DataContext = _droneClient;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
                //CollectionViewSource source = new CollectionViewSource();
                //source.Source = _droneClient.ConfigurationSectionsViewModel;
                //source.IsSourceGrouped = true;
                //source.ItemsPath = new PropertyPath("ConfigItems");
                //configItems.SetBinding(GridView.ItemsSourceProperty, new Binding() { Source = source });
                
                int altitudeMax = _droneClient.Configuration.Control.AltitudeMax;
                AltitudeMax.Maximum = 100;
                AltitudeMax.Value = altitudeMax / 1000;
                AltitudeMax.StepFrequency = Math.Round(AltitudeMax.Maximum / 100, 1);
            }

        private async void AltitudeMax_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            int newValue = (int)AltitudeMax.Value * 1000;
            _droneClient.Configuration.Control.AltitudeMax = newValue;
            await _droneClient.SendConfiguration();
        }
    }
}
