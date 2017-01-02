/**********************************************************/
/*************** The generation fragment
/**********************************************************/

using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using V4App = Android.Support.V4.App;
using Android.Webkit;
using Android.Graphics;
using MyBodyShape.Android.Helpers;

namespace MyBodyShape.Android.Fragments
{
    /// <summary>
    /// The generation fragment.
    /// </summary>
    public class GenerationFragment : V4App.Fragment
    {
        /// <summary>
        /// The fragment view.
        /// </summary>
        private View fragmentView;

        /// <summary>
        /// The generate model button.
        /// </summary>
        private Button generateButton;

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
            if (fragmentView == null)
            {
                fragmentView = inflater.Inflate(Resource.Layout.Generate, container, false);
                generateButton = fragmentView.FindViewById<Button>(Resource.Id.generateButton);
                generateButton.Click += OnGenerateButton_Click;
            }

            return fragmentView;
        }

        /// <summary>
        /// The generate button event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnGenerateButton_Click(object sender, EventArgs e)
        {
            // Send generation to API here
            

            // Rubik's Cube
            var linearLayout = fragmentView.FindViewById<LinearLayout>(Resource.Id.layoutGenerateCenter);
            linearLayout.RemoveAllViewsInLayout();

            // New web view
            WebView webView = new WebView(this.Context);
            webView.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            webView.Visibility = ViewStates.Visible;
            webView.LoadUrl(string.Format("file:///android_asset/RubiksCube.html"));
            webView.SetBackgroundColor(new Color(0, 0, 0, 0));
            webView.SetLayerType(LayerType.Software, null); 
            linearLayout.AddView(webView);

            // Disable the swipes
            var viewPager = this.Activity.FindViewById<BodyShapeViewPager>(Resource.Id.bodyshapeViewPager);
            viewPager.SetSwipeEnabled(false);
        }
    }
}