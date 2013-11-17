using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using System.Runtime.CompilerServices;

using ARDrone2Client.Common.ViewModel;

#if NETFX_CORE
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
#endif

namespace ARDrone2Client.Common.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

#if NETFX_CORE
        private readonly Frame _frame;
#endif
        public BaseViewModel()
        {
#if NETFX_CORE
            _frame = (Frame)Window.Current.Content;
#endif
        }

#if NETFX_CORE
        public bool CanGoBack
        {
            get
            {
                return _frame.CanGoBack;
            }
        }

        protected virtual void GoBack()
        {
            _frame.GoBack();
        }

        protected void NavigateTo(Type pageType)
        {
            _frame.Navigate(pageType);
        }

        protected void NavigateTo(Type pageType, object parameter)
        {
            _frame.Navigate(pageType, parameter);
        }
#endif

        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                SmartDispatcher.Invoke(() =>
                {
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }
    }
}