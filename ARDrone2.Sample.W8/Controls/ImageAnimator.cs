using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace AR.Drone2.Sample.W8.Controls
{
    public sealed class ImageAnimator : Control
    {


        public ImageAnimator()
        {
            this.DefaultStyleKey = typeof(ImageAnimator);
        }
        static Random rnd = new Random();
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var image = (Image)this.GetTemplateChild("image");
            image.ImageOpened += (a, b) =>
            {
                if (image.Source != null)
                {
                    try
                    {
                        var bitmapSource = (Windows.UI.Xaml.Media.Imaging.BitmapSource)(image.Source);

                        var height = (bitmapSource.PixelHeight > this.ActualHeight) ? bitmapSource.PixelHeight : this.ActualHeight;
                        if (image.Parent != null && image.Parent is FrameworkElement)
                        {
                            var parent = (image.Parent as FrameworkElement);
                            var width = bitmapSource.PixelWidth;
                            parent.Height = image.Height = height;
                            parent.Width = image.Width = width;
                        }

                        var imageStoryboard = this.GetStoryboard(image);
                        imageStoryboard.Begin();
                    }
                    catch (Exception exe)
                    {
                        //IGNORE
                    }
                }
            };


        }

        private Storyboard GetStoryboard(Image target)
        {
            Storyboard storyboard = new Storyboard();

            if (target.Source != null)
            {
                Storyboard.SetTarget(storyboard, target.RenderTransform);

                var bitmapSource = (Windows.UI.Xaml.Media.Imaging.BitmapSource)(target.Source);

                DoubleAnimationUsingKeyFrames anim;
                QuadraticEase easingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
                //if (bitmapSource.PixelHeight > bitmapSource.PixelWidth)
                {


                    anim = new DoubleAnimationUsingKeyFrames();
                    //ANIM on Y
                    var minYValue = 0;
                    var maxYValue = Math.Abs(this.ActualHeight - bitmapSource.PixelHeight);
                    var offset = (this.ActualHeight > maxYValue) ? maxYValue / 2 : 0.0;


                    Storyboard.SetTargetProperty(anim, "TranslateY");
                    anim.AutoReverse = true;
                    anim.RepeatBehavior = RepeatBehavior.Forever;
                    var durationOnY = rnd.Next(9, 19) * Math.Max(maxYValue / 150, 1);


                    anim.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromSeconds(0), Value = minYValue + offset, EasingFunction = easingFunction });
                    anim.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromSeconds(durationOnY), Value = -maxYValue + offset, EasingFunction = easingFunction });
                    anim.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromSeconds(durationOnY * 2), Value = minYValue + offset, EasingFunction = easingFunction });

                    storyboard.Children.Add(anim);

                }


                //if (bitmapSource.PixelWidth > bitmapSource.PixelHeight)
                {

                    var minXValue = 0;

                    var maxXValue = Math.Abs(this.ActualWidth - bitmapSource.PixelWidth);
                    var offsetX = (this.ActualWidth > bitmapSource.PixelWidth) ? maxXValue / 2 : 0.0;

                    anim = new DoubleAnimationUsingKeyFrames();
                    Storyboard.SetTargetProperty(anim, "TranslateX");
                    anim.AutoReverse = true;
                    anim.RepeatBehavior = RepeatBehavior.Forever;
                    var durationOnX = rnd.Next(9, 19) * Math.Max(1, maxXValue / 150);
                    easingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
                    anim.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromSeconds(0), Value = minXValue + offsetX, EasingFunction = easingFunction });
                    anim.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromSeconds(durationOnX), Value = -maxXValue + offsetX, EasingFunction = easingFunction });
                    anim.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromSeconds(durationOnX * 2), Value = minXValue, EasingFunction = easingFunction });
                    storyboard.Children.Add(anim);
                }
            }

            return storyboard;
        }




        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(string), typeof(ImageAnimator), new PropertyMetadata("none",
                (s, a) =>
                {
                    //var ctl = (FlipControl)s;
                    //if (_activeFlipControls.Contains(ctl))
                    //    ctl.UpdateVisualState(true);
                    //Debug.WriteLine("test");
                }));


        #region ImageSource

        public string ImageSource
        {
            get { return (string)this.GetValue(ImageSourceProperty); }
            set { this.SetValue(ImageSourceProperty, value); }
        }



        #endregion



    }
}