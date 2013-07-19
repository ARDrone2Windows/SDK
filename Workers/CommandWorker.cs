using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Text;
using System.Threading;
using ARDrone2Client.Common.Configuration;
using ARDrone2Client.Common.Configuration.Sections;
using ARDrone2Client.Common.Helpers;
using Windows.System.Threading;
using ARDrone2Client.Common.Navigation;
using ARDrone2Client.Common;

namespace ARDrone2Client.Common.Workers
{
    public class CommandWorker : WorkerBase
    {
        private readonly object _SyncRoot = new object();
        private bool _Started = false;
        private DroneClient _DroneClient;
        public const string _ServiceName = "5556";
        //public const int KeepAliveTimeout = 500;
        private int _SequenceNumber = 0;
        private ThreadPoolTimer _Timer;
        private Queue<string> _NormalPriorityCommandQueue;
        private Queue<string> _HighPriorityCommandQueue;
        private Queue<string> _ConfigCommandList;
        private DatagramSocket udpClient;
        private DataWriter udpWriter;
        private bool isInitialized = false;

        //private StorageFile commandHistoryFile;

        public CommandWorker(DroneClient droneClient)
        {
            _DroneClient = droneClient;
            _NormalPriorityCommandQueue = new Queue<string>();
            _HighPriorityCommandQueue = new Queue<string>();
            _ConfigCommandList = new Queue<string>();
        }

        public override async void Start()
        {
            if (_Started)
                return;
            _SequenceNumber = 1;

            // Connect To Drone
            udpClient = new DatagramSocket();
            await udpClient.BindServiceNameAsync(_ServiceName);
            await udpClient.ConnectAsync(new HostName(DroneClient.Host), _ServiceName);
            udpWriter = new DataWriter(udpClient.OutputStream);

            //string path = string.Format("AR.Drone-CommandHistory_{0:yyyy-MM-dd-HH-mm}.txt", DateTime.Now);
            //commandHistoryFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(path, CreationCollisionOption.ReplaceExisting);
            // Write first message
            //byte[] firstMessage = BitConverter.GetBytes(1);
            //WriteString(firstMessage.ToString());

            udpWriter.WriteByte(1);
            await udpWriter.StoreAsync();

            _Timer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(timerElapsedHandler), TimeSpan.FromMilliseconds(25));
            _Started = true;
        }

        public override void Stop()
        {
            if (!_Started)
                return;
            _Timer.Cancel();
            _Timer = null;

            _NormalPriorityCommandQueue.Clear();
            _HighPriorityCommandQueue.Clear();
            
            if (udpWriter != null)
                udpWriter.Dispose();
            if (udpClient != null)
                udpClient.Dispose();
            _Started = false;
        }
        public override bool IsAlive
        {
            get
            {
                return true;
            }
        }
        public DroneClient DroneClient
        {
            get
            {
                return _DroneClient;
            }
        }

        internal void SendMessage(string command)
        {
            _NormalPriorityCommandQueue.Enqueue(command);
        }

        internal void EnqueueConfigCommand(string command)
        {
            _ConfigCommandList.Enqueue(command);
            Debug.WriteLine("EnqueueConfigCommand=" + command);
        }

        internal void FlushConfigCommands()
        {
            _ConfigCommandList.Clear();
        }
        
