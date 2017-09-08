/**********************************************************/
/*************** The splash activity
/**********************************************************/

using AndroidApp = Android.App;
using AndroidViews = Android.Views;
using Android.OS;
using Android.Content.PM;
using System.Threading.Tasks;
using AndroidContent = Android.Content;
using Android.Support.V7.App;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Android.Widget;
using Android.Graphics;
using Calligraphy;
using Android.Webkit;
using Android.Views;

namespace MyBodyShape.Android
{
    /// <summary>
    /// The splash activity.
    /// </summary>
    [AndroidApp.Activity(Label = "My BodyShape", Theme = "@style/splash_style", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        /// <summary>
        /// The shared preferences.
        /// </summary>
        private AndroidContent.ISharedPreferences prefs;

        /// <summary>
        /// The text view of simulations total number.
        /// </summary>
        private TextView nbSimulationsTextView;

        /// <summary>
        /// The OnCreate method.
        /// </summary>
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        /// <summary>
        /// The OnBackPressed method.
        /// </summary>
        public override void OnBackPressed()
        {
            // Do nothing.
        }

        /// <summary>
        /// The attach base context method.
        /// </summary>
        /// <param name="base"></param>
        protected override void AttachBaseContext(AndroidContent.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        /// <summary>
        /// The OnResume method.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();

            SetContentView(Resource.Layout.Splash);
            nbSimulationsTextView = FindViewById<TextView>(Resource.Id.nbSimulationsResults);
            nbSimulationsTextView.Bottom = 20;
            var splashLayout = FindViewById<RelativeLayout>(Resource.Id.splashLayout);
            
            // Rubik's Cuke.
            var webView = FindViewById<WebView>(Resource.Id.rubiksCubeSplash);
            webView.Visibility = ViewStates.Visible;
            webView.LoadUrl(string.Format("file:///android_asset/RubiksCube.html"));
            webView.SetBackgroundColor(Color.ParseColor("#000000"));
            webView.SetLayerType(LayerType.Software, null);

            // Caching
            prefs = AndroidApp.Application.Context.GetSharedPreferences("bodyshape", AndroidContent.FileCreationMode.Private);

            Task startupWork = new Task(() => { StartupBodyShapeApp(); });
            startupWork.Start();
        }

        /// <summary>
        /// The start BodyShape activity method.
        /// </summary>
        private async void StartupBodyShapeApp()
        {
            // Check and get configuration with server url extraction
            var configJsonStream = new System.IO.StreamReader(this.Assets.Open("config.json"));
            var configJsonStreamString = configJsonStream.ReadToEnd();
            var configData = JsonConvert.DeserializeObject<Dictionary<string, string>>(configJsonStreamString);
            var serverUrl = configData["server_url"];

            // Check https requests and internet connection
            var bodyShapeClient = new HttpClient()
            {
                Timeout = TimeSpan.FromMilliseconds(30000)
            };
            var uri = new Uri(serverUrl + "/Home/GetSimulationsNumber");
            try
            {
                var getResult = await bodyShapeClient.GetAsync(uri);
                if (getResult.IsSuccessStatusCode)
                {
                    // Font
                    CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                    .SetDefaultFontPath("Fonts/GARAIT.ttf")
                    .SetFontAttrId(Resource.Attribute.fontPath)
                    .Build());

                    // Display total number of simulations
                    var numberSimulationsResult = await getResult.Content.ReadAsStringAsync();
                    var numberSimulations = Convert.ToInt32(numberSimulationsResult);
                    this.RunOnUiThread(() =>
                    {
                        nbSimulationsTextView.Text = string.Format(Languages.Resources.Res_SimulationsNumber, numberSimulations);
                    });
                    await Task.Delay(3000);

                    // Check if language data is stored
                    var language = this.prefs.GetString("language", null);
                    if (language != null)
                    {
                        StartActivity(new AndroidContent.Intent(AndroidApp.Application.Context, typeof(MainActivity)));
                    }
                    else
                    {
                        StartActivity(new AndroidContent.Intent(AndroidApp.Application.Context, typeof(LanguageActivity)));
                    }               
                }
                else
                {
                    this.RunOnUiThread(() =>
                    {
                        nbSimulationsTextView.SetTextColor(Color.Red);
                        nbSimulationsTextView.TextAlignment = AndroidViews.TextAlignment.Center;
                        nbSimulationsTextView.Text = "No internet connection or the server is unavailable.\nTry again later...";
                    });
                    await Task.Delay(5000).ContinueWith((old) =>
                    {
                        Process.KillProcess(Process.MyPid());
                    });
                }
            }
            catch(Exception)
            {
                this.RunOnUiThread(() =>
                {
                    nbSimulationsTextView.SetTextColor(Color.Red);
                    nbSimulationsTextView.TextAlignment = AndroidViews.TextAlignment.Center;
                    nbSimulationsTextView.Text = "An error occured during MyBodyShape app launch.\nCheck your internet connection...\nTry again later.";
                });
                await Task.Delay(5000).ContinueWith((old) =>
                {
                   Process.KillProcess(Process.MyPid());
                });    
            }
        }
    }
}