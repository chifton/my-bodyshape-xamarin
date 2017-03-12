 /**********************************************************/
/*************** The results fragment
/**********************************************************/

using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using V4App = Android.Support.V4.App;


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
        /// The members views.
        /// </summary>
        private TextView headText;
        private TextView neckText;
        private TextView thoraxText;
        private TextView abdomenText;
        private TextView bottomText;
        private TextView thighleftText;
        private TextView thighrightText;
        private TextView legleftText;
        private TextView legrightText;
        //private TextView ankleleftText;
        //private TextView anklerightText;
        private TextView footleftText;
        private TextView footrightText;
        private TextView armleftText;
        private TextView armrightText;
        private TextView forearmleftText;
        private TextView forearmrightText;
        private TextView handleftText;
        private TextView handrightText;
        private TextView errorText;
        private TextView totalMassText;

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
                fragmentView = inflater.Inflate(Resource.Layout.Results, container, false);
                headText = fragmentView.FindViewById<TextView>(Resource.Id.headMass);
                neckText = fragmentView.FindViewById<TextView>(Resource.Id.neckMass);

                headText = fragmentView.FindViewById<TextView>(Resource.Id.headMass);
                neckText = fragmentView.FindViewById<TextView>(Resource.Id.neckMass);
                thoraxText = fragmentView.FindViewById<TextView>(Resource.Id.thoraxMass);
                abdomenText = fragmentView.FindViewById<TextView>(Resource.Id.abdoMass);
                bottomText = fragmentView.FindViewById<TextView>(Resource.Id.fesseMass);
                thighleftText = fragmentView.FindViewById<TextView>(Resource.Id.cuissegaucheMass);
                thighrightText = fragmentView.FindViewById<TextView>(Resource.Id.cuissedroiteMass);
                legleftText = fragmentView.FindViewById<TextView>(Resource.Id.jambegaucheMass);
                legrightText = fragmentView.FindViewById<TextView>(Resource.Id.jambedroiteMass);
                //ankleleftText = fragmentView.FindViewById<TextView>(Resource.Id.chevillegaucheMass);
                //anklerightText = fragmentView.FindViewById<TextView>(Resource.Id.chevilledroiteMass);
                footleftText = fragmentView.FindViewById<TextView>(Resource.Id.piedgaucheMass);
                footrightText = fragmentView.FindViewById<TextView>(Resource.Id.pieddroitMass);
                armleftText = fragmentView.FindViewById<TextView>(Resource.Id.brasgaucheMass);
                armrightText = fragmentView.FindViewById<TextView>(Resource.Id.brasdroitMass);
                forearmleftText = fragmentView.FindViewById<TextView>(Resource.Id.avantbrasgaucheMass);
                forearmrightText = fragmentView.FindViewById<TextView>(Resource.Id.avantbrasdroitMass);
                handleftText = fragmentView.FindViewById<TextView>(Resource.Id.maingaucheMass);
                handrightText = fragmentView.FindViewById<TextView>(Resource.Id.maindroiteMass);

                errorText = fragmentView.FindViewById<TextView>(Resource.Id.errorInfo);
                totalMassText = fragmentView.FindViewById<TextView>(Resource.Id.totalMass);
            }

            return fragmentView;
        }

        /// <summary>
        /// The show results method.
        /// </summary>
        public void ShowResults()
        {
            ISharedPreferences resultsPrefs = Application.Context.GetSharedPreferences("bodyshaperesults", FileCreationMode.Private);
            ISharedPreferencesEditor resultsEditor = resultsPrefs.Edit();

            headText.Text = $"Head : { resultsPrefs.GetFloat("headMass", 0) } kgs";
            neckText.Text = $"Neck : { resultsPrefs.GetFloat("neckMass", 0) } kgs";
            thoraxText.Text = $"Thorax : { resultsPrefs.GetFloat("thoraxMass", 0) } kgs";
            abdomenText.Text = $"Abdomen : { resultsPrefs.GetFloat("abdoMass", 0) } kgs";
            bottomText.Text = $"Bottom : { resultsPrefs.GetFloat("fesseMass", 0) } kgs";

            thighleftText.Text = $"Left Thigh : { resultsPrefs.GetFloat("cuissegaucheMass", 0) } kgs";
            thighrightText.Text = $"Right Thigh : { resultsPrefs.GetFloat("cuissedroiteMass", 0) } kgs";
            legleftText.Text = $"Left Leg : { resultsPrefs.GetFloat("jambegaucheMass", 0) } kgs";
            legrightText.Text = $"Right Leg : { resultsPrefs.GetFloat("jambedroiteMass", 0) } kgs";
            //ankleleftText.Text = $"Left Ankle : { resultsPrefs.GetFloat("chevillegaucheMass", 0) } kgs";
            //anklerightText.Text = $"Right Ankle : { resultsPrefs.GetFloat("chevilledroiteMass", 0) } kgs";
            footleftText.Text = $"Left Foot : { resultsPrefs.GetFloat("piedgaucheMass", 0) } kgs";
            footrightText.Text = $"Right Foot : { resultsPrefs.GetFloat("pieddroitMass", 0) } kgs";

            armleftText.Text = $"Left Upperarm : { resultsPrefs.GetFloat("brasgaucheMass", 0) } kgs";
            armrightText.Text = $"Right Upperarm : { resultsPrefs.GetFloat("brasdroitMass", 0) } kgs";
            forearmleftText.Text = $"Left Forearm : { resultsPrefs.GetFloat("avantbrasgaucheMass", 0) } kgs";
            forearmrightText.Text = $"Right Forearm : { resultsPrefs.GetFloat("avantbrasdroitMass", 0) } kgs";
            handleftText.Text = $"Left Hand : { resultsPrefs.GetFloat("maingaucheMass", 0) } kgs";
            handrightText.Text = $"Right Hand : { resultsPrefs.GetFloat("maindroiteMass", 0) } kgs";

            var errorResult = resultsPrefs.GetFloat("generationError", 0);
            if (errorResult != 0)
            {
                errorText.Text = $"Only {Math.Abs(errorResult) } % of error during the generation ! Congratulations !";
            }
            totalMassText.Text = $"Total Generated Mass : { resultsPrefs.GetFloat("poidstotal", 0) } kgs";
        }
    }
}