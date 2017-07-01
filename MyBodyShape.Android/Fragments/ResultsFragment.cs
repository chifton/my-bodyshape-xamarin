 /**********************************************************/
/*************** The results fragment
/**********************************************************/

using System;
using Android.App;
using Android.Content;
using AndroidNet = Android.Net;
using AndroidOS = Android.OS;
using AndroidViews = Android.Views;
using Android.Widget;
using V4App = Android.Support.V4.App;
using System.Collections.Generic;
using Newtonsoft.Json;
using MyBodyShape.Android.Helpers;
using Android.Text;
using Android.Views;

namespace MyBodyShape.Android.Fragments
{
    /// <summary>
    /// The results fragment.
    /// </summary>
    public class ResultsFragment : V4App.Fragment
    {
        /// <summary>
        /// The fragment view.
        /// </summary>
        private View fragmentView;

        /// <summary>
        /// The snaps results images directory path.
        /// </summary>
        private string snapsResultsDir;

        /// <summary>
        /// The total mass text view.
        /// </summary>
        private TextView totalMassText;

        /// <summary>
        /// The results linear layout.
        /// </summary>
        private LinearLayout resultsSnapLayouts;

        /// <summary>
        /// The OnCreate method.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        public override void OnCreate(AndroidOS.Bundle savedInstanceState)
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
        public override AndroidViews.View OnCreateView(AndroidViews.LayoutInflater inflater, AndroidViews.ViewGroup container, AndroidOS.Bundle savedInstanceState)
        {
            if (fragmentView == null)
            {
                fragmentView = inflater.Inflate(Resource.Layout.Results, container, false);
                resultsSnapLayouts = fragmentView.FindViewById<LinearLayout>(Resource.Id.layoutResults);
                totalMassText = fragmentView.FindViewById<TextView>(Resource.Id.totalMass);
                snapsResultsDir = System.IO.Path.Combine(AndroidOS.Environment.ExternalStorageDirectory.AbsolutePath, AndroidOS.Environment.DirectoryDownloads);
            }
            
            return fragmentView;
        }

        /// <summary>
        /// The show results method.
        /// </summary>
        public void ShowResults(string snapId)
        {
            // Results text
            var resultsText = string.Empty;
            totalMassText.TextAlignment = AndroidViews.TextAlignment.Center;

            // We just store resutls
            ISharedPreferences resultsPrefs = Application.Context.GetSharedPreferences("bodyshaperesults", FileCreationMode.Private);
            ISharedPreferencesEditor resultsEditor = resultsPrefs.Edit();

            // Check and get configuration with server url extraction
            var configJsonStream = new System.IO.StreamReader(this.Activity.Assets.Open("config.json"));
            var configJsonStreamString = configJsonStream.ReadToEnd();
            var configData = JsonConvert.DeserializeObject<Dictionary<string, string>>(configJsonStreamString);
            var serverUrl = configData["server_url"];

            // Download snap results image
            var desiredHeight = Resources.DisplayMetrics.HeightPixels * 70 / 100;
            var downloadUri = new Uri(serverUrl + "/Home/DownloadSnapResult?id=" + snapId + "&height=" + desiredHeight);
            var webClient = new BodyShapeWebClient();
            webClient.Timeout = 30000;
            webClient.Headers.Add("Content-Type", "binary/octet-streOpenWriteam");
            
            try
            {
                // Uri
                var resultsUri = System.IO.Path.Combine(snapsResultsDir, "snap_" + snapId + ".png");
                
                // Download results image
                webClient.DownloadFile(downloadUri, resultsUri);

                // Display results
                Java.IO.File file = new Java.IO.File(resultsUri);
                if (file.Exists())
                {
                    var resultsSnapView = new GestureRecognizerView(this.Context, AndroidNet.Uri.FromFile(new Java.IO.File(resultsUri)), resultsSnapLayouts.Width, resultsSnapLayouts.Height);
                    resultsSnapView.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                    resultsSnapView.Visibility = ViewStates.Visible;
                    resultsSnapView.SetScaleType(ImageView.ScaleType.FitCenter);
                    resultsSnapLayouts.AddView(resultsSnapView, 0);

                    var viewPager = this.Activity.FindViewById<BodyShapeViewPager>(Resource.Id.bodyshapeViewPager);
                    viewPager.SetSwipeEnabled(true);
                    viewPager.SetCurrentItem(3, true);
                }
                else
                {
                    this.Activity.RunOnUiThread(() =>
                    {
                        resultsText = "<span style='text-align:center;'><font color='red'>An error occured while getting results image.<br/>Try again later...</font><br/></span>";
                    });
                } 
            }
            catch (Exception)
            {
                this.Activity.RunOnUiThread(() =>
                {
                    resultsText = "<span style='text-align:center;'><font color='red'>An error occured while getting results image.<br/>Try again later...</font><br/></span>";
                });
            }
            
            resultsText = resultsText + $"<span style='text-align:center;'><font color='white'>Total Generated Mass : <font color='#008000'>{ resultsPrefs.GetFloat("poidstotal", 0) } kgs</font></font><br/></span>";
            var errorResult = resultsPrefs.GetFloat("generationError", 0);
            if (errorResult != 0)
            {
                resultsText = resultsText + $"<span style='text-align:center;'><font color='red'>Error : {Math.Abs(errorResult) } %</font><br/></span>";
            }

            // Set the text
            totalMassText.SetText(Html.FromHtml(resultsText), TextView.BufferType.Spannable);
        }
    }
}