using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

using ARDrone2Client.Common.Configuration;
using ARDrone2Client.Common.Configuration.Native;
using ARDrone2Client.Common.Configuration.Sections;
using ARDrone2Client.Common.Input;
using ARDrone2Client.Common.Navigation;
using ARDrone2Client.Common.Navigation.Native;
using ARDrone2Client.Common.Workers;
using ARDrone2Client.Common.Helpers;
using ARDrone2Client.Common.ViewModel;
using System.Text.RegularExpressions;
using System.IO;
using Windows.System.Threading;

namespace ARDrone2Client.Common
{
    public class DroneClient : DisposableBase
    {
        private const int AckControlAndWaitForConfirmationTimeout = 1000;
        
        private string _ApplicationId = "a1b2c3d4";
        public string ApplicationId
        {
            get
            {
                return _ApplicationId;
            }
            set
            {
                _ApplicationId = value;
            }
        }
        private string _UserId = "b2c3d4e5";
        public string UserId
        {
            get
            {
                return _UserId;
            }
            set
            {
                _UserId = value;
            }
        }
        private string _SessionId = "c3d4e5f6";
        public string SessionId
        {
            get
            {
                return _SessionId;
            }
            set
            {
                _SessionId = value;
            }
        }

        private static readonly object _SyncRoot = new object();
        private readonly CommandWorker _CommandWorker;
        private readonly ConfigurationWorker _ConfigurationWorker;
        private readonly NavDataWorker _NavDataWorker;
        private readonly WatchdogWorker _WatchdogWorker;
        private ThreadPoolTimer _InputTimer;

        private string _Host = "192.168.1.1";
        public string Host
        {
            get
            {
                return _Host;
            }
            set
            {
                _Host = value;
            }
        }
        private NavigationData _navigationData;
        internal NavigationData NavigationData
        {
            set
            {
                _navigationData = value;
            }
            get
            {
                return _navigationData;
            }
        }

        private NavigationDataViewModel _NavigationDataViewModel;
        public NavigationDataViewModel NavigationDataViewModel
        {
            get
            {
                return _NavigationDataViewModel;
            }
        }

        // Messages
        private ObservableCollection<Message> _Messages = new ObservableCollection<Message>();
        public ObservableCollection<Message> Messages
        {
            get
            {
                return _Messages;
            }
        }

        // Actions & Events
        public Action<NavigationPacket> NavigationPacketAcquired { get; set; }
        public Action<NavigationData> NavigationDataUpdated { get; set; }
        public Action<ConfigurationPacket> ConfigurationPacketAcquired { get; set; }
        public Action<DroneConfiguration> ConfigurationUpdated { get; set; }

