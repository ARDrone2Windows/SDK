using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR.Drone2.Sample.W8
{
    public class ViewModelLocator
    {
        private Lazy<MainPageViewModel> mainPageViewModel = new Lazy<MainPageViewModel>(() => new MainPageViewModel(), false);
        public MainPageViewModel MainPageViewModel
        {
            get
            {
                return this.mainPageViewModel.Value;
            }
        }

        private Lazy<FlightPageViewModel> flightPageViewModel = new Lazy<FlightPageViewModel>(() => new FlightPageViewModel(), false);
        public FlightPageViewModel FlightPageViewModel
        {
            get
            {
                return this.flightPageViewModel.Value;
            }
        }
    }
}
