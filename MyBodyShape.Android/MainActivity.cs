/**********************************************************/
/*************** The main activity
/**********************************************************/

using Android.App;
using Android.Runtime;
using Android.OS;
using MyBodyShape.Android.Fragments;
using MyBodyShape.Android.Adapters;
using V4App = Android.Support.V4.App;
using Android.Content.PM;
using MyBodyShape.Android.Helpers;
using Android.Graphics;
using Android.Content;
using AndroidSupport = Android.Support;
using AndroidViews = Android.Views;
using AndroidWidgets = Android.Widget;
using System;
using Android.Graphics.Drawables;
using System.Threading;

namespace MyBodyShape.Android
{
    /// <summary>
    /// The main activity.
    /// </summary>
    [Activity(Label = "MyBodyShape", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : V4App.FragmentActivity
    {
        /// <summary>
        /// The first picture fragment.
        /// </summary>
        public FirstPictureFragment FirstPictureFragment { get; set; }

        /// <summary>
        /// The second picture fragment.
        /// </summary>
        public SecondPictureFragment SecondPictureFragment { get; set; }

        /// <summary>
        /// The generation fragment.
        /// </summary>
        public GenerationFragment GenerationFragment { get; set; }

        /// <summary>
        /// The results fragment.
        /// </summary>
        public ResultsFragment ResultsFragment { get; set; }

        /// <summary>
        /// The 3d model fragment.
        /// </summary>
        public Model3DFragment Model3DFragment { get; set; }

        /// <summary>
        /// The feedback fragment.
        /// </summary>
        public FeedBackFragment FeedbackFragment { get; set; }

        /// <summary>
        /// The private policy fragment.
        /// </summary>
        public PrivacyPolicyFragment PrivacyPolicyFragment { get; set; }

        /// <summary>
        /// The onCreate method for the main activity.
        /// </summary>
        /// <param name="bundle">The activity bundle.</param>
        protected override void OnCreate(Bundle bundle)
        {
            // Main view
            base.OnCreate(bundle);
            RequestWindowFeature(AndroidViews.WindowFeatures.CustomTitle);
            SetContentView(Resource.Layout.Main);

            // Custom activity
            Window.SetFeatureInt(AndroidViews.WindowFeatures.CustomTitle, Resource.Layout.window_title);

            // Get the language
            var storedLanguage = string.Empty;
            ISharedPreferences prefs = Application.Context.GetSharedPreferences("bodyshape", FileCreationMode.Private);
            ISharedPreferences resultsPrefs = Application.Context.GetSharedPreferences("bodyshaperesults", FileCreationMode.Private);
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            Languages.Resources.Culture = new System.Globalization.CultureInfo("en-US");
            var language = prefs.GetString("language", null);
            if (language != null)
            {
                storedLanguage = language;
            }

            // Clear all old data except language
            ISharedPreferencesEditor editor = prefs.Edit();
            ISharedPreferencesEditor resultsEditor = resultsPrefs.Edit();
            editor.Clear();
            resultsEditor.Clear();
            editor.Commit();
            resultsEditor.Commit();
            
            // Restore stored language
            if (!string.IsNullOrEmpty(storedLanguage))
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(language);
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
                Languages.Resources.Culture = new System.Globalization.CultureInfo(language);

                // Store for future launch
                editor.PutString("language", storedLanguage);
                editor.Commit();
            }
            
            // Fragments
            this.FirstPictureFragment = new FirstPictureFragment();
            this.SecondPictureFragment = new SecondPictureFragment();
            this.GenerationFragment = new GenerationFragment();
            this.ResultsFragment = new ResultsFragment();
            this.Model3DFragment = new Model3DFragment();
            this.FeedbackFragment = new FeedBackFragment();
            this.PrivacyPolicyFragment = new PrivacyPolicyFragment();
            var fragments = new V4App.Fragment[]
            {
                this.FirstPictureFragment,
                this.SecondPictureFragment,
                this.GenerationFragment,
                this.ResultsFragment,
                this.Model3DFragment,
                this.FeedbackFragment,
                this.PrivacyPolicyFragment
            };

            // Titles
            var titles = CharSequence.ArrayFromStringArray(new[]
            {
                Languages.Resources.Res_Android_FrontTitle,
                Languages.Resources.Res_Android_SideTitle,
                Languages.Resources.Res_Android_Generate,
                Languages.Resources.Res_Android_MemberWeights,
                Languages.Resources.Res_Android_3dBodyShape,
                Languages.Resources.Res_Android_Feedback,
                Languages.Resources.Res_Android_Privacy
            });

            // Tabs icons
            var rootFrontIcon = AndroidSupport.V4.Content.ContextCompat.GetDrawable(Application.Context, Resource.Drawable.front_icon);
            Bitmap bitmapFront = ((BitmapDrawable)rootFrontIcon).Bitmap;
            var iconFront = new BitmapDrawable(Resources, Bitmap.CreateScaledBitmap(bitmapFront, 20, 60, true));
            var rootSideIcon = AndroidSupport.V4.Content.ContextCompat.GetDrawable(Application.Context, Resource.Drawable.icon_side);
            Bitmap bitmapSide = ((BitmapDrawable)rootSideIcon).Bitmap;
            var iconSide = new BitmapDrawable(Resources, Bitmap.CreateScaledBitmap(bitmapSide, 15, 60, true));
            var rootGenerationIcon = AndroidSupport.V4.Content.ContextCompat.GetDrawable(Application.Context, Resource.Drawable.rubis);
            Bitmap bitmapGeneration = ((BitmapDrawable)rootGenerationIcon).Bitmap;
            var iconGeneration = new BitmapDrawable(Resources, Bitmap.CreateScaledBitmap(bitmapGeneration, 50, 50, true));
            var root3DIcon = AndroidSupport.V4.Content.ContextCompat.GetDrawable(Application.Context, Resource.Drawable.icon_3d);
            Bitmap bitmap3D = ((BitmapDrawable)root3DIcon).Bitmap;
            var icon3D = new BitmapDrawable(Resources, Bitmap.CreateScaledBitmap(bitmap3D, 50, 50, true));
            var rootResultsIcon = AndroidSupport.V4.Content.ContextCompat.GetDrawable(Application.Context, Resource.Drawable.results);
            Bitmap bitmapResults = ((BitmapDrawable)rootResultsIcon).Bitmap;
            var iconResults = new BitmapDrawable(Resources, Bitmap.CreateScaledBitmap(bitmapResults, 50, 50, true));
            var rootFeedbackIcon = AndroidSupport.V4.Content.ContextCompat.GetDrawable(Application.Context, Resource.Drawable.calepin);
            Bitmap bitmapFeedback = ((BitmapDrawable)rootFeedbackIcon).Bitmap;
            var iconFeedback = new BitmapDrawable(Resources, Bitmap.CreateScaledBitmap(bitmapFeedback, 50, 50, true));
            var rootPrivatePolicyIcon = AndroidSupport.V4.Content.ContextCompat.GetDrawable(Application.Context, Resource.Drawable.justice);
            Bitmap bitmapPrivatePolicy = ((BitmapDrawable)rootPrivatePolicyIcon).Bitmap;
            var iconPrivatePolicy = new BitmapDrawable(Resources, Bitmap.CreateScaledBitmap(bitmapPrivatePolicy, 50, 50, true));
            
            // View pager and adapter
            var viewPager = FindViewById<BodyShapeViewPager>(Resource.Id.bodyshapeViewPager);
            viewPager.SetBackgroundColor(Color.Black);

            viewPager.Adapter = new MainAdapter(base.SupportFragmentManager, fragments, titles, iconFront, iconSide, iconGeneration, icon3D, iconResults, iconFeedback, iconPrivatePolicy);

            // Top tabs events
            var languageButton = FindViewById<AndroidWidgets.ImageView>(Resource.Id.iconlanguage);
            languageButton.Click += LanguageButton_Click;
            var loginButton = FindViewById<AndroidWidgets.ImageView>(Resource.Id.iconsign);
            loginButton.Click += LoginButton_Click;
            var helpButton = FindViewById<AndroidWidgets.ImageView>(Resource.Id.iconhelp);
            helpButton.Click += HelpButton_Click;
        }

        /// <summary>
        /// The help button click.
        /// </summary>
        private void HelpButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The login button click.
        /// </summary>
        private void LoginButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The languageg button click.
        /// </summary>
        private void LanguageButton_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(LanguageActivity)));
        }

        /// <summary>
        /// The OnDestroy method
        /// </summary>
        protected override void OnDestroy()
        {
            // Empty total memory
            GC.Collect();

            base.OnDestroy();
        }
    }
}