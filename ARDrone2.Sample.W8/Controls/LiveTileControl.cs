using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace AR.Drone2.Sample.W8.Controls
{
    public class SlideEventArgs : EventArgs
    {
    }

    [TemplatePart(Name = FRONTITEM_PARTNAME, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = BACKITEM_PARTNAME, Type = typeof(ContentPresenter))]
    public sealed class LiveTileControl : Control
    {
        private static Random random = new Random();

        #region Constants

        const string FRONTITEM_PARTNAME = "PART_Front";
        const string BACKITEM_PARTNAME = "PART_Back";

        #endregion

        #region Enums

        public enum SlideDirection
        {
            Up,
            Left
        }

        #endregion

        #region Events

        public delegate void SlideEventHandler(object sender, SlideEventArgs e);

        public event SlideEventHandler SlideStarting;
        public event SlideEventHandler SlideEnded;

        #endregion

        #region Member Fields

        private DispatcherTimer _timer;
        private bool _isPlayingAnimation;
        private bool _isBackVisible;

        private ContentPresenter _frontItem;
        private ContentPresenter _backItem;

        private TranslateTransform _frontItemTransform;
        private TranslateTransform _backItemTransform;

        #endregion

        #region Dependency Properties

        public SlideDirection Direction
        {
            get { return (SlideDirection)this.GetValue(DirectionProperty); }
            set { this.SetValue(DirectionProperty, value); }
        }

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(SlideDirection), typeof(LiveTileControl), new PropertyMetadata(SlideDirection.Up));

        public object FrontContent
        {
            get { return (object)this.GetValue(FrontContentProperty); }
            set { this.SetValue(FrontContentProperty, value); }
        }

        public static readonly DependencyProperty FrontContentProperty =
            DependencyProperty.Register("FrontContent", typeof(object), typeof(LiveTileControl), new PropertyMetadata(null, (o, args) =>
            {
                var ctrl = o as LiveTileControl;
                if (ctrl != null && ctrl._frontItem != null)
                {
                    ctrl.ChangeDataContext();
                }
            }));

        public object BackContent
        {
            get { return (object)this.GetValue(BackContentProperty); }
            set { this.SetValue(BackContentProperty, value); }
        }

        public static readonly DependencyProperty BackContentProperty =
            DependencyProperty.Register("BackContent", typeof(object), typeof(LiveTileControl), new PropertyMetadata(null, (o, args) =>
            {
                var ctrl = o as LiveTileControl;
                if (ctrl != null && ctrl._backItem != null)
                {
                    ctrl.ChangeDataContext();
                }
            }));

        public DataTemplate FrontItemTemplate
        {
            get { return (DataTemplate)this.GetValue(FrontItemTemplateProperty); }
            set { this.SetValue(FrontItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty FrontItemTemplateProperty =
            DependencyProperty.Register("FrontItemTemplate", typeof(DataTemplate), typeof(LiveTileControl), new PropertyMetadata(null, (o, args) =>
            {
                var ctrl = o as LiveTileControl;
                if (ctrl != null)
                {
                    ctrl.ChangeDataContext();
                }
            }));

        public DataTemplate BackItemTemplate
        {
            get { return (DataTemplate)this.GetValue(BackItemTemplateProperty); }
            set { this.SetValue(BackItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty BackItemTemplateProperty =
            DependencyProperty.Register("BackItemTemplate", typeof(DataTemplate), typeof(LiveTileControl), new PropertyMetadata(null, (o, args) =>
            {
                var ctrl = o as LiveTileControl;
                if (ctrl != null)
                {
                    ctrl.ChangeDataContext();
                }
            }));

        private void ChangeDataContext()
        {
            if (this._frontItem != null && this._backItem != null)
            {
                this._frontItem.DataContext = this.FrontContent;
                this._backItem.DataContext = this.BackContent;
            }
        }

        #endregion

        #region Constructor

        public LiveTileControl()
        {
            this.DefaultStyleKey = typeof(LiveTileControl);

            this.SizeChanged += this.OnLiveTileControlSizeChanged;
            this.Unloaded += this.OnLiveTileControlUnLoaded;
        }

        #endregion

        protected override void OnApplyTemplate()
        {
            this._frontItem = this.GetTemplateChild(FRONTITEM_PARTNAME) as ContentPresenter;
            this._backItem = this.GetTemplateChild(BACKITEM_PARTNAME) as ContentPresenter;

            if (this._frontItem != null && this._backItem != null)
            {
                this._frontItemTransform = this._frontItem.RenderTransform as TranslateTransform;
                this._backItemTransform = this._backItem.RenderTransform as TranslateTransform;

                if (this._backItemTransform != null)
                {
                    if (this.Direction == SlideDirection.Up)
                    {
                        this._backItemTransform.Y = this.Height;
                    }
                    else
                    {
                        this._backItemTransform.X = this.Width;
                    }
                }

                var startTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(random.Next(1, 5))
                };
                startTimer.Tick += (sender, o) =>
                {
                    this.RaiseSlideStarting();

                    // Show Back item
                    this.ShowBackItem();

                    this._isBackVisible = !this._isBackVisible;

                    startTimer.Stop();
                    startTimer = null;
                };

                    startTimer.Start();

                this.StartAnimation();
                this.ChangeDataContext();
            }

            base.OnApplyTemplate();
        }

        #region Private Methods

        private void OnLiveTileControlUnLoaded(object sender, RoutedEventArgs args)
        {
            if (this._timer != null)
            {
                this._timer.Stop();
                this._timer = null;
            }

            if (this._frontItemTransform != null)
            {
                if (this.Direction == SlideDirection.Up)
                {
                    this._frontItemTransform.Y = 0;
                }
                else
                {
                    this._frontItemTransform.X = 0;
                }
            }

            if (this._backItemTransform != null)
            {
                if (this.Direction == SlideDirection.Up)
                {
                    this._backItemTransform.Y = this.Height;
                }
                else
                {
                    this._backItemTransform.X = this.Width;
                }
            }
        }

        private void OnLiveTileControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this._frontItem != null && this._backItem != null)
            {
                this._frontItem.Width = this._backItem.Width = e.NewSize.Width;
                this._frontItem.Height = this._backItem.Height = e.NewSize.Height;
            }
        }

        private void RaiseSlideStarting()
        {
            var handler = this.SlideStarting;
            if (handler != null)
            {
                handler(this, new SlideEventArgs());
            }
        }

        private void RaiseSlideEnded()
        {
            var handler = this.SlideEnded;
            if (handler != null)
            {
                handler(this, new SlideEventArgs());
            }
        }

        private void StartAnimation()
        {
            if (this._timer == null)
            {
                this._timer = new DispatcherTimer
                {
                    Interval = new TimeSpan(0, 0, random.Next(4, 7))
                };

                this._timer.Tick += (sender, o) =>
                {
                    if (!this._isPlayingAnimation)
                    {
                        this.RaiseSlideStarting();

                        if (this._isBackVisible)
                        {
                            // Show Front item
                            this.ShowFrontItem();
                        }
                        else
                        {
                            // Show Back item
                            this.ShowBackItem();
                        }

                        this._isBackVisible = !this._isBackVisible;
                    }
                };
                this._timer.Start();
            }
        }

        private void ShowFrontItem()
        {
            var sb = new Storyboard();

            var hideBackItemAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(2000)),
                To = this.Direction == SlideDirection.Up ? this.Height : this.Width,
                FillBehavior = FillBehavior.HoldEnd,
                EasingFunction = new QuinticEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            Storyboard.SetTarget(hideBackItemAnimation, this._backItemTransform);
            Storyboard.SetTargetProperty(hideBackItemAnimation, this.Direction == SlideDirection.Up ? "Y" : "X");

            sb.Children.Add(hideBackItemAnimation);

            var showFrontItemAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(2000)),
                To = 0,
                FillBehavior = FillBehavior.HoldEnd,
                EasingFunction = new QuinticEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            Storyboard.SetTarget(showFrontItemAnimation, this._frontItemTransform);
            Storyboard.SetTargetProperty(showFrontItemAnimation, this.Direction == SlideDirection.Up ? "Y" : "X");

            sb.Children.Add(showFrontItemAnimation);

            sb.Completed += (a, b) =>
            {
                this._isPlayingAnimation = false;

                this.RaiseSlideEnded();
            };
            sb.Begin();

            this._isPlayingAnimation = true;
        }

        private void ShowBackItem()
        {
            var sb = new Storyboard();

            var showBackItemAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(2000)),
                To = 0,
                FillBehavior = FillBehavior.HoldEnd,
                EasingFunction = new QuinticEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            Storyboard.SetTarget(showBackItemAnimation, this._backItemTransform);
            Storyboard.SetTargetProperty(showBackItemAnimation, this.Direction == SlideDirection.Up ? "Y" : "X");

            sb.Children.Add(showBackItemAnimation);

            var hideFrontItemAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(2000)),
                To = this.Direction == SlideDirection.Up ? -this.Height : -this.Width,
                FillBehavior = FillBehavior.HoldEnd,
                EasingFunction = new QuinticEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            Storyboard.SetTarget(hideFrontItemAnimation, this._frontItemTransform);
            Storyboard.SetTargetProperty(hideFrontItemAnimation, this.Direction == SlideDirection.Up ? "Y" : "X");

            sb.Children.Add(hideFrontItemAnimation);

            sb.Completed += (a, b) =>
            {
                this._isPlayingAnimation = false;

                this.RaiseSlideEnded();
            };
            sb.Begin();

            this._isPlayingAnimation = true;
        }

        #endregion
    }
}
