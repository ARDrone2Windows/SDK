using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using ARDrone2Client.Common;
using ARDrone2Client.Common.Configuration;
using ARDrone2Client.Common.Input;
using ARDrone2Client.Common.Navigation;
using ARDrone2Client.Common.ViewModel;

namespace ARDrone2.Sample.WP8
{
    public partial class ConfigurationPage : PhoneApplicationPage
    {
        private DroneClient _droneClient;
        
        public ConfigurationPage()
        {
            InitializeComponent();

            _droneClient = DroneClient.Instance;
            this.DataContext = _droneClient;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (await _droneClient.ConnectAsync())
            {
                int altitudeMax = _droneClient.Configuration.Control.AltitudeMax;
                AltitudeMax.Value = altitudeMax / 1000;
            }
        }

        private async void send_button_Click(object sender, EventArgs e)
        {
            int newValue = (int)AltitudeMax.Value * 1000;
            _droneClient.Configuration.Control.AltitudeMax = newValue;
            await _droneClient.SendConfiguration();

            OKPanel.Visibility = System.Windows.Visibility.Visible;
        }
    }
}