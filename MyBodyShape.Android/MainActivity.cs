/**********************************************************/
/*************** The main activity
/**********************************************************/

using Android.App;
using Android.Runtime;
using Android.OS;
using MyBodyShape.Android.Fragments;
using MyBodyShape.Android.Adapters;
using V4App = Android.Support.V4.App;
using V4View = Android.Support.V4.View;

namespace MyBodyShape.Android
{
    /// <summary>
    /// The main activity.
    /// </summary>
    [Activity(Label = "MyBodyShape", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : V4App.FragmentActivity
    {
        /// <summary>
        /// The onCreate method for the main activity.
        /// </summary>
        /// <param name="bundle">The activity bundle.</param>
        protected override void OnCreate(Bundle bundle)
        {
            // Main view
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            // Fragments
            var fragments = new V4App.Fragment[]
            {
                new FirstPictureFragment(),
                new SecondPictureFragment(),
                new ResultsFragment(),
                new Model3DFragment()
            };

            // Titles
            var titles = CharSequence.ArrayFromStringArray(new[]
            {
                "Front Picture",
                "Side Picture",
                "Members weights",
                "3D Models"
            });

            // View pager and adapter
            var viewPager = FindViewById<V4View.ViewPager>(Resource.Id.bodyshapeViewPager);
            viewPager.Adapter = new MainAdapter(base.SupportFragmentManager, fragments, titles);
        }
    }
}

