using System;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.Storage;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ARDrone2Client.Common.FTP
{
    public class FTPClient
    {
        #region Variables
        private static uint BUFFER_SIZE = 512;
        private bool _Verbose = false;
        private string _HostName = string.Empty;
        private string _ServiceName = "21";
        private string _WorkingDirectory = @"\";
        private string _Username = "anonymous";
        private string _Password = string.Empty;
        private FtpResponse _LastResponse = new FtpResponse();
        private bool _LoggedIn = false;
        private StreamSocket ftpSocket = null;
        private int _TimeOut = 10;
        #endregion

        #region Fields
        public bool Verbose
        {
            get
            {
                return _Verbose;
            }
            set
            {
                _Verbose = value;
            }
        }
        public string ServiceName
        {
            get
            {
                return _ServiceName;
            }
            set
            {
                _ServiceName = value;
            }
        }
        public int TimeOut
        {
            get
            {
                return _TimeOut;
            }
            set
            {
                _TimeOut = value;
            }
        }
        public bool LoggedIn
        {
            get
            {
                return _LoggedIn;
            }
            private set
            {
                _LoggedIn = value;
            }
        }
        public string HostName
        {
            get
            {
                return _HostName;
            }
            set
            {
                _HostName = value;
            }
        }
        public string WorkingDirectory
        {
            get
            {
                return _WorkingDirectory;
            }
            private set
            {
                _WorkingDirectory = value;
            }

        }
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
            }
        }
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
            }
        }
        public bool IsLoggedIn
        {
            get
            {
                return _LoggedIn;
            }
        }
        public FtpResponse LastResponse
        {
            get
            {
                return _LastResponse;
            }
            private set
            {
                _LastResponse = value;
            }
        }
        #endregion

        public async Task<bool> LoginAsync()
        {
            if (LoggedIn)
            {
                await CloseConnectionAsync();
            }
            FtpResponse response;
            Debug.WriteLine("Opening connection to " + HostName, "FtpClient");
            try
            {
                ftpSocket = new StreamSocket();
                await ftpSocket.ConnectAsync(new HostName(HostName), ServiceName, SocketProtectionLevel.PlainSocket);
            }
            catch (Exception ex)
            {
                ftpSocket.Dispose();
                ftpSocket = null;
                throw new FtpException("Couldn't connect to remote server", ex);
            }
            response = await ReadResponseAsync();
            if (response.ReturnCode != 220)
            {
                await CloseConnectionAsync();
                throw new FtpException(LastResponse);
            }
            response = await ExecuteAsync("USER " + Username);
            if (!(response.ReturnCode == 331 || response.ReturnCode == 230))
            {
                Disconnect();
                throw new FtpException(LastResponse);
            }
            if (response.ReturnCode != 230)
            {
                response = await ExecuteAsync("PASS " + _Password);
                if (!(response.ReturnCode == 230 || response.ReturnCode == 202))
                {
                    Disconnect();
                    throw new FtpException(LastResponse);
                }
            }
            LoggedIn = true;
            Debug.WriteLine("Connected to " + HostName, "FtpClient");
            return true;
        }

        public async Task CloseConnectionAsync()
        {
            Debug.WriteLine("Closing connection to " + _HostName, "FtpClient");
            if (ftpSocket != null)
            {
                await ExecuteAsync("QUIT");
            }
            Disconnect();
        }

        public async Task<IList<string>> ListFilesAsync(string mask = "")
        {
            CheckLoggedIn();
            var response = await ExecuteAsync("TYPE I");
            response.CheckReturnCode(200);
            StreamSocket dataSocket = await OpenSocketForTransferAsync();
            var command = "NLST";
            if (!string.IsNullOrEmpty(mask))
                command += " " + mask;
            response = await ExecuteAsync(command);
            response.CheckReturnCode(150, 125);
            var buffer = new StringBuilder();
            DateTime timeout = DateTime.Now.AddSeconds(TimeOut);
            while (timeout > DateTime.Now)
            {
                var reader = new DataReader(dataSocket.InputStream);
                reader.InputStreamOptions = InputStreamOptions.Partial;
                var count = await reader.LoadAsync(BUFFER_SIZE);
                buffer.Append(reader.ReadString(count));
                if (count < BUFFER_SIZE)
                    break;
            }
            dataSocket.Dispose();
            var bufferArray = buffer.ToString().Split(new string[] { "\r\n" }, int.MaxValue, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>(bufferArray.Length);
            response = await ReadResponseAsync();
            if (response.ReturnCode == 226)
            {
                for (var i = 0; i < bufferArray.Length; i++)
                    result.Add(bufferArray[i]);
            }
            return result;
        }

        public async Task<ulong> GetFileSizeAsync(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullException("A directory name wasn't provided. Please provide one and try your request again.");
            }
            CheckLoggedIn();
            var response = await ExecuteAsync("SIZE " + file);
            ulong fileSize = 0;
            if (response.ReturnCode == 213)
            {
                fileSize = ulong.Parse(response.Message);
            }
            else
            {
                throw new FtpException(response.Raw);
            }
            return fileSize;
        }

        public void CheckLoggedIn([CallerMemberName]string memberName = "")
        {
            if (!LoggedIn)
            {
                throw new FtpException(string.Format("You need to log in before you can perform this operation ({0})", memberName));
            }
        }

        public async Task<bool> DownloadFileAsync(string ftpFile, string localFile, Boolean resume)
        {
            CheckLoggedIn();
            var response = await ExecuteAsync("TYPE I");
            response.CheckReturnCode(200);
            Debug.WriteLine("Downloading file " + ftpFile + " from " + HostName + "/" + WorkingDirectory, "FtpClient");
            if (string.IsNullOrEmpty(localFile))
            {
                localFile = ftpFile;
            }
            var tempFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(localFile, CreationCollisionOption.OpenIfExists);
            var tempStream = await tempFile.OpenStreamForWriteAsync();
            StreamSocket dataSocket = await OpenSocketForTransferAsync();
            ulong resumeOffset = 0;
            if (resume)
            {
                var prop = await tempFile.GetBasicPropertiesAsync();
                resumeOffset = prop.Size;
                if (resumeOffset > 0)
                {
                    response = await ExecuteAsync("REST " + resumeOffset);
                    if (response.ReturnCode != 350)
                    {
                        resumeOffset = 0;
                        Debug.WriteLine("Resuming not supported:" + response.Message, "FtpClient");
                    }
                    else
                    {
                        Debug.WriteLine("Resuming at offset " + resumeOffset, "FtpClient");
                        tempStream.Seek(Convert.ToInt64(resumeOffset), SeekOrigin.Begin);
                    }
                }
            }
            response = await ExecuteAsync("RETR " + ftpFile);
            response.CheckReturnCode(150, 125);
            var reader = new DataReader(dataSocket.InputStream);
            reader.InputStreamOptions = InputStreamOptions.Partial;
            var writer = new DataWriter(tempStream.AsOutputStream());
            DateTime timeout = DateTime.Now.AddSeconds(TimeOut);
            while (timeout > DateTime.Now)
            {
                var count = await reader.LoadAsync(BUFFER_SIZE);
                var bytes = reader.ReadBuffer(count);
                writer.WriteBuffer(bytes);
                await writer.StoreAsync();
                if (count < BUFFER_SIZE)
                {
                    break;
                }
            }
            await tempStream.FlushAsync();
            dataSocket.Dispose();
            response = await ReadResponseAsync();
            response.CheckReturnCode(226, 250);
            var props = await tempFile.GetBasicPropertiesAsync();
            var tempSize = props.Size;
            var size = await GetFileSizeAsync(ftpFile);
            if (tempSize != size)
                return false;
            var destFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(localFile, CreationCollisionOption.ReplaceExisting);
            var destStream = await destFile.OpenStreamForWriteAsync();
            tempStream.Seek(0, SeekOrigin.Begin);
            await tempStream.CopyToAsync(destStream);
            await destStream.FlushAsync();
            await tempFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            return true;
        }

        public async Task<FtpResponse> DeleteFileAsync(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullException("A directory name wasn't provided. Please provide one and try your request again.");
            }
            CheckLoggedIn();
            var response = await ExecuteAsync("DELE " + file);
            response.CheckReturnCode(250);
            Debug.WriteLine("Deleted file " + file, "FtpClient");
            LastResponse = response;
            return response;
        }

        public async Task<FtpResponse> ChangeWorkingDirectoryAsync(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentNullException("A directory name wasn't provided. Please provide one and try your request again.");
            }
            CheckLoggedIn();
            var response = await ExecuteAsync("CWD " + directory);
            response.CheckReturnCode(250);
            await PrintWorkingDirectoryAsync();
            Debug.WriteLine("Current directory is " + WorkingDirectory, "FtpClient");
            LastResponse = response;
            return response;
        }

        public async Task<FtpResponse> PrintWorkingDirectoryAsync()
        {
            CheckLoggedIn();
            var response = await ExecuteAsync("PWD");
            response.CheckReturnCode(257);
            WorkingDirectory = response.Message.Split('"')[1];
            Debug.WriteLine("Current directory is " + WorkingDirectory, "FtpClient");
            LastResponse = response;
            return response;
        }

        private async Task<FtpResponse> ReadResponseAsync()
        {
            FtpResponse response = new FtpResponse();
            var responseBuffer = new StringBuilder();
            var reader = new DataReader(ftpSocket.InputStream);
            reader.InputStreamOptions = InputStreamOptions.Partial;
            while (true)
            {
                var loadedSize = await reader.LoadAsync(BUFFER_SIZE);
                if (loadedSize > 0)
                {
                    responseBuffer.Append(reader.ReadString(loadedSize));
                    if (loadedSize < BUFFER_SIZE)
                        break;
                }
                else
                {
                    break;
                }
            }
            response.Raw = responseBuffer.ToString();
            string[] msg = response.Raw.Split('\n');
            var re = new Regex(@"^(?<rc>\d{3})\s{1}(?<message>.*)\r");
            for (int i = 0; i < msg.Length - 1; i++)
            {
                var match = re.Match(msg[i]);
                if (match != null)
                {
                    response.ReturnCode = int.Parse(match.Groups["rc"].Value);
                    response.Message = match.Groups["message"].Value;
                    break;
                }
            }
            if (Verbose)
            {
                for (int i = 0; i < msg.Length - 1; i++)
                {
                    Debug.WriteLine(msg[i], "FtpClient");
                }
            }
            LastResponse = response;
            return response;
        }

        private async Task<FtpResponse> ExecuteAsync(String msg)
        {
            if (Verbose)
                Debug.WriteLine(msg, "FtpClient");
            var writer = new DataWriter(ftpSocket.OutputStream);
            writer.WriteString(msg + "\r\n");
            await writer.StoreAsync();
            writer.DetachStream();
            var response = await ReadResponseAsync();
            LastResponse = response;
            return response;
        }

        private async Task<StreamSocket> OpenSocketForTransferAsync()
        {
            var response = await ExecuteAsync("PASV");
            string host = HostName;
            string port = string.Empty;
            if (response.ReturnCode == 227)
            {
                var re = new Regex(@"^(?<rc>\d{3})\s{1}(?<message>.*\((?<ip1>\d*)\,(?<ip2>\d*)\,(?<ip3>\d*)\,(?<ip4>\d*)\,(?<port1>\d*)\,(?<port2>\d*)\))\r");
                var match = re.Match(response.Raw);
                if (match == null)
                    return null;
                host = string.Format("{0}.{1}.{2}.{3}", match.Groups["ip1"].Value, match.Groups["ip2"].Value, match.Groups["ip3"].Value, match.Groups["ip4"].Value);
                var portNumber = byte.Parse(match.Groups["port1"].Value) * 256 + byte.Parse(match.Groups["port2"].Value);
                port = portNumber.ToString();
            }
            else
            {
                response = await ExecuteAsync("EPSV");
                response.CheckReturnCode(229);
                var re = new Regex(@"^(?<rc>\d{3})\s{1}(?<message>.*\(\|(?<d1>\d*)\|(?<d2>\d*)\|(?<port>\d*)\|\))\r");
                var match = re.Match(response.Raw);
                if (match == null)
                    return null;
                port = match.Groups["port"].Value;
            }
            var tranferSocket = new StreamSocket();
            try
            {
                await tranferSocket.ConnectAsync(new HostName(host), port);
            }
            catch (Exception ex)
            {
                tranferSocket.Dispose();
                throw new FtpException("Can't connect to remote server", ex);
            }
            return tranferSocket;
        }

        public void Disconnect()
        {
            LoggedIn = false;
            if (ftpSocket != null)
            {
                ftpSocket.Dispose();
                ftpSocket = null;
            }
        }

        ~FTPClient()
        {
            Disconnect();
        }
    }
}
