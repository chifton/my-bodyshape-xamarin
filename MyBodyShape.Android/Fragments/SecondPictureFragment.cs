/**********************************************************/
/*************** The picture 2 fragment
/**********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using V4App = Android.Support.V4.App;

namespace MyBodyShape.Android.Fragments
{
    /// <summary>
    /// The picture 2 fragment.
    /// </summary>
    public class SecondPictureFragment : V4App.Fragment
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
            var view = inflater.Inflate(Resource.Layout.Picture2, container, false);
            var takePictureButton = view.FindViewById<Button>(Resource.Id.takepicture2Button);
            var loadPictureButton = view.FindViewById<Button>(Resource.Id.loadpicture2Button);
            takePictureButton.Click += OnTakePicture2Button_Click;
            loadPictureButton.Click += OnLoadPicture2Button_Click;
            return view;
        }

        /// <summary>
        /// The take button event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnTakePicture2Button_Click(object sender, System.EventArgs e)
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// The load button event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnLoadPicture2Button_Click(object sender, System.EventArgs e)
        {
            //throw new System.NotImplementedException();
        }
    }
}