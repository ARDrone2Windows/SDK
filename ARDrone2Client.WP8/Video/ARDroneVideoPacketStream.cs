using System;
using System.IO;

using ARDrone2VideoTcpWrapper;

namespace ARDrone2Client.WP8.Video
{
    class ARDroneVideoPacketStream : Stream
    {
        private readonly VideoPacketWrapper _videoPacket;

        public ARDroneVideoPacketStream(VideoPacketWrapper videoPacket)
        {
            _videoPacket = videoPacket;

        }
        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get { return _videoPacket.getDataLength(); }
        }

        public override long Position { get; set; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            _videoPacket.readBuffer(buffer, offset, (int)Position, count);
            Position += count;
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
