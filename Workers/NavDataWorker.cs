using ARDrone2Client.Common.Configuration;
using System;
using System.Diagnostics;
using System.Text;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using ARDrone2Client.Common;
using ARDrone2Client.Common.Navigation;
using ARDrone2Client.Common.Navigation.Native;

namespace ARDrone2Client.Common.Workers
{
    public class NavDataWorker : WorkerBase
    {
        public const string _ServiceName = "5554";
        public const int KeepAliveTimeout = 200;
        public const int Timeout = 2000;
        private int consecutiveCommandMask = 0;

        private DroneClient _DroneClient;
        private Stopwatch _TimeoutStopWatch;

        //private readonly Action<NavigationPacket> _navigationPacketAcquired;
        private DatagramSocket udpClient;
        private DataWriter udpWriter;

        public NavDataWorker(DroneClient droneClient)
        {
            _DroneClient = droneClient;
        }
        
        public override async void Start()
        {
            udpClient = new DatagramSocket();
         
            // Connect To Drone
            udpClient.MessageReceived += MessageReceived;
            await udpClient.BindServiceNameAsync(_ServiceName);
            await udpClient.ConnectAsync(new HostName(DroneClient.Host), _ServiceName);
            udpWriter = new DataWriter(udpClient.OutputStream);

            SendKeepAliveSignal();
            _TimeoutStopWatch = Stopwatch.StartNew();
        }

        public override void Stop()
        {
            if (udpWriter != null)
                udpWriter.Dispose();
            if (udpClient != null)
                udpClient.Dispose();
        }
        public override bool IsAlive
        {
            get
            {
                return (_TimeoutStopWatch != null) && (_TimeoutStopWatch.ElapsedMilliseconds < Timeout);
            }
        }
        public DroneClient DroneClient
        {
            get
            {
                return _DroneClient;
            }
            set
            {
                _DroneClient = value;
            }
        }

        private void MessageReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs eventArguments)
        {
            DataReader reader;
            try
            {
                reader = eventArguments.GetDataReader();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return;
            }
            uint dataLength = reader.UnconsumedBufferLength;
            byte[] data = new byte[dataLength];
            reader.ReadBytes(data);

            var packet = new NavigationPacket
            {
                Timestamp = DateTime.UtcNow.Ticks,
                Data = data
            };
            UpdateNavigationData(packet);

            _TimeoutStopWatch.Restart();
        }
        private void UpdateNavigationData(NavigationPacket packet)
        {
            NavigationData navigationData;
            if (TryParseNavigationPacket(ref packet, out navigationData))
            {
                _DroneClient.NavigationData = navigationData;
                _DroneClient.NavigationDataViewModel.Update(navigationData);

                if (navigationData.Masks.HasFlag(def_ardrone_state_mask_t.ARDRONE_COMMAND_MASK))
                {
                    if (consecutiveCommandMask >= 5)
                    {
                        _DroneClient.FlushConfigCommands();
                        consecutiveCommandMask = 0;
                    }
                    else
                        consecutiveCommandMask++;
                }
                else
                {
                    consecutiveCommandMask = 0;
                }
                //TODO gérer les autres MASKS (LOW Battery, Too much wind) pour la gestion d'alertes

            }
        }
        private bool TryParseNavigationPacket(ref NavigationPacket packet, out NavigationData navigationData)
        {
            navigationData = new NavigationData();
            NavdataBag navdataBag;
            if (NavdataBagParser.TryParse(ref packet, out navdataBag))
            {
                navigationData = NavdataConverter.ToNavigationData(navdataBag);
                return true;
            }
            return false;
        }

        private async void SendKeepAliveSignal()
        {
            byte[] payload = BitConverter.GetBytes(1);

            udpWriter.WriteBytes(payload);
            await udpWriter.StoreAsync();
        }
    }
}