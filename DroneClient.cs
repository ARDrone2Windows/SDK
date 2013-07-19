using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace ARDrone2Client.Common
{
    public class DroneClient : DisposableBase
    {
        private string ApplicationId = "a1b2c3d4";
        private string UserId = "b2c3d4e5";
        private string SessionId = "c3d4e5f6";

        private static readonly object _SyncRoot = new object();
        private readonly CommandWorker _CommandWorker;
        private readonly ConfigurationWorker _ConfigurationWorker;
        private readonly NavDataWorker _NavDataWorker;
        private readonly WatchdogWorker _WatchdogWorker;

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

        private List<ConfigurationSectionViewModel> _ConfigurationSectionsViewModel;
        public List<ConfigurationSectionViewModel> ConfigurationSectionsViewModel
        {
            get
            {
                return _ConfigurationSectionsViewModel;
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
            _ConfigurationSectionsViewModel = ConfigurationViewModelHelper.InitializeConfigurationSections(_configuration);

            _CommandWorker = new CommandWorker(this);
            _NavDataWorker = new NavDataWorker(this);
            _ConfigurationWorker = new ConfigurationWorker(this);
            //TODO ajouter le _configurationAcquisitionWorker
            _WatchdogWorker = new WatchdogWorker(this, new WorkerBase[] { _NavDataWorker, _CommandWorker });
        }

        protected override void DisposeOverride()
        {
            _WatchdogWorker.Dispose();
            _ConfigurationWorker.Dispose();
            _NavDataWorker.Dispose();
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
                        Debug.WriteLine(wb.GetType().Name);
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
                InitConfiguration();
                SetIndoorConfiguration();
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

        public void Close()
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
        //private void OnNavigationPacketAcquired(NavigationPacket packet)
        //{
        //    if (NavigationPacketAcquired != null)
        //        NavigationPacketAcquired(packet);
        //    //UpdateNavigationData(packet);
        //}

        public void Emergency()
        {
            _RequestedState = RequestedState.Emergency;
        }

        public void ResetEmergency()
        {
            _RequestedState = RequestedState.ResetEmergency;
        }

        public void RequestConfiguration()
        {
            _RequestedState = RequestedState.GetConfiguration;
        }

        public void Land()
        {
            _RequestedState = RequestedState.Land;
            SendMessageToUI("The Drone is landing");
        }

        public void TakeOff(bool automated = true)
        {
            _RequestedState = RequestedState.TakeOff;
            SendMessageToUI("The Drone is taking off");
        }

        public void Hover()
        {
            //No command means hovering
            _RequestedState = RequestedState.None;
        }
        private VideoChannelType vc = VideoChannelType.Next;
        public void SwitchVideoChannel()
        {
            //SetConfiguration(_configuration.Video.Channel.Set(VideoChannelType.Next).ToCommand());
            //vc = VideoChannelType.Next;
            vc = vc == VideoChannelType.Horizontal ? VideoChannelType.Vertical : VideoChannelType.Horizontal;

            lock (_SyncRoot)
            {
                //_CommandWorker.EnqueueConfigCommand(Command.ConfigIds(ApplicationId, UserId, SessionId));
                var switchVideo = _configuration.Video.Channel.Set(vc).ToCommand();
                _CommandWorker.EnqueueConfigCommand(switchVideo);
            }
        }
        
        private void InitConfiguration()
        {
            //SetConfiguration(_configuration.Custom.ApplicationId.Set(ApplicationId).ToCommand());
            //SetConfiguration(_configuration.Custom.ProfileId.Set(UserId).ToCommand());
            //SetConfiguration(_configuration.Custom.SessionId.Set(SessionId).ToCommand());
        }

        public void SetOutdoorConfiguration()
        {
            SetConfiguration(_configuration.Control.outdoor.Set(true).ToCommand());
            SetConfiguration(_configuration.Control.flight_without_shell.Set(true).ToCommand());
            SetConfiguration(_configuration.Control.altitude_max.Set(15000).ToCommand());
            //SetConfiguration(_configuration.Control.euler_angle_max.Set(0.25f).ToCommand());
        }

        public void SetIndoorConfiguration()
        {
            SetConfiguration(_configuration.Control.outdoor.Set(false).ToCommand());
            SetConfiguration(_configuration.Control.flight_without_shell.Set(false).ToCommand());
            SetConfiguration(_configuration.Control.altitude_max.Set(3000).ToCommand());
            //SetConfiguration(_configuration.Control.euler_angle_max.Set(0.25f).ToCommand());

            //TODO gérer une commande AT*CONFIG_IDS avant d'envoyer la commande dessous
            //SetConfiguration(_configuration.Video.Codec.Set((int)ARDRONE_VIDEO_CODEC.ARDRONE_VIDEO_CODEC_UVLC).ToCommand());
        }

        public void SetConfiguration(string command)
        {
            _CommandWorker.EnqueueConfigCommand(command);
        }

        public void CycleProgressiveMode()
        {
            _ProgressiveMode = (ProgressiveMode)(((int)_ProgressiveMode + 1) % Enum.GetNames(typeof(ProgressiveMode)).Length);
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