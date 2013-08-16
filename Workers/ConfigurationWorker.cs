using System;
using System.Diagnostics;
using System.Text;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Streams = Windows.Storage.Streams;
using System.Threading;
using System.Threading.Tasks;
using ARDrone2Client.Common.Configuration;
using Windows.System.Threading;
using ARDrone2Client.Common;
using ARDrone2Client.Common.ViewModel;
using System.IO;
using System.Text.RegularExpressions;

namespace ARDrone2Client.Common.Workers
{
    public class ConfigurationWorker : WorkerBase
    {
        private const string _ServiceName = "5559";
        private StreamSocket socket = null;
        private const int NetworkBufferSize = 65535;
        private const int ConfigTimeout = 1000;
        private DroneClient _DroneClient;

        public ConfigurationWorker(DroneClient droneClient)
        {
            _DroneClient = droneClient;
        }
        public DroneClient DroneClient
        {
            get
            {
                return _DroneClient;
            }
        }
        public override bool IsAlive
        {
            get
            {
                return true;
            }
        }
        public override void Start()
        {
            Loop();
        }

        public override void Stop()
        {
            if (socket != null)
                socket.Dispose();
        }

        protected override async void Loop()
        {
            Streams.DataReader reader = null;

            try
            {
                while (true)
                {
                    if (socket == null)
                    {
                        socket = new StreamSocket();
                        await socket.ConnectAsync(new HostName(DroneClient.Host), _ServiceName);
                    }

                    var readBuf = new Streams.Buffer((uint)NetworkBufferSize);
                    var readOp = socket.InputStream.ReadAsync(readBuf, (uint)NetworkBufferSize, Streams.InputStreamOptions.Partial);

                    readOp.Completed = (IAsyncOperationWithProgress<Streams.IBuffer, uint> asyncAction, AsyncStatus asyncStatus) =>
                    {
                        switch (asyncStatus)
                        {
                            case AsyncStatus.Completed:
                                Debug.WriteLine("Config:Completed ");
                                try
                                {
                                    Streams.IBuffer localBuf = asyncAction.GetResults();
                                    uint bytesRead = localBuf.Length;
                                    Debug.WriteLine("Config:Buffer (" + bytesRead + ")");
                                    reader = Streams.DataReader.FromBuffer(localBuf);
                                    OnDataReadCompletion(bytesRead, reader);
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e.ToString());
                                }
                                break;
                            case AsyncStatus.Canceled:
                                Debug.WriteLine("Config:Canceled ");
                                break;
                            case AsyncStatus.Error:
                                Debug.WriteLine("Config:Error ");
                                break;
                        }
                    };
                    //socket.Dispose();
                    await Task.Delay(500);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Stop();
            }
        }

        private void OnDataReadCompletion(uint bytesRead, Streams.DataReader reader)
        {
            if (reader == null)
            {
                return;
            }

            uint unreadLength = reader.UnconsumedBufferLength;
            if (unreadLength == 0)
            {
                return;
            }

            byte[] buffer = new byte[unreadLength];
            reader.ReadBytes(buffer);

            StringBuilder stringBuffer = new StringBuilder();
            stringBuffer.Append(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
            Debug.WriteLine("Configuration Brute: " + stringBuffer.ToString());

            var packet = new ConfigurationPacket
            {
                Timestamp = DateTime.UtcNow.Ticks,
                Data = buffer
            };
            if (UpdateConfiguration(packet))
            {
                if (_DroneClient.RequestedState == RequestedState.GetConfiguration)
                    _DroneClient.RequestedState = RequestedState.None;
                ConfigurationViewModelHelper.UpdateConfigurationSections(_DroneClient.ConfigurationSectionsViewModel, _DroneClient.Configuration);
            }
        }
        internal bool UpdateConfiguration(ConfigurationPacket packet)
        {
            using (var ms = new MemoryStream(packet.Data))
            {
                using (var sr = new StreamReader(ms))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        UpdateConfigurationLine(line);
                    }
                }
            }
            return true;
        }
        internal bool UpdateConfigurationLine(string line)
        {
            Regex _reKeyValue = new Regex(@"(?<key>\w+:\w+) = (?<value>.*)");
            var result = false;
            Match match = _reKeyValue.Match(line);
            if (match.Success)
            {
                string key = match.Groups["key"].Value;
                IConfigurationItem item;
                if (_DroneClient.Configuration.Items.TryGetValue(key, out item))
                {
                    string value = match.Groups["value"].Value;
                    if (item.TryUpdate(value))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
        private void OnConfigurationPacketAcquired(ConfigurationPacket packet)
        {
            if (UpdateConfiguration(packet))
            {
                if (_DroneClient.RequestedState == RequestedState.GetConfiguration)
                    _DroneClient.RequestedState = RequestedState.None;
                ConfigurationViewModelHelper.UpdateConfigurationSections(_DroneClient.ConfigurationSectionsViewModel, _DroneClient.Configuration);
            }
        }
    }
}