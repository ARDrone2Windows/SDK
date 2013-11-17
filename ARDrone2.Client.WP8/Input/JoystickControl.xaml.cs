using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Threading;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Windows.Media;

using ARDrone2Client.Common.Input;

namespace ARDrone2Client.WP8.Input
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

        bool moveJoystick = false;

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

        public JoystickControl()
        {
            InitializeComponent();
        }

        public void StartJoystick()
        {
            Touch.FrameReported += Touch_FrameReported;
        }

        public void StopJoystick()
        {
            Touch.FrameReported -= Touch_FrameReported;
        }

        void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            try
            {
                int pointsNumber = e.GetTouchPoints(ellipseSense).Count;
                TouchPointCollection pointCollection = e.GetTouchPoints(ellipseSense);


                for (int i = 0; i < pointsNumber; i++)
                {
                    if (pointCollection[i].Position.X > 0 && pointCollection[i].Position.X < ellipseSense.ActualWidth)
                    {
                        if (pointCollection[i].Position.Y > 0 && pointCollection[i].Position.Y < ellipseSense.ActualHeight)
                        {
                            Touch_FrameReported(pointCollection[i].Position);
                        }
                    }
                }
            }
            catch
            {
            }
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

                currentX = (float)(((p.X * 2) / ActualWidth) - 1);
                currentY = (float)-(((p.Y * 2) / ActualHeight) - 1);

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
            KeyTime ktEnd = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200));

            DoubleAnimationUsingKeyFrames animationFirstX = new DoubleAnimationUsingKeyFrames();
            DoubleAnimationUsingKeyFrames animationFirstY = new DoubleAnimationUsingKeyFrames();

            ellipseButton.RenderTransform = new CompositeTransform();

            Storyboard.SetTargetProperty(animationFirstX, new PropertyPath(CompositeTransform.TranslateXProperty));
            Storyboard.SetTarget(animationFirstX, ellipseButton.RenderTransform);
            animationFirstX.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = ktStart, Value = lastX });
            animationFirstX.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = ktEnd, Value = moveX });


            Storyboard.SetTargetProperty(animationFirstY, new PropertyPath(CompositeTransform.TranslateYProperty));
            Storyboard.SetTarget(animationFirstY, ellipseButton.RenderTransform);
            animationFirstY.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = ktStart, Value = lastY });
            animationFirstY.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = ktEnd, Value = moveY });

            sb.Children.Add(animationFirstX);
            sb.Children.Add(animationFirstY);
            sb.Begin();

            lastX = moveX;
            lastY = moveY;
        }

        private void ellipseSense_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            //timer.Start();
            moveJoystick = true;

            Debug.WriteLine("Manipulation Started");
        }

        private void ellipseSense_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            // Fire event
            OnStop();

            // Move Joystick to Center
            MoveJoystick(0, 0);
            moveJoystick = false;

            Debug.WriteLine("Manipulation Completed");
        }

        public event EventHandler Stop;
        protected virtual void OnStop()
        {
            var myStop = new MyStop();
            myStop.Stopped = true;
            if (Stop != null)
                Stop(this, myStop);
        }
    }
    public class MyStop : EventArgs
    {
        public bool Stopped;
    }
}