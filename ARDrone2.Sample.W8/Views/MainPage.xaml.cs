using System;
using System.Threading.Tasks;
using AR.Drone2.Sample.W8;
using ARDrone2Client.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AR.Drone2.Sample.W8.Views;
using AR.Drone2.Sample.W8.Model;

namespace ARDrone2.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel _viewModel;

        private DroneClient _droneClient;
        private DispatcherTimer _timer;

        public MainPage()
        {
            this.InitializeComponent();

            this._viewModel = DataContext as MainPageViewModel;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(this._viewModel != null)
            {
                this._viewModel.OnNavigatedTo();
            }

            _droneClient = DroneClient.Instance;

            _timer = new DispatcherTimer();
            _timer.Tick += _Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _timer.Start();
        }

        private async void _Timer_Tick(object sender, object e)
        {
            if (_droneClient.IsActive)
                return;

            try
            {
                SetConnectionStatus();
                await _droneClient.ConnectAsync();
                SetConnectionStatus();
            }
            catch (Exception)
            {
                SetConnectionStatus();
            }

            if (!_droneClient.IsActive)
                await Task.Delay(5000);
        }

        private void SetConnectionStatus()
        {
            if (_droneClient.IsActive)
            {
                this.progressConnection.Visibility = Visibility.Collapsed;
                this.imgConnection.Visibility = Visibility.Visible;
            }
            else
            {
                this.progressConnection.Visibility = Visibility.Visible;
                this.imgConnection.Visibility = Visibility.Collapsed;
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void OnConfigTapped(object sender, RoutedEventArgs routedEventArgs)
        {
            this.Frame.Navigate(typeof(ConfigurationPage));
        }

        private void GoToPilotagePageButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (PilotagePage));
        }

        private void GoToFlightPageOnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FlightPage), (FlightData)((FrameworkElement)sender).DataContext);
        }
    }
}
