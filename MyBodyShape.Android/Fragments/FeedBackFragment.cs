/**********************************************************/
/*************** The feedback fragment
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
using System.Net.Http;
using MyBodyShape.Android.Helpers;
using Android.Views.InputMethods;

namespace MyBodyShape.Android.Fragments
{
    /// <summary>
    /// The feedback fragment.
    /// </summary>
    public class FeedBackFragment : V4App.Fragment
    {
        /// <summary>
        /// The fragment view.
        /// </summary>
        private View fragmentView;

        /// <summary>
        /// The feedback uri.
        /// </summary>
        private Uri serverFeedBackUri;

        /// <summary>
        /// The feedback button.
        /// </summary>
        private Button feedBackButton;

        /// <summary>
        /// The feedback edit text.
        /// </summary>
        private EditText feedbackEditText;

        /// <summary>
        /// The bodyshape client.
        /// </summary>
        private HttpClient bodyShapeClient;

        /// <summary>
        /// The bodyshape view pager.
        /// </summary>
        private BodyShapeViewPager viewPager;

        /// <summary>
        /// The server url.
        /// </summary>
        private string serverUrl;

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
                fragmentView = inflater.Inflate(Resource.Layout.Feedback, container, false);
                feedbackEditText = fragmentView.FindViewById<EditText>(Resource.Id.feedBackEditText);
                feedBackButton = fragmentView.FindViewById<Button>(Resource.Id.feedBackButton);
                feedBackButton.Text = Languages.Resources.Res_Android_FeedbackButton;
                viewPager = this.Activity.FindViewById<BodyShapeViewPager>(Resource.Id.bodyshapeViewPager);
                feedBackButton.Click += OnFeedBackButton_Click;

                // Check and get configuration with server url extraction, then create http client
                var configJsonStream = new System.IO.StreamReader(this.Activity.Assets.Open("config.json"));
                var configJsonStreamString = configJsonStream.ReadToEnd();
                var configData = JsonConvert.DeserializeObject<Dictionary<string, string>>(configJsonStreamString);
                serverUrl = configData["server_url"];
                bodyShapeClient = new HttpClient()
                {
                    Timeout = TimeSpan.FromMilliseconds(30000)
                };
                serverFeedBackUri = new Uri(serverUrl + "/Home/FeedBack");
            }

            return fragmentView;
        }

        /// <summary>
        /// The feedback button click.
        /// </summary>
        private void OnFeedBackButton_Click(object sender, EventArgs e)
        {
            try
            {
                var text = feedbackEditText.Text;
                if (!string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text))
                {
                    var feedbackContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("mailContent", "*****  ANDROID *****     " + text)
                    });

                    var postResult = bodyShapeClient.PostAsync(serverFeedBackUri, feedbackContent).Result;
                    if (postResult.IsSuccessStatusCode)
                    {
                        var message = new AlertDialog.Builder(this.Activity);
                        message.SetMessage("Your feedback has been successfully sent. Thank you !");
                        message.Show();
                        
                        feedbackEditText.Text = string.Empty;
                        InputMethodManager inputManager = (InputMethodManager) this.Context.GetSystemService(Context.InputMethodService);
                        inputManager.HideSoftInputFromWindow(this.Activity.CurrentFocus.WindowToken, 0);

                        viewPager.SetCurrentItem(2, true);
                    }
                    else
                    {
                        var message = new AlertDialog.Builder(this.Activity);
                        message.SetMessage("An error occured during sending your feedback. Try again later.");
                        message.Show();
                    }
                }
                else
                {
                    var message = new AlertDialog.Builder(this.Activity);
                    message.SetMessage("Your feedback is empty. Please fill it...");
                    message.Show();
                }
            }
            catch (Exception)
            {
                var message = new AlertDialog.Builder(this.Activity);
                message.SetMessage("An error occured during sending your feedback. Try again later.");
                message.Show();
            }
        }
    }
}