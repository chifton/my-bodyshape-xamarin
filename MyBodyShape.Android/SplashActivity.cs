/**********************************************************/
/*************** The splash activity
/**********************************************************/

using Android.App;
using Android.OS;
using Android.Content.PM;
using System.Threading.Tasks;
using Android.Content;
using Android.Support.V7.App;

namespace MyBodyShape.Android
{
    /// <summary>
    /// The splash activity.
    /// </summary>
    [Activity(Theme = "@style/splash_style", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
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
        /// The OnResume method.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { StartupBodyShapeApp(); });
            startupWork.Start();
        }

        /// <summary>
        /// The start BodyShape activity method.
        /// </summary>
        private async void StartupBodyShapeApp()
        {
            await Task.Delay(3000);
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}