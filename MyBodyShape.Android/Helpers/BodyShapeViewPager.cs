/**********************************************************/
/*************** The bodyshape viewpager
/**********************************************************/

using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using V4App = Android.Support.V4.View;

namespace MyBodyShape.Android.Helpers
{
    /// <summary>
    /// A bodyshape viewpager.
    /// </summary>
    public class BodyShapeViewPager : V4App.ViewPager
    {
        /// <summary>
        /// The viewpager enabled state
        /// </summary>
        private bool enabled;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="attrs">The viewpager attributes.</param>
        public BodyShapeViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this.enabled = true;
        }

        /// <summary>
        /// The ontouch event.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <returns></returns>
        public override bool OnTouchEvent(MotionEvent e)
        {
            if(this.enabled)
            {
                return base.OnTouchEvent(e);
            }

            return false;
        }

        /// <summary>
        /// The onintercept touch event.
        /// </summary>
        /// <param name="ev">The event.</param>
        /// <returns></returns>
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (this.enabled)
            {
                return base.OnInterceptTouchEvent(ev);
            }

            return false;
        }

        /// <summary>
        /// The method to enable or disable the view pager.
        /// </summary>
        /// <param name="enabledOrNot"></param>
        public void SetSwipeEnabled(bool enabledOrNot)
        {
            this.enabled = enabledOrNot;
        }
    }
}