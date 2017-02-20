/**********************************************************/
/*************** The front move image repeat listener
/**********************************************************/

using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using ViewImageView = Android.Views;
using MyBodyShape.Android.Helpers;
using System;
using System.Collections.Generic;
using System.Timers;
using static Android.Views.View;

namespace MyBodyShape.Android.Listeners
{
    /// <summary>
    /// The front move image repeat listener.
    /// </summary>
    public class FrontMoveRepeatListener : Java.Lang.Object, IOnTouchListener
    {
        /// <summary>
        ///  The private controls.
        /// </summary>
        private Handler _repeatedHandler;
        private ViewImageView.View _imageView;
        private ImageButton _button;
        private Canvas _canvas;
        private Paint _paint;
        private Paint _pathPaint;
        private float _radius;
        private List<CircleArea> _circleList;
        private List<PathArea> _pathList;
        private View _downView;
        private int _normalInterval;
        private int _initialInterval;
        private Action<View> _intervalClickListener;
        private Action<View> _initialClickListener;
        private Action<View, bool> _intervalCancelListener;
        private Timer _timer;
        private bool _isLongPress = false;

        /// <summary>
        /// The move image runnable.
        /// </summary>
        public FrontMoveImageRunnable MoveimageRunnable { get; set; }

        /// <summary>
        /// The constructor.
        /// </summary>
        public FrontMoveRepeatListener(ImageButton button, ViewImageView.View imageView, Canvas canvas, Paint paint, Paint pathPaint, float radius, List<CircleArea> circlesList, List<PathArea> pathList, int initialInterval, int normalInterval, Action<View> clickListener, Action<View> initialClickListener, Action<View, bool> cancelListener = null)
        {
            _initialInterval = initialInterval;
            _normalInterval = normalInterval;
            _intervalClickListener = clickListener;
            _intervalCancelListener = cancelListener;
            _initialClickListener = initialClickListener;
            _timer = new Timer(initialInterval);
            _timer.Enabled = false;
            _timer.Elapsed += HandleTimerElapsed;
            _repeatedHandler = new Handler();
            _imageView = imageView;
            _button = button;
            _canvas = canvas;
            _paint = paint;
            _pathPaint = pathPaint;
            _radius = radius;
            _circleList = circlesList;
            _pathList = pathList;
            MoveimageRunnable = new FrontMoveImageRunnable(_imageView, _repeatedHandler, _button, _canvas, _paint, _pathPaint, _radius, _circleList, _pathList);
        }

        // The handle timer elapsed.
        private void HandleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _isLongPress = true;
            InvokeClickListener();
            _timer.Interval = _normalInterval;
            _timer.Start();
        }

        /// <summary>
        /// The invoke initial click listener.
        /// </summary>
        private void InvokeInitialClickListener()
        {
            if (_initialClickListener != null)
                _initialClickListener(_downView);
        }

        /// <summary>
        /// The invoke click listener.
        /// </summary>
        private void InvokeClickListener()
        {
            if (_intervalClickListener != null)
                _intervalClickListener(_downView);
        }

        /// <summary>
        /// The invoke cancel click listener.
        /// </summary>
        private void InvokeCancelListener()
        {
            if (_intervalCancelListener != null)
                _intervalCancelListener(_downView, _isLongPress);
        }

        /// <summary>
        /// The OnTouch listener.
        /// </summary>
        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    _repeatedHandler.PostDelayed(MoveimageRunnable, 100);
                    _downView = v;
                    _timer.Enabled = true;
                    _timer.Start();
                    _isLongPress = false;
                    InvokeInitialClickListener();
                    return true;
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    _repeatedHandler.RemoveCallbacks(MoveimageRunnable);
                    _timer.Stop();
                    InvokeCancelListener();
                    _downView = null;
                    _button.PerformClick();
                    return true;
            }

            return false;
        }

        /// <summary>
        /// The dispose method.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_timer != null)
            {
                _timer.Elapsed -= HandleTimerElapsed;
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
            _intervalClickListener = null;
            _intervalCancelListener = null;
            _initialClickListener = null;
        }
    }
}