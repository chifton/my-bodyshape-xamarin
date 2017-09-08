/**********************************************************/
/*************** The splash activity
/**********************************************************/

using AndroidApp = Android.App;
using AndroidViews = Android.Views;
using Android.OS;
using Android.Content.PM;
using V4App = Android.Support.V4.App;
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
using Android.App;

namespace MyBodyShape.Android
{
    /// <summary>
    /// The splash activity.
    /// </summary>
    [AndroidApp.Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@android:style/Theme.NoTitleBar")]
    public class LanguageActivity : Activity
    {
        /// <summary>
        /// The shared preferences.
        /// </summary>
        private AndroidContent.ISharedPreferences prefs;

        /// <summary>
        /// The shared preferences editor.
        /// </summary>
        private AndroidContent.ISharedPreferencesEditor editor;

        /// <summary>
        /// The english button.
        /// </summary>
        private ImageButton englishButton;

        /// <summary>
        /// The french button.
        /// </summary>
        private ImageButton frenchButton;

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
            base.OnBackPressed();
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

            // Caching
            prefs = AndroidApp.Application.Context.GetSharedPreferences("bodyshape", AndroidContent.FileCreationMode.Private);
            editor = prefs.Edit();

            SetContentView(Resource.Layout.Language);

            englishButton = FindViewById<ImageButton>(Resource.Id.layoutLanguageEnglish);
            englishButton.SetScaleType(ImageView.ScaleType.Center);
            englishButton.SetAdjustViewBounds(true);
            englishButton.SetBackgroundResource(Resource.Drawable.english);
            englishButton.Click += EnglishButton_Click;

            frenchButton = FindViewById<ImageButton>(Resource.Id.layoutLanguageFrench);
            frenchButton.SetScaleType(ImageView.ScaleType.Center);
            frenchButton.SetAdjustViewBounds(true);
            frenchButton.SetBackgroundResource(Resource.Drawable.french);
            frenchButton.Click += OnFrenchButton_Click;
        }

        /// <summary>
        /// The english button handler.
        /// </summary>
        private void EnglishButton_Click(object sender, EventArgs e)
        {
            this.StartupBodyShapeApp("en-US");
        }

        /// <summary>
        /// The french button handler.
        /// </summary>
        private void OnFrenchButton_Click(object sender, EventArgs e)
        {
            this.StartupBodyShapeApp("fr-FR");
        }

        /// <summary>
        /// The start BodyShape activity method.
        /// </summary>
        private void StartupBodyShapeApp(string languageCode)
        {
            // Save the language
            var language = this.prefs.GetString("language", null);
            if (language != null)
            {
                this.editor.Remove("language");
            }
            this.editor.PutString("language", languageCode);
            this.editor.Commit();

            StartActivity(new AndroidContent.Intent(AndroidApp.Application.Context, typeof(MainActivity)));
        }
    }
}