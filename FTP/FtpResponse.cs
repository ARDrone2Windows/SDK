using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARDrone2Client.Common.FTP
{
    public class FtpResponse
    {
        private int _ReturnCode;

        public FtpResponse()
        {

        }
        public FtpResponse(string raw)
        {
            _Raw = raw;
        }
        public int ReturnCode
        {
            get { return _ReturnCode; }
            set { _ReturnCode = value; }
        }
        private string _Message;

        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }
        private string _Raw;

        public string Raw
        {
            get { return _Raw; }
            set { _Raw = value; }
        }
        public void CheckReturnCode(params int[] expectedReturnCode)
        {
            for (var i = 0; i < expectedReturnCode.Length; i++)
            {
                if (expectedReturnCode[i] == ReturnCode)
                {
                    return;
                }
            }
            throw new FtpException(Message);
        }
    }
}
