using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Media;

using ARDrone2VideoTcpWrapper;

namespace ARDrone2Client.WP8.Video
{
    public class ARDroneStreamSource : MediaStreamSource
    {
        private MediaStreamDescription _mediaStreamDescription;
        private bool _stopped;
        private readonly ARDroneTcpClientWrapper _tcpCLient;

        private Dictionary<MediaSampleAttributeKeys, string> _iSampleAttrs = new Dictionary<MediaSampleAttributeKeys, string>
            {
                {MediaSampleAttributeKeys.KeyFrameFlag, "1"}
            };

        private Dictionary<MediaSampleAttributeKeys, string> _pSampleAttrs = new Dictionary<MediaSampleAttributeKeys, string>
            {
                {MediaSampleAttributeKeys.KeyFrameFlag, "0"}
            };
        public ARDroneStreamSource(string hostName)
        {
            _tcpCLient = new ARDroneTcpClientWrapper(hostName);
        }
        protected override void OpenMediaAsync()
        {

            _mediaStreamDescription = new MediaStreamDescription(MediaStreamType.Video,
                                                                    new Dictionary<MediaStreamAttributeKeys, string>
                                                                         {
                                                                             {
                                                                                 MediaStreamAttributeKeys.CodecPrivateData,
                                                                                 "00000001674D401E965201405FF2E020100000000168EF3880"
                                                                             },
                                                                             {
                                                                                 MediaStreamAttributeKeys.VideoFourCC,
                                                                                 "H264"
                                                                             },
                                                                             {MediaStreamAttributeKeys.Width, "640"},
                                                                             {MediaStreamAttributeKeys.Height, "360"},
                                                                         });
            AudioBufferLength = 15;
            _tcpCLient.Start(succeeded =>
            {

                Task.Run(() =>
                {
                    this.ReportOpenMediaCompleted(new Dictionary<MediaSourceAttributesKeys, string>
                        {
                          {  MediaSourceAttributesKeys.CanSeek, "0"},
                          {MediaSourceAttributesKeys.Duration, "0"}
                        },
                        new List<MediaStreamDescription>{ _mediaStreamDescription });
                    BeginPopSamples();
                });
            });
        }

        private LinkedList<VideoPacketWrapper> _pendingPackets = new LinkedList<VideoPacketWrapper>();
        private int _waitingForPacket = 0;
        object _syncRoot = new object();
        private void BeginPopSamples()
        {
            while (!_stopped)
            {
                var frame = _tcpCLient.PopVideoPacketSync();
                lock (_syncRoot)
                {
                    if (_waitingForPacket > 0)
                    {
                        --_waitingForPacket;
                        long ts = frame.getTimeStamp();
                        ts *= 10000;
                        var attrs = frame.isFrameTypeI() ? _iSampleAttrs : _pSampleAttrs;
                        attrs[MediaSampleAttributeKeys.FrameWidth] = frame.getWidth().ToString();
                        attrs[MediaSampleAttributeKeys.FrameHeight] = frame.getHeight().ToString();

                        Debug.WriteLine("PopSamples frame reporting");
                        ReportGetSampleCompleted(new MediaStreamSample(_mediaStreamDescription,
                                                                       new ARDroneVideoPacketStream(frame),
                                                                       0,
                                                                       frame.getDataLength(),
                                                                       ts, attrs));
                    }
                    else
                    {
                        Debug.WriteLine("PopSamples frame queueing");
                        _pendingPackets.AddLast(frame);
                    }
                }
            }
        }


        protected override void SeekAsync(long seekToTime)
        {
            ReportSeekCompleted(seekToTime);
        }

        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            lock (_syncRoot)
            {
                if (_pendingPackets.Count > 0)
                {
                    var frame = _pendingPackets.First.Value;
                    _pendingPackets.RemoveFirst();
                    long ts = frame.getTimeStamp();
                    ts *= 10000;
                    var attrs = frame.isFrameTypeI() ? _iSampleAttrs : _pSampleAttrs;
                    attrs[MediaSampleAttributeKeys.FrameWidth] = frame.getWidth().ToString();
                    attrs[MediaSampleAttributeKeys.FrameHeight] = frame.getHeight().ToString();
                    Debug.WriteLine("GetSample completed");
                    ReportGetSampleCompleted(new MediaStreamSample(_mediaStreamDescription,
                                                                   new ARDroneVideoPacketStream(frame),
                                                                   0,
                                                                   frame.getDataLength(),
                                                                   ts, attrs));
                }
                else
                {
                    ++_waitingForPacket;

                    Debug.WriteLine("GetSample progress");
                    ReportGetSampleProgress(0);
                }
            }
        }

        protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
        {
        }

        protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
        {
        }

        protected override void CloseMedia()
        {
            _stopped = true;
            _tcpCLient.Stop();
        }
    }
}
