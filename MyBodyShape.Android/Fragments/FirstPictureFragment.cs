/**********************************************************/
/*************** The picture 1 fragment
/**********************************************************/

using System;
using System.Collections.Generic;

using Java.IO;

using Android.OS;
using AndroidOS = Android.OS;
using Android.Widget;
using Android.Views;
using Android.Content;
using Android.Provider;
using Android.Content.PM;
using AndroidNet = Android.Net;
using Android.Graphics;
using V4App = Android.Support.V4.App;

using MyBodyShape.Android.Helpers;
using Android.App;

namespace MyBodyShape.Android.Fragments
{
    /// <summary>
    /// Static class for image informations.
    /// </summary>
    public static class App1
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
    }

    /// <summary>
    /// The picture 1 fragment.
    /// </summary>
    public class FirstPictureFragment : V4App.Fragment
    {
        /// <summary>
        /// The fragment view.
        /// </summary>
        private View fragmentView;

        /// <summary>
        /// The take picture button.
        /// </summary>
        private Button takePictureButton;

        /// <summary>
        /// The load picture button.
        /// </summary>
        private Button loadPictureButton;

        /// <summary>
        /// The request picture code.
        /// </summary>
        private const int requestPictureCode = 11;

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
            if(fragmentView == null)
            {
                fragmentView = inflater.Inflate(Resource.Layout.Picture1, container, false);
                takePictureButton = fragmentView.FindViewById<Button>(Resource.Id.takepicture1Button);
                loadPictureButton = fragmentView.FindViewById<Button>(Resource.Id.loadpicture1Button);
                takePictureButton.Click += OnTakePicture1Button_Click;
                loadPictureButton.Click += OnLoadPicture1Button_Click;
            }
            
            return fragmentView;
        }

        /// <summary>
        /// The take button event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnTakePicture1Button_Click(object sender, System.EventArgs e)
        {
            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                var customDate = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                var root = Guid.NewGuid().ToString() + "-" + customDate + "-Android";
                var fileName = root + "Picture_1";

                Intent intent = new Intent(MediaStore.ActionImageCapture);
                App1._file = new File(App1._dir, $"{ fileName }.png");
                intent.PutExtra(MediaStore.ExtraOutput, AndroidNet.Uri.FromFile(App1._file));
                StartActivityForResult(intent, requestPictureCode);
            }
            else
            {
                // On affiche un message à l'utilisateur
            }
        }

        /// <summary>
        /// The load button event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnLoadPicture1Button_Click(object sender, System.EventArgs e)
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// The result of the pictures.
        /// </summary>
        /// <param name="requestCode">The request code.</param>
        /// <param name="resultCode">The result code.</param>
        /// <param name="data">The picture data.</param>
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if(resultCode == -1)
            {
                // The take picture result
                if (requestCode == requestPictureCode)
                {
                    // Delete buttons
                    var linearLayout = fragmentView.FindViewById<LinearLayout>(Resource.Id.layoutPicture1Container);
                    linearLayout.RemoveAllViewsInLayout();

                    // New image view
                    ImageView imageView = new ImageView(this.Context);
                    imageView.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                    imageView.Visibility = ViewStates.Visible;
                    linearLayout.AddView(imageView);

                    // Result
                    base.OnActivityResult(requestCode, resultCode, data);

                    // Image data
                    Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                    AndroidNet.Uri contentUri = AndroidNet.Uri.FromFile(App1._file);
                    mediaScanIntent.SetData(contentUri);
                    Context.SendBroadcast(mediaScanIntent);

                    // Resize and display
                    int height = Resources.DisplayMetrics.HeightPixels;
                    int width = imageView.Width;
                    App1.bitmap = App1._file.Path.LoadAndResizeBitmap(width, height);
                    if (App1.bitmap != null)
                    {
                        imageView.SetImageBitmap(App1.bitmap);
                        App1.bitmap = null;
                    }

                    // Memory
                    GC.Collect();
                }
            }
            else
            {

            }    
        }

        /// <summary>
        /// Create a directory for the pictures if this is the first time
        /// </summary>
        private void CreateDirectoryForPictures()
        {
            App1._dir = new File(AndroidOS.Environment.GetExternalStoragePublicDirectory(AndroidOS.Environment.DirectoryPictures), "BodyShapePictures");
            if (!App1._dir.Exists())
            {
                App1._dir.Mkdirs();
            }
        }

        /// <summary>
        /// Checks whether or not the phone has a camera app
        /// </summary>
        /// <returns></returns>
        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = Context.PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
    }
}