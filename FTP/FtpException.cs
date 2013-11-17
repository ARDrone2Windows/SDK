using System;

namespace ARDrone2Client.Common.FTP
{
    public class FtpException : Exception
    {
        public FtpException(string message)
            : base(message)
        {
        }
        public FtpException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public FtpException(FtpResponse response)
            : base(response.Message)
        {
        }
        public FtpException(FtpResponse response, Exception innerException)
            : base(response.Message, innerException)
        {
        }
    }
}