        private async void timerElapsedHandler(ThreadPoolTimer timer)
        {
            if (udpWriter == null)
                return;

            //Create commands based on input
            var input = DroneClient.InputState;
            
            if (input != null && (input.Roll != 0 || input.Pitch != 0 || input.Yaw != 0 || input.Gaz != 0))
            {
                _NormalPriorityCommandQueue.Enqueue(Command.Progressive(DroneClient.ProgressiveMode, input.Roll, input.Pitch, input.Yaw, input.Gaz));
                //DroneClient.SendMessageToUI(input.ToString());
            }
            else
                _NormalPriorityCommandQueue.Enqueue(Command.Progressive(ProgressiveMode.Hovering, 0, 0, 0, 0));

            //Create commands based on configuration
            //List<string> parameters = DroneClient.InputConfigurationCommands.GetAvailableConfigurationCommands();
            //foreach (string parameter in parameters)
            //{
            //    try
            //    {
            //        _NormalPriorityCommandQueue.Enqueue(
            //            new ConfigCommand(
            //                parameter, 
            //                DroneClient.InputConfigurationCommands.GetConfigurationCommandValue(parameter)));
            //    }
            //    catch (KeyNotFoundException) { }
            //}
            
            //Create commands based on requestedState
            Debug.WriteLine("State: " + DroneClient.NavigationData.State + ", RequestedState: " + DroneClient.RequestedState);
            var state = DroneClient.NavigationData.State;
            switch (DroneClient.RequestedState)
            {
                case RequestedState.Emergency:
                    if (state.HasFlag(NavigationState.Emergency))
                    {
                        DroneClient.RequestedState = RequestedState.None;
                    }
                    else
                    {
                        _HighPriorityCommandQueue.Enqueue(Command.Ref(RefMode.Emergency));
                    }
                    break;
                case RequestedState.ResetEmergency:
                    if (state.HasFlag(NavigationState.Emergency))
                    {
                        _NormalPriorityCommandQueue.Enqueue(Command.Ref(RefMode.ResetEmergency));
                        DroneClient.RequestedState = RequestedState.None;
                    }
                    else
                    {
                        _NormalPriorityCommandQueue.Enqueue(Command.Ref(RefMode.Emergency));
                        DroneClient.RequestedState = RequestedState.None;
                    }
                    break;
                case RequestedState.Land:
                    if (state.HasFlag(NavigationState.Landed))
                    {
                        DroneClient.RequestedState = RequestedState.None;
                    }
                    else
                    {
                        _NormalPriorityCommandQueue.Enqueue(Command.Ref(RefMode.Land));
                    }
                    break;
                case RequestedState.TakeOff:
                    if (state.HasFlag(NavigationState.Flying) || state.HasFlag(NavigationState.Hovering))
                    {
                        DroneClient.RequestedState = RequestedState.None;
                    }
                    else
                    {
                        _NormalPriorityCommandQueue.Enqueue(Command.Ref(RefMode.Takeoff));
                    }
                    break;
                case RequestedState.Initialize:
                    //TODO Check if Unkown is the right state
                    if (state.HasFlag(NavigationState.Unknown) || state.HasFlag(NavigationState.Landed))
                    {
                        //if never received at least one Navdata message fome the AR.Drone
                        if (!state.HasFlag(NavigationState.Command))
                        {
                            //Step 1                            

                            _NormalPriorityCommandQueue.Enqueue(DroneClient.Configuration.General.NavdataDemo.Set(true).ToCommand());
                            DroneClient.SendMessageToUI("Trying to initialize Drone connection - Step 1");
                        }
                        else
                        {
                            //Step 2    
                            if (!isInitialized)
                            {
                                DroneClient.SendMessageToUI("Trying to initialize Drone connection - Step 2");
                                _NormalPriorityCommandQueue.Enqueue(Command.Control(ControlMode.AckControlMode));
                                isInitialized = true;
                            }
                            //Step 3
                            else
                            {
                                DroneClient.RequestedState = RequestedState.None;
                            }
                        }
                    }
                    break;
                case RequestedState.GetConfiguration:
                    //We send AT*CTRL with ControlMode.AckControlMode bit until NavData reception
                    _NormalPriorityCommandQueue.Enqueue(Command.Control(ControlMode.CfgGetControlMode));
                    break;
                case RequestedState.None:
                    break;
            }
            string command = string.Empty;
            var buffer = new StringBuilder();
            if (_ConfigCommandList.Count > 0)
                _ConfigCommandList.Enqueue(Command.Control(ControlMode.AckControlMode));
            while (_HighPriorityCommandQueue.Count > 0 || _NormalPriorityCommandQueue.Count > 0 || _ConfigCommandList.Count > 0)
            {
                lock (_SyncRoot)
                {
                    if (_HighPriorityCommandQueue.Count > 0)
                        command = _HighPriorityCommandQueue.Dequeue();
                    else if (_NormalPriorityCommandQueue.Count > 0)
                        command = _NormalPriorityCommandQueue.Dequeue();
                    else if (_ConfigCommandList.Count > 0)
                        command = _ConfigCommandList.Dequeue();
                }
                if (!string.IsNullOrEmpty(command))
                {
                    buffer.Append(command.Replace(Command.SequenceArg, _SequenceNumber.ToString()));
                    _SequenceNumber++;
                }
            }
            if (buffer.Length == 0)
                return;
            //TODO am�liorer le code pour g�rer la fermeture pr�matur�e du socket
            try
            {
                // We send the command
                Debug.WriteLine("Buffer: " + buffer.ToString());
                udpWriter.WriteBytes(Encoding.UTF8.GetBytes(buffer.ToString()));
                await udpWriter.StoreAsync();
                //await ARDrone2Client.Common.Log.Instance.WriteAsync(buffer.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            //A faire en cas state data bootstrap
            //TODO regarder si c'est vraiment n�cessaire
            // envoyer ProgressiveCommand(ProgressiveMode.Hovering, 0, 0, 0, 0) semble suffisant
            if (false != true)
            {
                // We send a keep alive command
                //_NormalPriorityCommandQueue.Enqueue(Command.Watchdog().Replace(Command.SequenceArg, _SequenceNumber.ToString()));
            }
        }
    }
}