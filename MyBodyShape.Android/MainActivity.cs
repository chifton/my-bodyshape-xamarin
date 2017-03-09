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

namespace MyBodyShape.Android
{
    /// <summary>
    /// The main activity.
    /// </summary>
    [Activity(Label = "MyBodyShape", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
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
        /// The onCreate method for the main activity.
        /// </summary>
        /// <param name="bundle">The activity bundle.</param>
        protected override void OnCreate(Bundle bundle)
        {
            // Main view
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            // Clear all old data
            ISharedPreferences prefs = Application.Context.GetSharedPreferences("bodyshape", FileCreationMode.Private);
            ISharedPreferences resultsPrefs = Application.Context.GetSharedPreferences("bodyshaperesults", FileCreationMode.Private);
            ISharedPreferencesEditor editor = prefs.Edit();
            ISharedPreferencesEditor resultsEditor = resultsPrefs.Edit();
            editor.Clear();
            resultsEditor.Clear();
            editor.Commit();
            resultsEditor.Commit();

            // Fragments
            this.FirstPictureFragment = new FirstPictureFragment();
            this.SecondPictureFragment = new SecondPictureFragment();
            this.GenerationFragment = new GenerationFragment();
            this.ResultsFragment = new ResultsFragment();
            this.Model3DFragment = new Model3DFragment();
            var fragments = new V4App.Fragment[]
            {
                this.FirstPictureFragment,
                this.SecondPictureFragment,
                this.GenerationFragment,
                this.ResultsFragment,
                this.Model3DFragment
            };

            // Titles
            var titles = CharSequence.ArrayFromStringArray(new[]
            {
                "Front Picture",
                "Side Picture",
                "Generate",
                "Members weights",
                "3D Model"
            });

            // View pager and adapter
            var viewPager = FindViewById<BodyShapeViewPager>(Resource.Id.bodyshapeViewPager);
            viewPager.SetBackgroundColor(Color.Black);

            viewPager.Adapter = new MainAdapter(base.SupportFragmentManager, fragments, titles);
        }
    }
}