        private static DroneClient _Instance;
        public static DroneClient Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_SyncRoot)
                    {
                        if (_Instance == null)
                            _Instance = new DroneClient();
                    }
                }
                return _Instance;
            }
        }

        public bool IsFlying
        {
            get
            {
                return NavigationData.State.HasFlag(NavigationState.Flying);
            }
        }

        public DroneClient()
        {
            _NavigationDataViewModel = new ViewModel.NavigationDataViewModel();
            _configuration = new DroneConfiguration();

            _CommandWorker = new CommandWorker(this);
            _NavDataWorker = new NavDataWorker(this);
            _ConfigurationWorker = new ConfigurationWorker(this);
            //TODO ajouter le _configurationAcquisitionWorker
            _WatchdogWorker = new WatchdogWorker(this, new WorkerBase[] { _NavDataWorker, _CommandWorker });
            _InputTimer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(InputTimerElapsedHandler), TimeSpan.FromMilliseconds(1000 / 12));
        }
        private IInputProvider _CurrentInputProvider = null;
        private int _InputProviderInactiveCount = 0;
        private void InputTimerElapsedHandler(ThreadPoolTimer timer)
        {
            if (_CurrentInputProvider != null)
            {
                _CurrentInputProvider.Update();
                if (InputState.IsEmpty())
                    _InputProviderInactiveCount++;
                if (_InputProviderInactiveCount > 10)
                {
                    _InputProviderInactiveCount = 0;
                    _CurrentInputProvider = null;
                }
            }
            else
            {
                foreach (var provider in InputProviders)
                {
                    provider.Update();
                    if (!InputState.IsEmpty())
                        _CurrentInputProvider = provider;
                }
            }
        }

        protected override void DisposeOverride()
        {
            if (_InputTimer != null)
                _InputTimer.Cancel();
            if (_WatchdogWorker != null)
                _WatchdogWorker.Dispose();
            if (_ConfigurationWorker != null)
                _ConfigurationWorker.Dispose();
            if (_NavDataWorker != null)
                _NavDataWorker.Dispose();
            if (_CommandWorker != null)
                _CommandWorker.Dispose();
        }

        private void ResetNavigationData()
        {
            _navigationData = new NavigationData();
        }

        public bool IsActive
        {
            get
            {
                var workers = new WorkerBase[] { _ConfigurationWorker, _NavDataWorker, _CommandWorker, _WatchdogWorker };
                var activeCount = 0;
                foreach (WorkerBase wb in workers)
                {
                    if (wb.IsAlive)
                    {
                        activeCount++;
                    }
                    else
                    {
                        Debug.WriteLine("DroneClient.IsAlive=false " + wb.GetType().Name);
                    }
                }
                return activeCount == workers.Length;
            }
        }
        private RequestedState _RequestedState = RequestedState.None;
        public RequestedState RequestedState
        {
            get
            {
                return _RequestedState;
            }
            set
            {
                _RequestedState = value;
            }
        }
        private readonly InputState _InputState = new InputState();
        public InputState InputState
        {
            get
            {
                return _InputState;
            }
        }
        private ProgressiveMode _ProgressiveMode = ProgressiveMode.Progressive;
        public ProgressiveMode ProgressiveMode
        {
            get
            {
                return _ProgressiveMode;
            }
            set
            {
                _ProgressiveMode = value;
            }
        }


        public async Task<bool> ConnectAsync()
        {
            try
            {
                if (IsActive)
                    return true;
                SendMessageToUI("Starting Connection process");
                await Log.Instance.WriteLineAsync("DroneClient:Connecting");
                _RequestedState = RequestedState.Initialize;
                _ConfigurationWorker.Start();
                _NavDataWorker.Start();
                //TODO revoir InitState
                _CommandWorker.Start();
                _WatchdogWorker.Start();

                // We Check if the differents workers have been well initialized
                int attempt = 0;
                while (!IsActive && attempt < 10)
                {
                    await Task.Delay(100);
                    attempt++;
                }
                if (attempt == 10)
                    return false;
                SendMessageToUI("All workers have been activated");
                attempt = 0;
                while (_RequestedState != RequestedState.None && attempt < 10) { await Task.Delay(100); attempt++; }
                if (attempt == 10)
                {
                    Close();
                    DisposeOverride();
                    return false;
                }
                SendMessageToUI("Connected with the Drone successfully");

                await InitMultiConfiguration();
                SetDefaultConfiguration();

                RequestConfiguration();
                SendMessageToUI("Configuration sent to the Drone successfully");
                return true;
            }
            catch (Exception ex)
            {
                SendMessageToUI(ex.Message);
                return false;
            }
        }

        public async void Close()
        {
            if (_ConfigurationWorker != null)
                _ConfigurationWorker.Stop();
            if (_NavDataWorker != null)
                _NavDataWorker.Stop();
            if (_CommandWorker != null)
                _CommandWorker.Stop();
            if (_WatchdogWorker != null)
                _WatchdogWorker.Stop();
            ResetNavigationData();
            await Log.Instance.WriteLineAsync("DroneClient:Closed");
        }

        private readonly DroneConfiguration _configuration;
        public DroneConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
        }

        private List<IInputProvider> _InputProviders = null;
        public List<IInputProvider> InputProviders
        {
            get
            {
                if (_InputProviders == null)
                {
                    lock (_SyncRoot)
                    {
                        if (_InputProviders == null)
                            _InputProviders = new List<IInputProvider>();
                    }
                }
                return _InputProviders;
            }
        }

        public async void Emergency()
        {
            _RequestedState = RequestedState.Emergency;
            await Log.Instance.WriteLineAsync("DroneClient:Emergency");
        }

        public async void ResetEmergency()
        {
            _RequestedState = RequestedState.ResetEmergency;
            await Log.Instance.WriteLineAsync("DroneClient:ResetEmergency");
        }

        public async void RequestConfiguration()
        {
            _RequestedState = RequestedState.GetConfiguration;
            await Log.Instance.WriteLineAsync("DroneClient:RequestConfiguration");
        }

        public async void Land()
        {
            _RequestedState = RequestedState.Land;
            await Log.Instance.WriteLineAsync("DroneClient:Land");
            SendMessageToUI("The Drone is landing");
        }

        public async void TakeOff(bool automated = true)
        {
            _RequestedState = RequestedState.TakeOff;
            await Log.Instance.WriteLineAsync("DroneClient:TakeOff");
            SendMessageToUI("The Drone is taking off");
        }

        public async void Hover()
        {
            //No command means hovering
            _RequestedState = RequestedState.None;
            await Log.Instance.WriteLineAsync("DroneClient:Hover");
        }

        public async void TakePicture()
        {
            _configuration.Userbox.Command = new UserboxCommand(UserboxCommandType.Screenshot, 0, 0, DateTime.Now);
            await SendConfiguration();

            await Log.Instance.WriteLineAsync("DroneClient:PictureTaken");
            SendMessageToUI("Picture taken successfully");
        }

        public async void StartRecordingVideo()
        {
            _configuration.Video.Codec = VideoCodecType.MP4_360P_H264_720P;
          
            //If you want to store your video on USB
            _configuration.Video.OnUsb = true;
            _configuration.Userbox.Command = new UserboxCommand(UserboxCommandType.Start, DateTime.Now);
            await SendConfiguration();

            await Log.Instance.WriteLineAsync("DroneClient:StartVideoRecording");
            SendMessageToUI("Video recording started successfully");
        }

        public async void StopRecordingVideo()
        {
            _configuration.Video.Codec = VideoCodecType.H264_720P;
            _configuration.Userbox.Command = new UserboxCommand(UserboxCommandType.Stop);
            await SendConfiguration();

            await Log.Instance.WriteLineAsync("DroneClient:StopVideoRecording");
            SendMessageToUI("Video recording stopted successfully");
        }

        public async void PlayAnimation(FlightAnimationType animationType)
        {
            _configuration.Control.FlightAnimation = new FlightAnimation(animationType, 2);
            await SendConfiguration();

            await Log.Instance.WriteLineAsync("DroneClient:PlayAnimation");
            SendMessageToUI("Animation executed successfully");
        }

        public async void PlayLedAnimation()
        {
            int ledAnimationNumber = (int)ARDRONE_LED_ANIMATION.ARDRONE_LED_ANIMATION_BLINK_ORANGE;
            float frequency = 2.0f; // in Hz.
            int duration = 2;       // in s.

            _configuration.Leds.Animation = string.Format("{0},{1},{2}", ledAnimationNumber, ConversionHelper.ToInt(frequency), duration);
            await SendConfiguration();

            await Log.Instance.WriteLineAsync("DroneClient:PlayLedAnimation");
            SendMessageToUI("Led animation executed successfully");
        }


        public async void SwitchVideoChannel()
        {
            _configuration.Video.Channel = 
                _configuration.Video.Channel == VideoChannelType.Horizontal ? VideoChannelType.Vertical : VideoChannelType.Horizontal;

            await SendConfiguration();
            await Log.Instance.WriteLineAsync("DroneClient:SwitchVideoChannel");
        }

        public async void SetDefaultConfiguration()
        {
            _configuration.General.NavdataDemo = true;
            _configuration.Video.Codec = VideoCodecType.H264_720P;

            await SendConfiguration();
            await Log.Instance.WriteLineAsync("DroneClient:SetDefaultConfiguration");
        }

        public async void SetOutdoorConfiguration()
        {
            _configuration.Control.Outdoor = true;
            _configuration.Control.FlightWithoutShell = true;
            _configuration.Control.AltitudeMax = 15000;
            
            //_configuration.Control.EulerAngleMax = 0.25f;

            await SendConfiguration();
            await Log.Instance.WriteLineAsync("DroneClient:SetOutdoorConfiguration");
        }

        public async void SetIndoorConfiguration()
        {
            _configuration.Control.Outdoor = false;
            _configuration.Control.FlightWithoutShell = false;
            _configuration.Control.AltitudeMax = 3000;

            //configuration.General.NavdataOptions = NavdataOptions.All;

            //configuration.Video.BitrateCtrlMode = VideoBitrateControlMode.Dynamic;
            //configuration.Video.Bitrate = 1000;
            //configuration.Video.MaxBitrate = 2000;

            // record video to usb
            //configuration.Video.OnUsb = true;
            // usage of MP4_360P_H264_720P codec is a requariment for video recording to usb
            //configuration.Video.Codec = VideoCodecType.MP4_360P_H264_720P;
            // start
            //configuration.Userbox.Command = new UserboxCommand(UserboxCommandType.Start);
            // stop
            //configuration.Userbox.Command = new UserboxCommand(UserboxCommandType.Stop);

            await SendConfiguration();
            await Log.Instance.WriteLineAsync("DroneClient:SetIndoorConfiguration");
        }

        private async Task InitMultiConfiguration()
        {
            // set new session, application and profile
            await AckControlAndWaitForConfirmation(); // wait for the control confirmation

            _configuration.Custom.SessionId = DroneConfiguration.NewId();
            SendChanges(_configuration);

            await AckControlAndWaitForConfirmation();

            _configuration.Custom.ProfileId = DroneConfiguration.NewId();
            SendChanges(_configuration);

            await AckControlAndWaitForConfirmation();

            _configuration.Custom.ApplicationId = DroneConfiguration.NewId();
            SendChanges(_configuration);

            await AckControlAndWaitForConfirmation();
        }

        public async Task SendConfiguration()
        {
            DroneConfiguration configuration = _configuration;

            if (string.IsNullOrEmpty(configuration.Custom.SessionId) ||
                configuration.Custom.SessionId == "00000000")
            {
                await InitMultiConfiguration();
            }

            //send all changes in one pice
            SendChanges(configuration);
        }

        public void SendChanges(DroneConfiguration configuration)
        {
            KeyValuePair<string, string> item;

            while (configuration.Changes.Count > 0)
            {
                lock (_SyncRoot)
                {
                    if (configuration.Changes.Count > 0)
                        item = configuration.Changes.Dequeue();
                }
                if (!string.IsNullOrEmpty(item.Key))
                {
                    if (string.IsNullOrEmpty(configuration.Custom.SessionId) == false &&
                        string.IsNullOrEmpty(configuration.Custom.ProfileId) == false &&
                        string.IsNullOrEmpty(configuration.Custom.ApplicationId) == false)
                        SendConfigCommand(Command.ConfigIds(configuration.Custom.SessionId, configuration.Custom.ProfileId, configuration.Custom.ApplicationId));

                    SendConfigCommand(Command.Config(item.Key, item.Value));
                }
            }
        }


        public void SendConfigCommand(string configurationCommand)
        {
            _CommandWorker.EnqueueConfigCommand(configurationCommand);
        }

        public async Task<bool> AckControlAndWaitForConfirmation()
        {
            Stopwatch swTimeout = Stopwatch.StartNew();

            try
            {
                bool ackControlSent = false;
                while (swTimeout.ElapsedMilliseconds < AckControlAndWaitForConfirmationTimeout)
                {
                    if (NavigationData.Masks.HasFlag(def_ardrone_state_mask_t.ARDRONE_COMMAND_MASK))
                    {
                        SendConfigCommand(Command.Control(ControlMode.AckControlMode));
                        ackControlSent = true;
                    }

                    if (ackControlSent && NavigationData.Masks.HasFlag(def_ardrone_state_mask_t.ARDRONE_COMMAND_MASK) == false)
                    {
                        return true;
                    }
                    await Task.Delay(5);
                }
                return false;
            }
            finally
            {
                //await Log.Instance.WriteLineAsync(string.Format("AckCommand done in {0} ms.", swTimeout.ElapsedMilliseconds));
            }
        }

        public void PostCommand(string command)
        {
            _CommandWorker.PostCommand(command);
        }

        public async void CycleProgressiveMode()
        {
            _ProgressiveMode = (ProgressiveMode)(((int)_ProgressiveMode + 1) % Enum.GetNames(typeof(ProgressiveMode)).Length);
            await Log.Instance.WriteLineAsync("DroneClient:CycleProgressive");
        }

        internal void SendMessageToUI(string message)
        {
            SmartDispatcher.Invoke(() =>
            {
                Messages.Insert(0, new Message() { Content = message });
            });
        }

        internal void FlushConfigCommands()
        {
            _CommandWorker.FlushConfigCommands();
        }
    }
}