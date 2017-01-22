/**********************************************************/
/*************** The generation fragment
/**********************************************************/

using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using V4App = Android.Support.V4.App;
using Android.Webkit;
using Android.Graphics;
using MyBodyShape.Android.Helpers;
using MyBodyShape.Android.Listeners;

namespace MyBodyShape.Android.Fragments
{
    /// <summary>
    /// The generation fragment.
    /// </summary>
    public class GenerationFragment : V4App.Fragment
    {
        /// <summary>
        /// The fragment view.
        /// </summary>
        private View fragmentView;

        /// <summary>
        /// The generate model button.
        /// </summary>
        private Button generateButton;

        /// <summary>
        /// The minus height button.
        /// </summary>
        private Button minusHeightButton;

        /// <summary>
        /// The plus height button.
        /// </summary>
        private Button plusHeightButton;

        /// <summary>
        /// The height text view.
        /// </summary>
        private EditText heightTextEdit;

        /// <summary>
        /// The minus weight button.
        /// </summary>
        private Button minusWeightButton;

        /// <summary>
        /// The plus weight button.
        /// </summary>
        private Button plusWeightButton;

        /// <summary>
        /// The weight text view.
        /// </summary>
        private EditText weightTextEdit;

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
                fragmentView = inflater.Inflate(Resource.Layout.Generate, container, false);

                minusHeightButton = fragmentView.FindViewById<Button>(Resource.Id.height_btn_minus);
                plusHeightButton = fragmentView.FindViewById<Button>(Resource.Id.height_btn_plus);
                heightTextEdit = fragmentView.FindViewById<EditText>(Resource.Id.heightText);
                minusHeightButton.Click += OnMinusHeightButton_Click;
                plusHeightButton.Click += OnPlusHeightButton_Click;
                minusHeightButton.SetOnTouchListener(new EditTextRepeatListener(minusHeightButton, heightTextEdit, 1000, 2000, (button) => {}, button => {}, (button, isLongPress) => {}));
                plusHeightButton.SetOnTouchListener(new EditTextRepeatListener(plusHeightButton, heightTextEdit, 1000, 2000, (button) => { }, button => { }, (button, isLongPress) => { }));

                minusWeightButton = fragmentView.FindViewById<Button>(Resource.Id.weight_btn_minus);
                plusWeightButton = fragmentView.FindViewById<Button>(Resource.Id.weight_btn_plus);
                weightTextEdit = fragmentView.FindViewById<EditText>(Resource.Id.weightText);
                minusWeightButton.Click += OnMinusWeightButton_Click;
                plusWeightButton.Click += OnPlusWeightButton_Click;
                minusWeightButton.SetOnTouchListener(new EditTextRepeatListener(minusWeightButton, weightTextEdit, 1000, 2000, (button) => { }, button => { }, (button, isLongPress) => { }));
                plusWeightButton.SetOnTouchListener(new EditTextRepeatListener(plusWeightButton, weightTextEdit, 1000, 2000, (button) => { }, button => { }, (button, isLongPress) => { }));

                generateButton = fragmentView.FindViewById<Button>(Resource.Id.generateButton);
                generateButton.Click += OnGenerateButton_Click;
            }

            return fragmentView;
        }

        /// <summary>
        /// The minus height click.
        /// </summary>
        private void OnMinusHeightButton_Click(object sender, EventArgs e)
        {
            var currentButton = sender as Button;

            int parsedNumber;
            if (int.TryParse(this.heightTextEdit.Text, out parsedNumber))
            {
                if (parsedNumber > 70)
                {
                    heightTextEdit.Text = (parsedNumber - 1).ToString();
                }
            }
        }
        
        /// <summary>
        /// The plus height click.
        /// </summary>
        private void OnPlusHeightButton_Click(object sender, EventArgs e)
        {
            int parsedNumber;
            if (int.TryParse(this.heightTextEdit.Text, out parsedNumber))
            {
                if(parsedNumber < 250)
                {
                    heightTextEdit.Text = (parsedNumber + 1).ToString();
                }
            }
        }

        /// <summary>
        /// The minus weight click.
        /// </summary>
        private void OnMinusWeightButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(weightTextEdit.Text))
            {
                weightTextEdit.Text = "0";
            }

            int parsedNumber;
            if (int.TryParse(this.weightTextEdit.Text, out parsedNumber))
            {
                if (parsedNumber > 0)
                {
                    weightTextEdit.Text = (parsedNumber - 1).ToString();
                }
            }
        }

        /// <summary>
        /// The plus weight click.
        /// </summary>
        private void OnPlusWeightButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(weightTextEdit.Text))
            {
                weightTextEdit.Text = "0";
            }

            int parsedNumber;
            if (int.TryParse(this.weightTextEdit.Text, out parsedNumber))
            {
                if (parsedNumber < 250)
                {
                    weightTextEdit.Text = (parsedNumber + 1).ToString();
                }
            }
        }

        /// <summary>
        /// The generate button event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnGenerateButton_Click(object sender, EventArgs e)
        {
            // Send generation to API here
            

            // Rubik's Cube
            var linearLayout = fragmentView.FindViewById<LinearLayout>(Resource.Id.layoutGenerateCenter);
            linearLayout.RemoveAllViewsInLayout();

            // New web view
            WebView webView = new WebView(this.Context);
            webView.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            webView.Visibility = ViewStates.Visible;
            webView.LoadUrl(string.Format("file:///android_asset/RubiksCube.html"));
            webView.SetBackgroundColor(new Color(0, 0, 0, 0));
            webView.SetLayerType(LayerType.Software, null); 
            linearLayout.AddView(webView);

            // Disable the swipes
            var viewPager = this.Activity.FindViewById<BodyShapeViewPager>(Resource.Id.bodyshapeViewPager);
            viewPager.SetSwipeEnabled(false);
        }
    }
}