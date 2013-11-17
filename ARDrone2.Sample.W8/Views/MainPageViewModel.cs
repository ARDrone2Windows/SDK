using AR.Drone2.Sample.W8.Common;
using AR.Drone2.Sample.W8.Model;
using AR.Drone2.Sample.W8.Views;
using ARDrone2Client.Common.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;

namespace AR.Drone2.Sample.W8
{
    public class MainPageViewModel : BaseViewModel
    {
        private ObservableCollection<FlightData> _flights;
        public ObservableCollection<FlightData> Flights
        {
            get
            {
                return this._flights;
            }
            set
            {
                this._flights = value;
                NotifyPropertyChanged("Flights");
            }
        }

        public void OnNavigatedTo()
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            Flights = new ObservableCollection<FlightData>();

            var random = new Random();
            // Design data
            for (int i = 0; i < 10; i++)
            {
                var flight = new FlightData { Name = DateTime.Now.AddDays(-i).ToString("dd/MM/yyyy"), Date = DateTime.Now.AddDays(-i).ToString("dd/MM/yyyy"), StartTime = TimeSpan.Parse("11:50"), EndTime = TimeSpan.Parse("11:55"), Duration = "5 min", AltitudeMax = "10m", AltitudeMin = "1m" };
                
                for (int j = 0; j < 10; j++)
                {
                    flight.Pictures.Add(new FlightPictureData { Index = j, Name = "Image " + j, Source = "/Assets/Samples/Image" + random.Next(1, 3) + ".png" });
                    flight.Videos.Add(new FlightVideoData { Name = "Video " + j, Preview = "/Assets/Samples/Video" + random.Next(1, 3) + ".png" });
                }

                Flights.Add(flight);
            }
        }
    }
}
