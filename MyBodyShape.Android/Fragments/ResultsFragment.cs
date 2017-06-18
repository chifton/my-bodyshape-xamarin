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
using Android.Graphics;
using MyBodyShape.Android.Helpers;
using Android.Text;

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
        private AndroidViews.View fragmentView;

        /// <summary>
        /// The snaps results images directory path.
        /// </summary>
        private string snapsResultsDir;

        /// <summary>
        /// The total mass text view.
        /// </summary>
        private TextView totalMassText;

        /// <summary>
        /// The results image view.
        /// </summary>
        private ImageView resultsSnap;

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
                resultsSnap = fragmentView.FindViewById<ImageView>(Resource.Id.resultsSnap);
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
                    resultsSnap.SetScaleType(ImageView.ScaleType.FitCenter);
                    resultsSnap.SetImageURI(AndroidNet.Uri.FromFile(new Java.IO.File(resultsUri)));
                }
                else
                {
                    this.Activity.RunOnUiThread(() =>
                    {
                        resultsText = "<center><font color='red'>An error occured while getting results image.<br/>Try again later...</font></center>";
                    });
                } 
            }
            catch (Exception)
            {
                this.Activity.RunOnUiThread(() =>
                {
                    resultsText = "<center><font color='red'>An error occured while getting results image.<br/>Try again later...</font></center>";
                });
            }
            
            resultsText = resultsText + $"<center><font color='white'>Total Generated Mass : { resultsPrefs.GetFloat("poidstotal", 0) } kgs</font></center>";
            var errorResult = resultsPrefs.GetFloat("generationError", 0);
            if (errorResult != 0)
            {
                resultsText = resultsText + $"<center><font color='red'><br/>Error : {Math.Abs(errorResult) } %</font></center>";
            }

            // Set the text
            totalMassText.SetText(Html.FromHtml(resultsText), TextView.BufferType.Spannable);
        }
    }
}