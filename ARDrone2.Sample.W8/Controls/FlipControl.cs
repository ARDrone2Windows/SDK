using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace AR.Drone2.Sample.W8.Controls
{
    public class FlipControl : Control
    {
        private CancellationTokenSource _flipCTS;
        public FlipControl()
        {
            DefaultStyleKey = typeof(FlipControl);
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            this.PointerPressed += FlipControl_MouseLeftButtonDown;
            this.PointerReleased += FlipControl_MouseLeftButtonUp;
        }
        
        private void FlipControl_MouseLeftButtonDown(object sender, PointerRoutedEventArgs e)
        {
            _lastMouseLeftButtonDownPosition = e.GetCurrentPoint(this).Position;
            _lastMouseLeftButtonDown = DateTime.Now;
        }

        private DateTime _lastMouseLeftButtonDown = DateTime.MinValue;

        private DateTime _lastMouseLeftButtonUp = DateTime.MinValue;
        private DispatcherTimer _doubleTapDispatcherTimer;
        private Point _lastMouseLeftButtonDownPosition;
        //private Point _lastMouseLeftButtonDownPosition;

        void FlipControl_MouseLeftButtonUp(object sender, PointerRoutedEventArgs e)
        {
            var now = DateTime.Now;


            CancelDoubleTapTimer();


            if ((now - _lastMouseLeftButtonDown).TotalMilliseconds > 400)
            {
                return;
            }

            var curPost = e.GetCurrentPoint(this).Position;
            var deltaPos = new Point(
                Math.Abs(_lastMouseLeftButtonDownPosition.X - curPost.X),
                Math.Abs(_lastMouseLeftButtonDownPosition.Y - curPost.Y)
                );

            if (deltaPos.X > 5 || deltaPos.Y > 5)
            {
                return;

            }

            if (IsFlippingOnDoubleTap)
            {
                //double tap
                if ((now - _lastMouseLeftButtonUp).TotalMilliseconds < 400)
                {
                    CancelDoubleTapTimer();
                    IsAlternate = !IsAlternate;

                }
                else
                {
                    _doubleTapDispatcherTimer = new DispatcherTimer();
                    _doubleTapDispatcherTimer.Interval = TimeSpan.FromMilliseconds(500);
                    _doubleTapDispatcherTimer.Tick += _doubleTapDispatcherTimer_Tick;
                    _doubleTapDispatcherTimer.Start();
                }

            }
            else
            {
                ExecuteCommand();
            }


            _lastMouseLeftButtonUp = now;
        }

        private void CancelDoubleTapTimer()
        {
            if (_doubleTapDispatcherTimer != null)
            {
                _doubleTapDispatcherTimer.Stop();
                _doubleTapDispatcherTimer = null;
            }
        }

        private void ExecuteCommand()
        {
            if (Command != null) Command.Execute(CommandParameter);
        }

        void _doubleTapDispatcherTimer_Tick(object sender, object e)
        {
            CancelDoubleTapTimer();
            ExecuteCommand();

        }



        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_flipCTS != null)
            {
                _flipCTS.Cancel();
                _flipCTS = null;
            }

            CancelDoubleTapTimer();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {

            StartFlipFlap();
        }

        private async void StartFlipFlap()
        {
            if (_flipCTS != null)
            {
                _flipCTS.Cancel();
                _flipCTS = null;
            }
            _flipCTS = new CancellationTokenSource();

            if (IsAutoFlipping)
            {
                if (!IsAlternate && FlipWhenLoaded)
                {
                    IsAlternate = true;
                    ComputeIsAlternate(this);
                }

                await DoFlipFlapAsync(_flipCTS.Token);

                if (IsAlternate)
                {
                    ComputeIsAlternate(this);
                }
            }

        }

        static readonly Random _random = new Random();
        private async Task DoFlipFlapAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var timeToWait = 1000 + _random.NextDouble() * 3000;
                    await Task.Delay((int)timeToWait, token);
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    if (IsAutoFlipping)
                    {
                        IsAlternate = true;
                    }
                    timeToWait = 3000 + _random.NextDouble() * 3000;
                    await Task.Delay((int)timeToWait, token);
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    if (IsAutoFlipping)
                    {
                        IsAlternate = false;
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        #region CommandParameter

        /// <summary>
        /// CommandParameter Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(FlipControl),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the CommandParameter property. This dependency property 
        /// indicates ....
        /// </summary>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        #endregion




        #region Command

        /// <summary>
        /// Command Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(FlipControl),
                new PropertyMetadata((ICommand)null));

        /// <summary>
        /// Gets or sets the Command property. This dependency property 
        /// indicates ....
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #endregion




        public object MainView
        {
            get { return GetValue(MainViewProperty); }
            set { SetValue(MainViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MainViewe.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MainViewProperty =
            DependencyProperty.Register("MainView", typeof(object), typeof(FlipControl), new PropertyMetadata(null));



        public object AlternateView
        {
            get { return GetValue(AlternateViewProperty); }
            set { SetValue(AlternateViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AlternateView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlternateViewProperty =
            DependencyProperty.Register("AlternateView", typeof(object), typeof(FlipControl), new PropertyMetadata(null));



        #region IsFlippingOnDoubleTap

        /// <summary>
        /// IsFlippingOnDoubleTap Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsFlippingOnDoubleTapProperty =
            DependencyProperty.Register("IsFlippingOnDoubleTap", typeof(bool), typeof(FlipControl),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the IsFlippingOnDoubleTap property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsFlippingOnDoubleTap
        {
            get { return (bool)GetValue(IsFlippingOnDoubleTapProperty); }
            set { SetValue(IsFlippingOnDoubleTapProperty, value); }
        }

        #endregion



        #region IsAutoFlipping

        /// <summary>
        /// IsAutoFlipping Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsAutoFlippingProperty =
            DependencyProperty.Register("IsAutoFlipping", typeof(bool), typeof(FlipControl),
                new PropertyMetadata((bool)true,
                    OnIsAutoFlippingChanged));

        /// <summary>
        /// Gets or sets the IsAutoFlipping property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsAutoFlipping
        {
            get { return (bool)GetValue(IsAutoFlippingProperty); }
            set { SetValue(IsAutoFlippingProperty, value); }
        }

        /// <summary>
        /// Handles changes to the IsAutoFlipping property.
        /// </summary>
        private static void OnIsAutoFlippingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FlipControl target = (FlipControl)d;
            bool oldIsAutoFlipping = (bool)e.OldValue;
            bool newIsAutoFlipping = target.IsAutoFlipping;
            target.OnIsAutoFlippingChanged(oldIsAutoFlipping, newIsAutoFlipping);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the IsAutoFlipping property.
        /// </summary>
        protected virtual void OnIsAutoFlippingChanged(bool oldIsAutoFlipping, bool newIsAutoFlipping)
        {
            if (!newIsAutoFlipping)
            {

                if (_flipCTS != null)
                {
                    _flipCTS.Cancel();
                    _flipCTS = null;
                }

            }
            else
            {
                StartFlipFlap();
            }
        }

        #endregion


        #region FlipWhenLoaded

        /// <summary>
        /// Does the FlipControl needs to flip when the loaded event is raised ?
        /// </summary>
        public static readonly DependencyProperty FlipWhenLoadedProperty =
            DependencyProperty.Register("FlipWhenLoaded", typeof(bool), typeof(FlipControl),
            new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the FlipWhenLoaded property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool FlipWhenLoaded
        {
            get { return (bool)GetValue(FlipWhenLoadedProperty); }
            set { SetValue(FlipWhenLoadedProperty, value); }
        }

        #endregion



        public bool AnimationCompleted
        {
            get { return (bool)GetValue(AnimationCompletedProperty); }
            set { SetValue(AnimationCompletedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AnimationCompleted.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationCompletedProperty =
            DependencyProperty.Register("AnimationCompleted", typeof(bool), typeof(FlipControl), null);



        public bool IsAlternate
        {
            get { return (bool)GetValue(IsAlternateProperty); }
            set { SetValue(IsAlternateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAlternate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAlternateProperty =
            DependencyProperty.Register("IsAlternate", typeof(bool), typeof(FlipControl), new PropertyMetadata(false, (s,
                                                                                                                       a)
                                                                                                                      => ComputeIsAlternate(s as FlipControl)));

        private static void ComputeIsAlternate(FlipControl flp)
        {
            if (flp == null) return;

            flp.AnimationCompleted = false;

            var root = flp.GetTemplateChild("root") as FrameworkElement;
            if (root == null)
            {
                return;
            }
            string storyboardName = (flp.IsAlternate ? "goToAlternate" : "goToMain") + flp.Orientation;
            var sb = root.Resources[storyboardName] as Storyboard;
            if (sb == null)
            {
                return;
            }
            EventHandler<object> h = null;
            h = (s, e) =>
            {
                sb.Completed -= h;
                flp.AnimationCompleted = true;
            };
            sb.Completed += h;
            sb.Begin();
        }

        #region Orientation

        /// <summary>
        /// Orientation Dependency Property
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(FlipControlOrientation), typeof(FlipControl),
                new PropertyMetadata(FlipControlOrientation.Horizontal));




        /// <summary>
        /// Gets or sets the Orientation property. This dependency property 
        /// indicates ....
        /// </summary>
        public FlipControlOrientation Orientation
        {
            get { return (FlipControlOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion




    }

    public enum FlipControlOrientation
    {
        Horizontal,
        Vertical
    }
}
