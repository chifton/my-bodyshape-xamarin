/**********************************************************/
/*************** The edit text repeat listener
/**********************************************************/

using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Timers;
using static Android.Views.View;

namespace MyBodyShape.Android.Listeners
{
    /// <summary>
    /// The edit text repeat listener.
    /// </summary>
    public class EditTextRepeatListener : Java.Lang.Object, IOnTouchListener
    {
        /// <summary>
        ///  The private controls.
        /// </summary>
        private Handler _repeatedHandler;
        private EditTextRunnable _editTextRunnable;
        private EditText _editText;
        private Button _button;
        private View _downView;
        private int _normalInterval;
        private int _initialInterval;
        private Action<View> _intervalClickListener;
        private Action<View> _initialClickListener;
        private Action<View, bool> _intervalCancelListener;
        private Timer _timer;
        private bool _isLongPress = false;

        /// <summary>
        /// The constructor.
        /// </summary>
        public EditTextRepeatListener(Button button, EditText editText, int initialInterval, int normalInterval, Action<View> clickListener, Action<View> initialClickListener, Action<View, bool> cancelListener = null)
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
            _editText = editText;
            _button = button;
            _editTextRunnable = new EditTextRunnable(_editText, _repeatedHandler, _button);
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
                    _repeatedHandler.PostDelayed(_editTextRunnable, 250);
                    _downView = v;
                    v.SetBackgroundColor(Color.RoyalBlue);
                    _timer.Enabled = true;
                    _timer.Start();
                    _isLongPress = false;
                    InvokeInitialClickListener();
                    return true;
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    _repeatedHandler.RemoveCallbacks(_editTextRunnable);
                    v.SetBackgroundColor(Color.CornflowerBlue);
                    _timer.Stop();
                    InvokeCancelListener();
                    _downView = null;
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