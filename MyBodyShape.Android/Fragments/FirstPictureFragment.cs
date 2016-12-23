/**********************************************************/
/*************** The picture 1 fragment
/**********************************************************/

using Android.OS;
using Android.Widget;
using Android.Views;
using V4App = Android.Support.V4.App;

namespace MyBodyShape.Android.Fragments
{
    /// <summary>
    /// The picture 1 fragment.
    /// </summary>
    public class FirstPictureFragment : V4App.Fragment
    {
        /// <summary>
        /// The OnCreate method.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        /// <summary>
        /// The OnCreateView method.
        /// </summary>
        /// <param name="inflater">The inflater.</param>
        /// <param name="container">The container.</param>
        /// <param name="savedInstanceState">The saved instance state.</param>
        /// <returns></returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Picture1, container, false);
            var takePictureButton = view.FindViewById<Button>(Resource.Id.takepicture1Button);
            var loadPictureButton = view.FindViewById<Button>(Resource.Id.loadpicture1Button);
            takePictureButton.Click += OnTakePicture1Button_Click;
            loadPictureButton.Click += OnLoadPicture1Button_Click;
            return view;
        }

        /// <summary>
        /// The take button event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnTakePicture1Button_Click(object sender, System.EventArgs e)
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// The load button event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnLoadPicture1Button_Click(object sender, System.EventArgs e)
        {
            //throw new System.NotImplementedException();
        }
    }
}