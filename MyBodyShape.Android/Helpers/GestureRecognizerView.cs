/**********************************************************/
/*************** The gesture recognizer view
/**********************************************************/

using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using AndroidViews = Android.Views;
using MyBodyShape.Android.Listeners;
using Android.Views;
using Android.Widget;
using AndroidNet = Android.Net;
using System;

namespace MyBodyShape.Android.Helpers
{
    public class GestureRecognizerView : ImageView
    {
        /// <summary>
        /// The gesture recognier view variables.
        /// </summary>
        private static readonly int InvalidPointerId = -1;
        private readonly Drawable _resultsImage;
        private readonly AndroidViews.ScaleGestureDetector _scaleDetector;
        private int _activePointerId = InvalidPointerId;
        private float _lastTouchX;
        private float _lastTouchY;
        private float _posX;
        private float _posY;
        public float ScaleFactor = 1.0f;

        /// <summary>
        /// The constructor.
        /// </summary>
        public GestureRecognizerView(Context context, AndroidNet.Uri uri, int width, int height) : base(context, null, 0)
        {
            this.SetImageURI(uri);
            _resultsImage = this.Drawable;
            this.SetImageDrawable(null);
            var xLag = (width - _resultsImage.IntrinsicWidth) / 2;
            var yLag = (height - _resultsImage.IntrinsicHeight) / 2;
            _resultsImage.SetBounds(xLag, yLag, _resultsImage.IntrinsicWidth + xLag, _resultsImage.IntrinsicHeight);
            _scaleDetector = new AndroidViews.ScaleGestureDetector(context, new GestureListener(this));
        }
        
        /// <summary>
        /// The onDraw event.
        /// </summary>
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            canvas.Save();
            canvas.Translate(_posX, _posY);
            canvas.Scale(ScaleFactor, ScaleFactor);
            _resultsImage.Draw(canvas);
            canvas.Restore();
        }

        /// <summary>
        /// The OnTouch event.
        /// </summary>
        public override bool OnTouchEvent(MotionEvent ev)
        {
            _scaleDetector.OnTouchEvent(ev);

            MotionEventActions action = ev.Action & MotionEventActions.Mask;
            int pointerIndex;

            switch (action)
            {
                case MotionEventActions.Down:
                    _lastTouchX = ev.GetX();
                    _lastTouchY = ev.GetY();
                    _activePointerId = ev.GetPointerId(0);
                    break;

                case MotionEventActions.Move:
                    pointerIndex = ev.FindPointerIndex(_activePointerId);
                    float x = ev.GetX(pointerIndex);
                    float y = ev.GetY(pointerIndex);
                    if (!_scaleDetector.IsInProgress)
                    {
                        // Only move the ScaleGestureDetector isn't already processing a gesture.
                        float deltaX = x - _lastTouchX;
                        float deltaY = y - _lastTouchY;
                        _posX += deltaX;
                        _posY += deltaY;
                        Invalidate();
                    }

                    _lastTouchX = x;
                    _lastTouchY = y;
                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    // We no longer need to keep track of the active pointer.
                    _activePointerId = InvalidPointerId;
                    break;

                case MotionEventActions.PointerUp:
                    // check to make sure that the pointer that went up is for the gesture we're tracking.
                    pointerIndex = (int)(ev.Action & MotionEventActions.PointerIndexMask) >> (int)MotionEventActions.PointerIndexShift;
                    int pointerId = ev.GetPointerId(pointerIndex);
                    if (pointerId == _activePointerId)
                    {
                        // This was our active pointer going up. Choose a new
                        // action pointer and adjust accordingly
                        int newPointerIndex = pointerIndex == 0 ? 1 : 0;
                        _lastTouchX = ev.GetX(newPointerIndex);
                        _lastTouchY = ev.GetY(newPointerIndex);
                        _activePointerId = ev.GetPointerId(newPointerIndex);
                    }
                    break;
            }
            return true;
        }
    }
}