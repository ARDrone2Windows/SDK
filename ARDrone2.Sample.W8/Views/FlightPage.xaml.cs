using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using AR.Drone2.Sample.W8.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace AR.Drone2.Sample.W8.Views
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class FlightPage : Page
    {
        public FlightPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ((FlightPageViewModel) (this.DataContext)).OnNavigatedTo(e.Parameter as FlightData);
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void OpenPhotoViewer(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var dc = btn.DataContext as FlightPictureData;
                if (dc != null)
                {
                    ((FlightPageViewModel) this.DataContext).SelectedIndex = dc.Index;

                    this.ParametersGrid.IsOpen = true;
                }
            }
        }

        private void ParametersGrid_Opened(object sender, object e)
        {
            this.overlay.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void ParametersGrid_Closed(object sender, object e)
        {
            this.overlay.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}
