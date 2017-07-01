/**********************************************************/
/*************** The gesture listener
/**********************************************************/

using Android.Views;
using MyBodyShape.Android.Helpers;

namespace MyBodyShape.Android.Listeners
{
    /// <summary>
    /// The gesture listener.
    /// </summary>
    public class GestureListener : ScaleGestureDetector.SimpleOnScaleGestureListener
    {
        /// <summary>
        ///  The private controls.
        /// </summary>
        private readonly GestureRecognizerView _view;

        /// <summary>
        /// The constructor.
        /// </summary>
        public GestureListener(GestureRecognizerView view)
        {
            _view = view;
        }

        /// <summary>
        /// The onScale event.
        /// </summary>
        public override bool OnScale(ScaleGestureDetector detector)
        {
            _view.ScaleFactor *= detector.ScaleFactor;

            if (_view.ScaleFactor > 5.0f)
            {
                _view.ScaleFactor = 5.0f;
            }
            if (_view.ScaleFactor < 0.1f)
            {
                _view.ScaleFactor = 0.1f;
            }

            _view.Invalidate();
            return true;
        }
    }
}