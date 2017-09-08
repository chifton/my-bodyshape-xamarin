/**********************************************************/
/*************** The privacy policy fragment
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
using Newtonsoft.Json;
using Android.Webkit;
using Android.Graphics;

namespace MyBodyShape.Android.Fragments
{
    /// <summary>
    /// The privacy policy fragment.
    /// </summary>
    public class PrivacyPolicyFragment : V4App.Fragment
    {
        /// <summary>
        /// The fragment view.
        /// </summary>
        private View fragmentView;

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
                // Extract server url
                var configJsonStream = new System.IO.StreamReader(this.Activity.Assets.Open("config.json"));
                var configJsonStreamString = configJsonStream.ReadToEnd();
                var configData = JsonConvert.DeserializeObject<Dictionary<string, string>>(configJsonStreamString);
                var serverUrl = configData["server_url"];

                fragmentView = inflater.Inflate(Resource.Layout.PrivacyPolicy, container, false);

                // Privacy policy website
                var privacyPolicyLayout = fragmentView.FindViewById<FrameLayout>(Resource.Id.privacyPolicyLayout);
                WebView webView = new WebView(this.Context);
                var webLayoutParams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                webView.Visibility = ViewStates.Visible;
                webView.Settings.JavaScriptEnabled = true;
                var languageUrlPrivatePolicy = Languages.Resources.Culture.Name == "en-US" ? "PrivacyPolicy.html" : "PrivacyPolicy_Fr.html";
                webView.LoadUrl(serverUrl + "/Static/" + languageUrlPrivatePolicy);
                webView.SetBackgroundColor(Color.ParseColor("#000000"));
                webView.SetLayerType(LayerType.Software, null);
                privacyPolicyLayout.AddView(webView);
            }

            return fragmentView;
        }
    }
}