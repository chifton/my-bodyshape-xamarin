/**********************************************************/
/*************** The picture 1 fragment
/**********************************************************/

using System;
using System.Linq;
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
using Android.Graphics.Drawables;

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
        /// The zoomable image view.
        /// </summary>
        private ZoomableImageView imageView;

        /// <summary>
        /// The temporary canvas.
        /// </summary>
        private Canvas tempCanvas;

        /// <summary>
        /// The temporary paint.
        /// </summary>
        private Paint tempPaint;

        /// <summary>
        /// The temporary bitmap.
        /// </summary>
        private Bitmap tempBitmap;

        /// <summary>
        /// The circles list.
        /// </summary>
        private List<CircleArea> circlesList;

        /// <summary>
        /// The current moving circle area.
        /// </summary>
        private CircleArea currentCircle;

        /// <summary>
        /// The take picture button.
        /// </summary>
        private Button takePictureButton;

        /// <summary>
        /// The load picture button.
        /// </summary>
        private Button loadPictureButton;

        /// <summary>
        /// The picture x ratio.
        /// </summary>
        private float xRatio;

        /// <summary>
        /// The picture y ratio.
        /// </summary>
        private float yRatio;

        /// <summary>
        /// The intrisic width.
        /// </summary>
        private float intrinsicWidth;

        /// <summary>
        /// The intrisic height.
        /// </summary>
        private float intrinsicHeight;

        /// <summary>
        /// The picture bitmap ratio.
        /// </summary>
        private float bitmapRatio;

        /// <summary>
        /// The zooming point of the image view.
        /// </summary>
        private Point zoomPoint;

        /// <summary>
        /// The zooming point of the bitmap.
        /// </summary>
        private Point zoomBitmapPoint;

        /// <summary>
        /// The request picture code.
        /// </summary>
        private const int takePictureCode = 11;

        /// <summary>
        /// The load picture code.
        /// </summary>
        private const int loadPictureCode = 21;

        /// <summary>
        /// The circle radius.
        /// </summary>
        private const int rootRadius = 50;

        /// <summary>
        /// The nearest distance.
        /// </summary>
        private const int nearestDistance = 100;

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
        private void OnTakePicture1Button_Click(object sender, EventArgs e)
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
                StartActivityForResult(intent, takePictureCode);
            }
            else
            {
                var message = new AlertDialog.Builder(this.Activity);
                message.SetMessage("Your mobile device has no app for taking pictures.");
                message.Show();
            }
        }

        /// <summary>
        /// The load button event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnLoadPicture1Button_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), loadPictureCode);
        }

        /// <summary>
        /// The result of the pictures.
        /// </summary>
        /// <param name="requestCode">The request code.</param>
        /// <param name="resultCode">The result code.</param>
        /// <param name="data">The picture data.</param>
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (resultCode == -1)
            {
                // Delete buttons
                var linearLayout = fragmentView.FindViewById<LinearLayout>(Resource.Id.layoutPicture1Container);
                linearLayout.RemoveAllViewsInLayout();

                // New image view
                imageView = new ZoomableImageView(this.Context);
                imageView.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                imageView.Visibility = ViewStates.Visible;
                linearLayout.AddView(imageView);

                // For further zooms
                zoomPoint = new Point();
                zoomBitmapPoint = new Point();

                // The take picture result
                if (requestCode == takePictureCode)
                {
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
                        tempBitmap = Bitmap.CreateBitmap(App1.bitmap.Width, App1.bitmap.Height, Bitmap.Config.Rgb565);
                        bitmapRatio = (float) tempBitmap.Height / tempBitmap.Width;
                        tempCanvas = new Canvas(tempBitmap);
                        tempCanvas.DrawBitmap(App1.bitmap, 0, 0, null);
                        imageView.SetImageDrawable(new BitmapDrawable(this.Resources, tempBitmap));
                        imageView.SetPaintShader(new BitmapShader(App1.bitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp));
                        imageView.Touch += OnBodyShapeTouchEvent;
                        
                        // Original image dimensions
                        Drawable drawable = imageView.Drawable;
                        intrinsicWidth = drawable.IntrinsicWidth;
                        intrinsicHeight = drawable.IntrinsicHeight;

                        // Draw front skeleton
                        this.DrawFrontSkeleton();
                    }

                    // Memory
                    GC.Collect();
                }
                // The load picture result
                else if (requestCode == loadPictureCode)
                {
                    if (data != null)
                    {
                        // Get the loaded image
                        AndroidNet.Uri uri = data.Data;

                        // Resize and display
                        int height = Resources.DisplayMetrics.HeightPixels;
                        int width = fragmentView.Width;
                        Bitmap resizedBitmap = MediaStore.Images.Media.GetBitmap(this.Activity.ContentResolver, uri);
                        var loadedBitmap = uri.Path.LoadInGalleryAndResizeBitmap(width, height, resizedBitmap);
                        if (loadedBitmap != null)
                        {
                            imageView.SetImageBitmap(loadedBitmap);
                        }

                        // Memory
                        GC.Collect();
                    }
                    else
                    {
                        var message = new AlertDialog.Builder(this.Activity);
                        message.SetMessage("No picture was found.");
                        message.Show();
                    }
                }
            }
            else
            {
                var message = new AlertDialog.Builder(this.Activity);
                message.SetMessage("An error occured during taking pictures.");
                message.Show();
            }
        }
       
        /// <summary>
        /// The draw skeleton method.
        /// </summary>
        private void DrawFrontSkeleton()
        {
            // Initialization
            circlesList = new List<CircleArea>();
            tempPaint = new Paint(PaintFlags.AntiAlias)
            {
                StrokeWidth = 10
            };
            var centerBitmap = (tempBitmap.Width - rootRadius) / 2;

            // Head
            circlesList.Add(this.DrawCircleArea(Color.Red, centerBitmap, 50, "head1"));
        }

        /// <summary>
        /// The draw circle area method.
        /// </summary>
        private CircleArea DrawCircleArea(Color color, float xPos, float yPos, string id)
        {
            tempPaint.Color = color;
            tempCanvas.DrawCircle(xPos, yPos, rootRadius, tempPaint);
            return new CircleArea(id, xPos, yPos, color);
        }

        /// <summary>
        /// The get nearest circle area method.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns></returns>
        private CircleArea GetNearestCircle(float x, float y)
        {
            var nearestPoint = this.circlesList.OrderBy(v => Math.Sqrt(Math.Pow(v.PositionX - x, 2) + Math.Pow(v.PositionY - y, 2))).FirstOrDefault();
            var distanceFromTouch = Math.Sqrt(Math.Pow(nearestPoint.PositionX - x, 2) + Math.Pow(nearestPoint.PositionY - y, 2));
            return distanceFromTouch < nearestDistance ? nearestPoint : null;
        }

        /// <summary>
        /// The ReDrawAll method at every move.
        /// </summary>
        private void ReDrawAll()
        {
            tempCanvas.DrawBitmap(App1.bitmap, 0, 0, tempPaint);
            foreach (CircleArea circle in circlesList)
            {
                tempPaint.Color = circle.Color;
                tempCanvas.DrawCircle(circle.PositionX, circle.PositionY, rootRadius, tempPaint);
            }            
        }

        /// <summary>
        /// The OnBodyShapTouch event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnBodyShapeTouchEvent(object sender, View.TouchEventArgs e)
        {
            // Bitmap coordinates calculation

            var viewer = sender as ImageView;
            float calculatedDrawLeft = 0;
            float calculatedDrawTop = 0;
            float calculatedDrawHeight = 0;
            float calculatedDrawWidth = 0;
            float calculatedBitmapRatio = (float) tempBitmap.Width / tempBitmap.Height;
            float calculatedimageViewRatio = (float) viewer.Width / viewer.Height;
            if (calculatedBitmapRatio > calculatedimageViewRatio)
            {
                calculatedDrawLeft = 0;
                calculatedDrawHeight = (calculatedimageViewRatio / calculatedBitmapRatio) * viewer.Height;
                calculatedDrawTop = (viewer.Height - calculatedDrawHeight) / 2;
            }
            else
            {
                calculatedDrawTop = 0;
                calculatedDrawWidth = (calculatedBitmapRatio / calculatedimageViewRatio) * viewer.Width;
                calculatedDrawLeft = (viewer.Width - calculatedDrawWidth) / 2;
            }

            if (xRatio == 0)
            {
                int scaledWidth = viewer.Width;
                xRatio = intrinsicWidth / scaledWidth;
            }
            if (yRatio == 0)
            {
                int scaledHeight = viewer.Height;
                yRatio = intrinsicHeight / scaledHeight;
            }

            var eventX = e.Event.GetX();
            var eventY = e.Event.GetY();
            var x = (eventX + calculatedDrawLeft) * xRatio;
            var y = (eventY + calculatedDrawTop) * yRatio * Resources.DisplayMetrics.HeightPixels / viewer.Height - 4 * calculatedDrawTop;

            // Zoom coordinates
            zoomPoint.X = (int) eventX;
            zoomPoint.Y = (int) eventY;
            zoomBitmapPoint.X = (int) x;
            zoomBitmapPoint.Y = (int) y;

            if (e.Event.Action == MotionEventActions.Down)
            {
                // Vibration
                Vibrator vibrator = (Vibrator) Activity.GetSystemService(Context.VibratorService);
                vibrator.Vibrate(50);

                // Get nearest circle
                currentCircle = this.GetNearestCircle(x, y);

                // Zoom
                imageView.EnableZoom(zoomPoint, zoomBitmapPoint);
            }
            else if (e.Event.Action == MotionEventActions.Move)
            {
                if(currentCircle != null)
                {
                    // Redraw
                    currentCircle.UpdatePosition(x, y);
                    this.ReDrawAll();
                }
            }
            else if (e.Event.Action == MotionEventActions.Up)
            {
                // UnZoom
                imageView.DisableZoom();

                currentCircle = null;
            }

            fragmentView.Invalidate();
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