using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace AR.Drone2.Sample.W8.Controls
{
    public sealed class ClipToBoundsControl : ContentControl
    {

        public ClipToBoundsControl()
        {
            SizeChanged += ClipToBoundsControl_SizeChanged;
        }

        void ClipToBoundsControl_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {

            Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height)
            };

        }
    }
}
