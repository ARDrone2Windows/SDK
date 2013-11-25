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
using AR.Drone2.Sample.W8.Views;

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
            AltitudeMax.Value = _droneClient.Configuration.Control.AltitudeMax/1000;
            VitesseVerticaleMax.Value = _droneClient.Configuration.Control.ControlVzMax;
            VitesseRotationMax.Value = (int)(_droneClient.Configuration.Control.ControlYaw * 180 / System.Math.PI);
            AngleInclinaisonMax.Value = (int)(_droneClient.Configuration.Control.EulerAngleMax * 180 / System.Math.PI);

            CareneExterieureToggle.IsOn = _droneClient.Configuration.Control.FlightWithoutShell;
            ExterieurToggle.IsOn = _droneClient.Configuration.Control.Outdoor;
        }

        private async void AltitudeMax_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _droneClient.Configuration.Control.AltitudeMax = (int)(AltitudeMax.Value * 1000);
            await _droneClient.SendConfiguration();
        }

        private async void VitesseVerticaleMax_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _droneClient.Configuration.Control.ControlVzMax = (int)VitesseVerticaleMax.Value;
            await _droneClient.SendConfiguration();
        }

        private async void VitesseRotationMax_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _droneClient.Configuration.Control.ControlYaw = VitesseRotationMax.Value * System.Math.PI / 180;
            await _droneClient.SendConfiguration();
        }

        private async void AngleInclinaisonMax_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _droneClient.Configuration.Control.EulerAngleMax = AngleInclinaisonMax.Value * System.Math.PI / 180;
            await _droneClient.SendConfiguration();
        }

        private async void CareneExterieureToggle_Toggled(object sender, RoutedEventArgs e)
        {
            _droneClient.Configuration.Control.FlightWithoutShell = CareneExterieureToggle.IsOn;
            await _droneClient.SendConfiguration();
        }

        private async void ExterieurToggle_Toggled(object sender, RoutedEventArgs e)
        {
            _droneClient.Configuration.Control.Outdoor = ExterieurToggle.IsOn;
            await _droneClient.SendConfiguration();
        }

        private async void ReinitButton_Click(object sender, RoutedEventArgs e)
        {
            _droneClient.Configuration.Control.ControlVzMax = 700;
            _droneClient.Configuration.Control.ControlYaw = 100 * System.Math.PI / 180;
            _droneClient.Configuration.Control.EulerAngleMax = 12 * System.Math.PI / 180;
            
            VitesseVerticaleMax.Value = _droneClient.Configuration.Control.ControlVzMax;
            VitesseRotationMax.Value = (int)(_droneClient.Configuration.Control.ControlYaw * 180 / System.Math.PI);
            AngleInclinaisonMax.Value = (int)(_droneClient.Configuration.Control.EulerAngleMax * 180 / System.Math.PI);

            await _droneClient.SendConfiguration();
        }

        private void ShowCommandButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(XboxControls));
        }
    }
}
