using System;
#if WINDOWS_PHONE
using System.Windows.Threading;
#else
using Windows.UI.Core;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
#endif

namespace ARDrone2Client.Common.ViewModel
{
    public static class SmartDispatcher
    {
#if !WINDOWS_PHONE
        private static CoreDispatcher _Dispatcher;
        public static CoreDispatcher Dispatcher
        {
            get
            {
                return _Dispatcher;
            }
            set
            {
                _Dispatcher = value;
            }
        }
#endif
        public static void Invoke(Action action)
        {
#if WINDOWS_PHONE
            //SynchronizationContext.Current.Post() 
#else
            if (_Dispatcher != null && !_Dispatcher.HasThreadAccess)
            {
                IAsyncAction asyncAction = _Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(action));
            }
            else
            {
                action();
            }
#endif
        }
    }
}
