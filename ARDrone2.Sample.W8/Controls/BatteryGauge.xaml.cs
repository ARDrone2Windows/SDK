using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AR.Drone2.Sample.W8.Controls
{
    public sealed partial class BatteryGauge : UserControl
    {
        public static readonly DependencyProperty TestValueProperty =
            DependencyProperty.Register("TestValue", typeof(double), typeof(BatteryGauge), new PropertyMetadata(default(double), ValuePropertyChangedCallback));

        private static void ValuePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var self = dependencyObject as BatteryGauge;
            if (self == null) return;
            self.PercentageValue = Math.Max(0, Math.Min(100, self.TestValue * 100 / self.MaxValue));
            self.PercentageValueAsGridLength = new GridLength(self.PercentageValue, GridUnitType.Star);
            self.MaxPercentageValueAsGridLength = new GridLength(100 - self.PercentageValue, GridUnitType.Star);
        }

        public double TestValue
        {
            get { return (double)GetValue(TestValueProperty); }
            set { SetValue(TestValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(BatteryGauge), new PropertyMetadata(100D, ValuePropertyChangedCallback));

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty PercentageValueProperty =
            DependencyProperty.Register("PercentageValue", typeof(double), typeof(BatteryGauge), new PropertyMetadata(default(double)));

        public double PercentageValue
        {
            get { return (double)GetValue(PercentageValueProperty); }
            private set { SetValue(PercentageValueProperty, value); }
        }

        private static readonly DependencyProperty PercentageValueAsGridLengthProperty =
            DependencyProperty.Register("PercentageValueAsGridLength", typeof (GridLength), typeof (BatteryGauge), new PropertyMetadata(default(GridLength)));

        private GridLength PercentageValueAsGridLength
        {
            get { return (GridLength) GetValue(PercentageValueAsGridLengthProperty); }
            set { SetValue(PercentageValueAsGridLengthProperty, value); }
        }

        public static readonly DependencyProperty MaxPercentageValueAsGridLengthProperty =
            DependencyProperty.Register("MaxPercentageValueAsGridLength", typeof (GridLength), typeof (BatteryGauge), new PropertyMetadata(default(GridLength)));

        public GridLength MaxPercentageValueAsGridLength
        {
            get { return (GridLength) GetValue(MaxPercentageValueAsGridLengthProperty); }
            set { SetValue(MaxPercentageValueAsGridLengthProperty, value); }
        }

        public BatteryGauge()
        {
            InitializeComponent();
        }
    }
}
