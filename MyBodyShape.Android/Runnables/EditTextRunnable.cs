/**********************************************************/
/*************** The edit text runnable
/**********************************************************/

using Android.OS;
using Android.Widget;
using Java.Lang;

namespace MyBodyShape.Android.Listeners
{
    /// <summary>
    /// The edit text runnable.
    /// </summary>
    public class EditTextRunnable : Java.Lang.Object, IRunnable
    {
        /// <summary>
        /// The current edit text.
        /// </summary>
        private EditText _editText;

        /// <summary>
        /// The repeated handler.
        /// </summary>
        private Handler _repeatedHandler;

        /// <summary>
        /// The current button.
        /// </summary>
        private Button _currentButton;

        /// <summary>
        /// The constructor.
        /// </summary>
        public EditTextRunnable(EditText editText, Handler repeatedHandler, Button currentButton)
        {
            _editText = editText;
            _repeatedHandler = repeatedHandler;
            _currentButton = currentButton;
        }

        /// <summary>
        /// The Run method.
        /// </summary>
        public void Run()
        {
            if(_editText.Id == Resource.Id.weightText)
            {
                if (string.IsNullOrEmpty(_editText.Text))
                {
                    _editText.Text = "0";
                }
            }

            int parsedNumber;
            if (int.TryParse(this._editText.Text, out parsedNumber))
            {
                switch (_currentButton.Id)
                {
                    case Resource.Id.height_btn_minus:
                        if (parsedNumber > 70)
                        {
                            _editText.Text = (parsedNumber - 1).ToString();
                        }
                        break;
                    case Resource.Id.height_btn_plus:
                        if (parsedNumber < 250)
                        {
                            _editText.Text = (parsedNumber + 1).ToString();
                        }
                        break;
                    case Resource.Id.weight_btn_minus:
                        if (parsedNumber > 0)
                        {
                            _editText.Text = (parsedNumber - 1).ToString();
                        }
                        break;
                    case Resource.Id.weight_btn_plus:
                        if (parsedNumber < 250)
                        {
                            _editText.Text = (parsedNumber + 1).ToString();
                        }
                        break;
                    default:
                        break;
                }
            }
            
            _repeatedHandler.PostDelayed(this, 250);
        }
    }
}