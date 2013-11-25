using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using ARDrone2Client.Common;
using ARDrone2Client.Common.Configuration;
using ARDrone2Client.Common.Input;

namespace AR.Drone2.Sample.W8.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PilotagePage
    {
        private readonly DroneClient _droneClient;
        private const string VideoSourceUrl = "ardrone://192.168.1.1";
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly DispatcherTimer _timer;
        public PilotagePage()
        {
            InitializeComponent();

            _droneClient = DroneClient.Instance;
            DataContext = _droneClient;
            DefaultViewModel["Messages"] = _droneClient.Messages;

            //Register joysticks
            if (_droneClient.InputProviders.Count == 0)
            {
                _droneClient.InputProviders.Add(new XBox360JoystickProvider(_droneClient));
                _droneClient.InputProviders.Add(new SoftJoystickProvider(_droneClient, RollPitchJoystick, YawGazJoystick));
            }

            _timer = new DispatcherTimer();
            _timer.Tick += _Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _timer.Start();
        }

        private async void _Timer_Tick(object sender, object e)
        {
            UpdateDisplay();

            await _droneClient.ConnectAsync();
            if (!_droneClient.IsActive)
                await Task.Delay(5000);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Application.Current.Suspending += OnSuspend;
            Application.Current.Resuming += OnResume;
            if (!_droneClient.IsActive)
                await _droneClient.ConnectAsync();
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            var active = _droneClient.IsActive;
            TakeOffLandButton.IsEnabled = active;
            TakeOffLandingTextBlock.Text = _droneClient.IsFlying() ? "LANDING" : "TAKE OFF";

            SwitchVideoChannelButton.IsEnabled = active;
            EmergencyButton.IsEnabled = active;
            ResetEmergencyButton.IsEnabled = active;
            IndoorOutdoorSwitch.IsEnabled = active;
            TakePictureButton.IsEnabled = active;
            PlayAnimationButton.IsEnabled = active;
            PlayLedAnimationButton.IsEnabled = active;
            FlatTrimButton.IsEnabled = active;
            StartVideoRecordingButton.IsEnabled = active;
            StopVideoRecordingButton.IsEnabled = active;
        }

        private void Page_OnLoaded(object sender, RoutedEventArgs e)
        {
            ArDroneMediaElem.Source = new Uri(VideoSourceUrl);
            UpdateDisplay();
        }

        private void OnResume(object sender, object e)
        {
            ArDroneMediaElem.Source = new Uri(VideoSourceUrl);
        }

        private async void OnSuspend(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            ArDroneMediaElem.Source = null;
            await Task.Delay(2000);
            deferral.Complete();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Application.Current.Suspending -= OnSuspend;
            Application.Current.Resuming -= OnResume;
        }

        private void SwitchVideoChannel_Click(object sender, RoutedEventArgs e)
        {
            _droneClient.SwitchVideoChannel();
            UpdateDisplay();
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);

            if (e.Pointer.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                return;

            Point pos = e.GetCurrentPoint(JoystickGrid).Position;
            var joy = pos.X < (ActualWidth / 2) ? RollPitchJoystick : YawGazJoystick;
            var bounds = new Rect(joy.ActualWidth / 2, joy.ActualHeight / 2, JoystickGrid.ActualWidth - joy.ActualWidth / 2, JoystickGrid.ActualHeight - 75 - joy.ActualHeight / 2);
            if (!bounds.Contains(pos))
                return;
            joy.Margin = new Thickness(pos.X - joy.ActualWidth / 2, pos.Y - joy.ActualHeight / 2, 0, 0);
            joy.VerticalAlignment = VerticalAlignment.Top;
            joy.HorizontalAlignment = HorizontalAlignment.Left;
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (e.Pointer.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                return;
            Point pos = e.GetCurrentPoint(JoystickGrid).Position;
            var joy = pos.X < (ActualWidth / 2) ? RollPitchJoystick : YawGazJoystick;
            joy.Margin = joy == YawGazJoystick ? new Thickness(0, 0, 80, 80) : new Thickness(80, 0, 0, 80);
            joy.VerticalAlignment = VerticalAlignment.Bottom;
            joy.HorizontalAlignment = joy == YawGazJoystick ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            joy.Reset();
        }

        private void Configuration_Click(object sender, RoutedEventArgs e)
        {
            ParametersGrid.IsOpen = !ParametersGrid.IsOpen;
        }

        private void Emergency_Click(object sender, RoutedEventArgs e)
        {
            _droneClient.Emergency();
            UpdateDisplay();
        }

        private void ResetEmergency_Click(object sender, RoutedEventArgs e)
        {
            _droneClient.ResetEmergency();
            UpdateDisplay();
        }

        private void TakePicture_Click(object sender, RoutedEventArgs e)
        {
            _droneClient.TakePicture();
            UpdateDisplay();
        }

        private void PlayAnimationButton_Click(object sender, RoutedEventArgs e)
        {
            _droneClient.PlayAnimation(FlightAnimationType.FlipLeft);
            UpdateDisplay();
        }

        private void PlayLedAnimationButton_Click(object sender, RoutedEventArgs e)
        {
            _droneClient.PlayLedAnimation();
            UpdateDisplay();
        }

        private void StartVideoRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            _droneClient.StartRecordingVideo();
            UpdateDisplay();
            StartVideoRecordingButton.Visibility = Visibility.Collapsed;
            StopVideoRecordingButton.Visibility = Visibility.Visible;

            VideoRecordingMessageGrid.IsOpen = true;
        }

        private void StopVideoRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            _droneClient.StopRecordingVideo();
            UpdateDisplay();
            StartVideoRecordingButton.Visibility = Visibility.Visible;
            StopVideoRecordingButton.Visibility = Visibility.Collapsed;

            VideoRecordingMessageGrid.IsOpen = false;
        }

        private void TakeOffLandButton_Click(object sender, RoutedEventArgs e)
        {
            if (_droneClient.IsFlying())
                _droneClient.Land();
            else
                _droneClient.TakeOff();
            UpdateDisplay();
        }
        private void ToggleLiveInfoGridButton_OnClick(object sender, RoutedEventArgs e)
        {
            LiveInfoGrid.IsOpen = !LiveInfoGrid.IsOpen;
        }

        private void OutdoorModeToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            var ts = sender as ToggleSwitch;
            if (ts == null) throw new Exception();
            if (ts.IsOn) _droneClient.SetOutdoorConfiguration();
            else _droneClient.SetIndoorConfiguration();
            UpdateDisplay();
        }

        private void VideoCroppingToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            var ts = sender as ToggleSwitch;
            if (ts == null) return;
            ArDroneMediaElem.Margin = ts.IsOn ? new Thickness(0) : new Thickness(0, 140, 0, 50);
        }

        private void ToggleLiveMessageGridButton_OnClick(object sender, RoutedEventArgs e)
        {
            LiveMessageGrid.IsOpen = !LiveMessageGrid.IsOpen;
        }

        private void FlatTrimButton_Click(object sender, RoutedEventArgs e)
        {
            _droneClient.ExecuteFlatTrim();
        }
    }
}
