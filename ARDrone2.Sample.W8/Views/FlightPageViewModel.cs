using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using AR.Drone2.Sample.W8.Model;
using ARDrone2Client.Common.ViewModel;

namespace AR.Drone2.Sample.W8
{
    public class FlightPageViewModel : BaseViewModel
    {
        #region Properties

        private FlightData _currentFlight;
        public FlightData CurrentFlight
        {
            get
            {
                return _currentFlight;
            }
            set
            {
                if (_currentFlight == value)
                    return;

                _currentFlight = value;
                NotifyPropertyChanged("CurrentFlight");
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return this._selectedIndex; }
            set
            {
                this._selectedIndex = value;

                this.NotifyPropertyChanged("SelectedIndex");
            }
        }

        #endregion

        #region Constructor

        public FlightPageViewModel()
        {
            if (DesignMode.DesignModeEnabled)
            {
                var random = new Random();

                // Design data
                var flight = new FlightData { Name = "Vol 1234", Date = "06/11/2013", StartTime = TimeSpan.Parse("11:50"), EndTime = TimeSpan.Parse("11:55"), Duration = "5 min", AltitudeMax = "10m", AltitudeMin = "1m" };
                for (int j = 0; j < 10; j++)
                {
                    flight.Pictures.Add(new FlightPictureData {Index  = j, Name = "Image " + j, Source = "/Assets/Samples/Image" + random.Next(1, 3) + ".jpg" });
                    flight.Videos.Add(new FlightVideoData { Name = "Video " + j, Preview = "/Assets/Samples/Video" + random.Next(1, 3) + ".png" });
                }

                CurrentFlight = flight;
            }
        }

        #endregion

        #region Methods

        public void OnNavigatedTo(FlightData flightData)
        {
            CurrentFlight = flightData;
        }
        
        #endregion

        #region Commands
        
        #endregion
    }
}
