using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace ARDrone2Client.Common
{
    public class Log
    {
        private static readonly object _SyncRoot = new object();
        private static readonly SemaphoreSlim _Semaphore = new SemaphoreSlim(1);
        static Log()
        {
            _DefaultFileName = string.Format("{0:yyyy-MM-dd HH-mm-ss}.log", DateTime.Now);
        }
        public Log()
        {
            _FileName = DefaultFileName;
        }
        private static Log _Instance = null;
        public static Log Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_SyncRoot)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new Log();
                        }
                    }
                }
                return _Instance;
            }
        }
        private static bool _Enabled = false;
        public static bool Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                _Enabled = value;
            }
        }
        private static string _DefaultFileName = string.Empty;
        public static string DefaultFileName
        {
            get
            {
                return _DefaultFileName;
            }
            set
            {
                _DefaultFileName = value;
            }
        }
        private string _FileName = string.Empty;
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
            }
        }
        private static string FormatException(Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Date/time=" + DateTime.Now.ToString());
            sb.AppendLine(exception.Message);
            sb.AppendLine("Source=" + exception.Source);
            sb.AppendLine("StackTrace=" + exception.StackTrace);
            return sb.ToString();
        }
        public async Task WriteAsync(Exception exception)
        {
            await WriteAsync(FormatException(exception));
        }
        public async Task WriteFormatAsync(string format, params object[] args)
        {
            await WriteAsync(string.Format(format, args));
        }
        public async Task WriteLineAsync(string message)
        {
            await WriteAsync(message + System.Environment.NewLine);
        }
        public async Task WriteAsync(string message)
        {
            if (!Enabled)
                return;
#if WINDOWS_PHONE
#else
            await _Semaphore.WaitAsync();
            await Task.Run(async () =>
                                     {
                                         try
                                         {
                                             var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
                                             await FileIO.AppendTextAsync(file, message);
                                         }
                                         catch (Exception ex)
                                         {
                                             Debug.WriteLine(ex);
                                         }
                                         finally
                                         {
                                             _Semaphore.Release();
                                         }
                                     }
            );
#endif
        }
    }
}