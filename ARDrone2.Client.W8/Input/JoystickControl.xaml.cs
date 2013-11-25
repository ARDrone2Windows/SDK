using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using ARDrone2Client.Common.Input;

namespace ARDrone2Client.Windows.Input
{
    public partial class JoystickControl : UserControl, IJoystickControl
    {
        const int ticketDefault = 50;
        const int byPassTicksMax = 3;
        private bool byPassTicks = false;
        private int byPassTicksCount = 0;

        private float currentX;
        private float currentY;

        double lastX;
        double lastY;
        double newX;
        double newY;

        uint pointerId = 0;
        #region Stick color property

        public static DependencyProperty StickColorProperty =
            DependencyProperty.Register("StickColor", typeof(SolidColorBrush), typeof(JoystickControl),
            new PropertyMetadata(new SolidColorBrush(Colors.Blue), JoystickControl.OnStickColorChanged));

        public SolidColorBrush StickColor
        {
            get { return (SolidColorBrush)GetValue(StickColorProperty); }
            set { SetValue(StickColorProperty, value); }
        }

        private static void OnStickColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var joystick = d as JoystickControl;
            if (joystick != null && e.NewValue is SolidColorBrush)
                joystick.StickColor = (SolidColorBrush)e.NewValue;
        }

        #endregion

        #region Background elipse color property

        public static DependencyProperty BorderElipseColorProperty =
            DependencyProperty.Register("BorderElipseColor", typeof(SolidColorBrush), typeof(JoystickControl),
            new PropertyMetadata(new SolidColorBrush(Colors.White), JoystickControl.OnBorderColorChanged));

        public SolidColorBrush BorderElipseColor
        {
            get { return (SolidColorBrush)GetValue(BorderElipseColorProperty); }
            set { SetValue(BorderElipseColorProperty, value); }
        }

        private static void OnBorderColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var joystick = d as JoystickControl;
            if (joystick != null && e.NewValue is SolidColorBrush)
                joystick.BorderElipseColor = (SolidColorBrush)e.NewValue;
        }

        #endregion

        public JoystickControl()
        {
            InitializeComponent();
            this.PointerReleased += (sender, args) => Reset();
            this.PointerMoved += OnPointerMoved; // CoreWindowOnPointerMoved;
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            Touch_FrameReported(args.GetCurrentPoint(this));
        }

        private void CoreWindowOnPointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            var windowToAppBarTransform = Window.Current.Content.TransformToVisual(ellipseSense);
            var p = windowToAppBarTransform.TransformPoint(args.CurrentPoint.Position);
            Touch_FrameReported(args.CurrentPoint, p);
        }

        private void Touch_FrameReported(PointerPoint pp)
        {
            if (pp == null)
                return;
            Touch_FrameReported(pp, pp.Position);
        }
        private void Touch_FrameReported(PointerPoint pp, Point p)
        {
            if (pp == null)
                return;
            if (pointerId == 0)
                pointerId = pp.PointerId;
            if (pointerId == pp.PointerId)
                Touch_FrameReported(p);
        }
        private void Touch_FrameReported(Point p)
        {
            if (byPassTicks)
            {
                byPassTicksCount++;
                if (byPassTicksCount <= byPassTicksMax)
                    return;
                byPassTicksCount = 0;
                byPassTicks = false;
            }
            try
            {
                if (p.X < 0)
                    p.X = 0;

                if (p.Y < 0)
                    p.Y = 0;

                if (p.X > ellipseSense.ActualWidth)
                    p.X = ellipseSense.ActualWidth;

                if (p.Y > ellipseSense.ActualHeight)
                    p.Y = ellipseSense.ActualHeight;

                Point center = new Point(ellipseSense.ActualWidth / 2, ellipseSense.ActualHeight / 2);


                //double angle = Math.Atan2(p.Y - center.Y, p.X - center.X) * 180 / Math.PI;
                //if (angle > 0)
                //{
                //    angle += 90;
                //}
                //else
                //{
                //    angle = 270 + (180 + angle);
                //    if (angle >= 360)
                //    {
                //        angle -= 360;
                //    }
                //}
                currentX = (float)(((p.X * 2) / ActualWidth) - 1);
                currentY = (float)-(((p.Y * 2) / ActualHeight) - 1);
                //Debug.WriteLine(string.Format("CurrentX={0}, CurrentY={1}", currentX, currentY));
                // Set Joystick Pos
                newX = p.X - (ellipseSense.ActualWidth / 2);
                newY = p.Y - (ellipseSense.ActualWidth / 2);
                MoveJoystick(newX, newY);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void MoveJoystick(double moveX, double moveY)
        {
            Storyboard sb = new Storyboard();
            KeyTime ktStart = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0));
            KeyTime ktEnd = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100));

            DoubleAnimationUsingKeyFrames animationFirstX = new DoubleAnimationUsingKeyFrames();
            DoubleAnimationUsingKeyFrames animationFirstY = new DoubleAnimationUsingKeyFrames();

            ellipseButton.RenderTransform = new CompositeTransform();

            Storyboard.SetTargetProperty(animationFirstX, "(UIElement.Projection).(PlaneProjection.GlobalOffsetX)");
            Storyboard.SetTarget(animationFirstX, ellipseButton);
            animationFirstX.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = ktStart, Value = lastX });
            animationFirstX.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = ktEnd, Value = moveX });
            
            Storyboard.SetTargetProperty(animationFirstY, "(UIElement.Projection).(PlaneProjection.GlobalOffsetY)");
            Storyboard.SetTarget(animationFirstY, ellipseButton);
            animationFirstY.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = ktStart, Value = lastY });
            animationFirstY.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = ktEnd, Value = moveY });

            sb.Children.Add(animationFirstX);
            sb.Children.Add(animationFirstY);
            sb.Begin();

            lastX = moveX;
            lastY = moveY;
        }

        private void ContainerOnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            byPassTicks = (byPassTicksMax > 0);
            Touch_FrameReported(e.GetCurrentPoint(this));
        }

        public void Reset()
        {
            pointerId = 0;
            byPassTicks = false;
            byPassTicksCount = 0;
            Touch_FrameReported(new Point(ellipseSense.ActualWidth / 2, ellipseSense.ActualHeight / 2));
        }

        public float X
        {
            get
            {
                return currentX;
            }
        }
        public float Y
        {
            get
            {
                return currentY;
            }
        }
    }

    //public class JoystickCoordinates : EventArgs
    //{
    //    public Double X;
    //    public Double Y;
    //}
}
