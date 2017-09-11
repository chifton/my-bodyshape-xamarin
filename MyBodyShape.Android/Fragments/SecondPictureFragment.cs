/**********************************************************/
/*************** The picture 2 fragment
/**********************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

using Java.IO;

using Android.OS;
using AndroidOS = Android.OS;
using Android.Widget;
using Android.Views;
using AndroidContent = Android.Content;
using Android.Provider;
using Android.Content.PM;
using AndroidNet = Android.Net;
using Android.Graphics;
using Android.Support.V4.Content;
using Android;
using Android.Runtime;
using Android.App;
using Android.Graphics.Drawables;
using V4App = Android.Support.V4.App;

using Newtonsoft.Json;

using MyBodyShape.Android.Helpers;
using MyBodyShape.Android.Listeners;


namespace MyBodyShape.Android.Fragments
{
    /// <summary>
    /// Static class for image informations.
    /// </summary>
    public static class App2
    {
        public static File _file;
        public static File _dir;
        public static string _path;
        public static Bitmap bitmap;
    }

    /// <summary>
    /// The picture 2 fragment.
    /// </summary>
    public class SecondPictureFragment : V4App.Fragment
    {
        /// <summary>
        /// The zoom radius.
        /// </summary>
        private const int ZOOM_RADIUS = 500;

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
        /// The temporary path paint.
        /// </summary>
        private Paint tempPathPaint;

        /// <summary>
        /// The temporary target paint.
        /// </summary>
        private Paint tempTargetPaint;

        /// <summary>
        /// The temporary bitmap.
        /// </summary>
        private Bitmap tempBitmap;

        /// <summary>
        /// The current x image.
        /// </summary>
        private int currentX;

        /// <summary>
        /// The current y image.
        /// </summary>
        private int currentY;

        /// <summary>
        /// The current scale indicator for the image.
        /// </summary>
        private double scaleIndicator;

        /// <summary>
        /// The image button dictionnary.
        /// </summary>
        private Dictionary<string, ImageButton> buttonDictionnary;

        /// <summary>
        /// The circles list.
        /// </summary>
        private List<CircleArea> circlesList;

        /// <summary>
        /// The path list.
        /// </summary>
        private List<PathArea> pathList;

        /// <summary>
        /// The superman list.
        /// </summary>
        private List<string[]> supermanList;

        /// <summary>
        /// The current moving circle area.
        /// </summary>
        private CircleArea currentCircle;

        /// <summary>
        /// The buffer circles.
        /// </summary>
        private List<CircleArea> bufferCircles;

        /// <summary>
        /// The twin superman circles.
        /// </summary>
        private List<CircleArea> twinCircles;

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
        private float widthRatio;

        /// <summary>
        /// The picture y ratio.
        /// </summary>
        private float heightRatio;

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
        /// The circle center of the point.
        /// </summary>
        private Point circleCenter;

        /// <summary>
        /// The request picture code.
        /// </summary>
        private const int takePictureCode = 12;

        /// <summary>
        /// The load picture code.
        /// </summary>
        private const int loadPictureCode = 22;

        /// <summary>
        /// The circle radius.
        /// </summary>
        private int rootRadius = 30;

        /// <summary>
        /// The nearest distance.
        /// </summary>
        private const int nearestDistance = 100;

        /// <summary>
        /// The website root width.
        /// </summary>
        private const int webSiteWidth = 292;

        /// <summary>
        /// The website root height.
        /// </summary>
        private const int webSiteHeight = 596;

        /// <summary>
        /// The zoom box check box.
        /// </summary>
        private static CheckBox zoomCheckBox;

        /// <summary>
        /// The constants.
        /// </summary>
        private const int REQUEST_CAMERA_PERMISSION = 2010;
        private const int REQUEST_WRITE_EXTERNAL_STORAGE = 2011;
        private const int REQUEST_READ_EXTERNAL_STORAGE = 2012;
        private const int LEFT_BUTTON_ID = 2000;
        private const int RIGHT_BUTTON_ID = 2001;
        private const int TOP_BUTTON_ID = 2002;
        private const int DOWN_BUTTON_ID = 2003;
        private const int ZOOM_BUTTON_ID = 2004;
        private const int UNZOOM_BUTTON_ID = 2005;
        private const int LEFT_PIVOT_BUTTON_ID = 2006;
        private const int RIGHT_PIVOT_BUTTON_ID = 2007;
        private const int ROTATE_BUTTON_ID = 2008;
        private const int ZOOM_CHECKBOX_ID = 5000;
        private const int ZOOM_CHECKBOX_TEXT_ID = 5001;

        /// <summary>
        /// The shared preferences.
        /// </summary>
        private AndroidContent.ISharedPreferences prefs;

        /// <summary>
        /// The shared preferences editor.
        /// </summary>
        private AndroidContent.ISharedPreferencesEditor editor;

        /// <summary>
        /// The buttons listeners.
        /// </summary>
        private Dictionary<string, SideMoveRepeatListener> listenerDictionnary;
        private SideMoveRepeatListener leftButtonListener;
        private SideMoveRepeatListener rightButtonListener;
        private SideMoveRepeatListener topButtonListener;
        private SideMoveRepeatListener downButtonListener;
        private SideMoveRepeatListener zoomButtonListener;
        private SideMoveRepeatListener unZoomButtonListener;

        /// <summary>
        /// The viewpager parent.
        /// </summary>
        private BodyShapeViewPager viewPager;

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
                fragmentView = inflater.Inflate(Resource.Layout.Picture2, container, false);
                takePictureButton = fragmentView.FindViewById<Button>(Resource.Id.takepicture2Button);
                takePictureButton.Text = Languages.Resources.Res_Android_TakePictureButton;
                loadPictureButton = fragmentView.FindViewById<Button>(Resource.Id.loadpicture2Button);
                loadPictureButton.Text = Languages.Resources.Res_Android_LoadPictureButton;
                takePictureButton.Click += OnTakePicture2Button_Click;
                loadPictureButton.Click += OnLoadPicture2Button_Click;
            }
            
            return fragmentView;
        }

        /// <summary>
        /// The OnDetach event.
        /// </summary>
        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        /// <summary>
        /// The result of the request permissions.
        /// </summary>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == REQUEST_CAMERA_PERMISSION)
            {
                if (grantResults[0] == Permission.Granted)
                {
                    this.CheckWriteAccess();
                }
                else
                {
                    var message = new AlertDialog.Builder(this.Activity);
                    message.SetMessage(Languages.Resources.Res_Android_CameraPermission);
                    message.Show();
                }
            }
            else if (requestCode == REQUEST_WRITE_EXTERNAL_STORAGE)
            {
                if (grantResults[0] == Permission.Granted)
                {
                    this.TakePicture();
                }
                else
                {
                    var message = new AlertDialog.Builder(this.Activity);
                    message.SetMessage(Languages.Resources.Res_Android_StorePictures);
                    message.Show();
                }
            }
            else if (requestCode == REQUEST_READ_EXTERNAL_STORAGE)
            {
                if (grantResults[0] == Permission.Granted)
                {
                    this.LoadPicture();
                }
                else
                {
                    var message = new AlertDialog.Builder(this.Activity);
                    message.SetMessage(Languages.Resources.Res_Android_ReadPictures);
                    message.Show();
                }
            }
        }

        /// <summary>
        /// The disable view method.
        /// </summary>
        public void DisableView()
        {
            this.imageView.Touch -= OnBodyShapeTouchEvent;
            foreach (var button in buttonDictionnary)
            {
                button.Value.SetOnTouchListener(null);
                button.Value.Click -= OnResizeSideImage;
                button.Value.Click -= OnLeftPivotImage;
                button.Value.Click -= OnRightPivotImage;
                button.Value.Click -= OnRotateImage;
            }
        }

        /// <summary>
        /// The result of the pictures.
        /// </summary>
        /// <param name="requestCode">The request code.</param>
        /// <param name="resultCode">The result code.</param>
        /// <param name="data">The picture data.</param>
        public override void OnActivityResult(int requestCode, int resultCode, AndroidContent.Intent data)
        {
            try
            {
                if (resultCode != (int)Result.Canceled)
                {
                    if (resultCode == (int)Result.Ok)
                    {
                        if (resultCode == -1)
                        {
                            // Delete buttons
                            var frameLayout = fragmentView.FindViewById<FrameLayout>(Resource.Id.layoutPicture2Container);
                            frameLayout.RemoveAllViewsInLayout();

                            // New image view
                            viewPager = this.Activity.FindViewById<BodyShapeViewPager>(Resource.Id.bodyshapeViewPager);
                            bool drawOnPicture = false;
                            imageView = new ZoomableImageView(this.Context);
                            imageView.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                            imageView.Visibility = ViewStates.Visible;
                            frameLayout.AddView(imageView);
                            buttonDictionnary = new Dictionary<string, ImageButton>();
                            listenerDictionnary = new Dictionary<string, SideMoveRepeatListener>();

                            // Caching
                            prefs = Application.Context.GetSharedPreferences("bodyshape", AndroidContent.FileCreationMode.Private);
                            editor = prefs.Edit();

                            // For further zooms
                            zoomPoint = new Point();
                            zoomBitmapPoint = new Point();
                            circleCenter = new Point();
                            bufferCircles = new List<CircleArea>();
                            twinCircles = new List<CircleArea>();

                            // The height
                            int height = Resources.DisplayMetrics.HeightPixels;

                            // Result
                            base.OnActivityResult(requestCode, resultCode, data);

                            // The take picture result
                            if (requestCode == takePictureCode)
                            {

                                // Image data
                                AndroidContent.Intent mediaScanIntent = new AndroidContent.Intent(AndroidContent.Intent.ActionMediaScannerScanFile);
                                AndroidNet.Uri contentUri = FileProvider.GetUriForFile(this.Context, this.Context.ApplicationContext.PackageName + ".provider", App2._file);
                                mediaScanIntent.SetData(contentUri);
                                Context.SendBroadcast(mediaScanIntent);

                                // Resize image
                                int width = imageView.Width;
                                App2.bitmap = App2._file.Path.LoadAndResizeBitmap(width, height);
                                if (App2.bitmap != null)
                                {
                                    drawOnPicture = true;
                                }
                            }
                            // The load picture result
                            else if (requestCode == loadPictureCode)
                            {
                                if (data != null && data.Data != null)
                                {
                                    // Get the loaded image
                                    AndroidNet.Uri uri = data.Data;
                                    var fileStream = this.Activity.ContentResolver.OpenInputStream(uri);

                                    // Resize and display
                                    int width = fragmentView.Width;
                                    Bitmap resizedBitmap = MediaStore.Images.Media.GetBitmap(this.Activity.ContentResolver, uri);
                                    var loadedBitmap = fileStream.LoadInGalleryAndResizeBitmap(width, height, resizedBitmap);
                                    if (loadedBitmap != null)
                                    {
                                        drawOnPicture = true;
                                        App2.bitmap = loadedBitmap;

                                        // Persist image
                                        var customDate = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                                        var root = Guid.NewGuid().ToString() + "-" + customDate + "-Android";
                                        var fileNamePersist = root + "Picture_2.png";
                                        var tempRatio = resizedBitmap.Height >= resizedBitmap.Width ? (float)resizedBitmap.Height / resizedBitmap.Width : (float)resizedBitmap.Width / resizedBitmap.Height;
                                        App2._path = this.PersistImage(Bitmap.CreateScaledBitmap(App2.bitmap, (int)(600 / tempRatio), 600, true), fileNamePersist);
                                    }
                                }
                                else
                                {
                                    var message = new AlertDialog.Builder(this.Activity);
                                    message.SetMessage(Languages.Resources.Res_Android_PictureNotFound);
                                    message.Show();
                                }
                            }

                            if (drawOnPicture)
                            {
                                // Update the radius
                                rootRadius = App2.bitmap.Height * 30 / 4032;

                                // Display the bitmap
                                tempBitmap = Bitmap.CreateBitmap(App2.bitmap.Width, App2.bitmap.Height, Bitmap.Config.Rgb565);
                                bitmapRatio = (float)tempBitmap.Height / tempBitmap.Width;
                                tempCanvas = new Canvas(tempBitmap);
                                tempCanvas.DrawBitmap(App2.bitmap, 0, 0, null);
                                imageView.SetImageDrawable(new BitmapDrawable(this.Resources, tempBitmap));
                                imageView.SetPaintShader(new BitmapShader(App2.bitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp));
                                imageView.Touch += OnBodyShapeTouchEvent;

                                // Draw front skeleton
                                this.DrawSideSkeleton(requestCode == takePictureCode);

                                // Buttons dimensions calculations
                                var buttonWidthHeight = (int)height / 18;
                                float calculatedDrawTop = 0;
                                float calculatedDrawHeight = 0;
                                float calculatedBitmapRatio = (float)tempBitmap.Width / tempBitmap.Height;
                                float calculatedimageViewRatio = (float)fragmentView.Width / fragmentView.Height;
                                calculatedDrawHeight = (calculatedimageViewRatio / calculatedBitmapRatio) * fragmentView.Height;
                                calculatedDrawTop = (fragmentView.Height - calculatedDrawHeight) / 2;

                                // Pictures buttons
                                ImageButton leftButton = new ImageButton(this.Context);
                                var leftParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                                leftButton.LayoutParameters = leftParams;
                                leftButton.SetScaleType(ImageView.ScaleType.Center);
                                leftButton.SetAdjustViewBounds(true);
                                leftButton.SetBackgroundResource(Resource.Drawable.previous_button);
                                leftButton.SetY(calculatedDrawTop + buttonWidthHeight);
                                leftButton.SetX(0);
                                leftButton.Id = LEFT_BUTTON_ID;
                                leftButton.Click += OnResizeSideImage;
                                leftButtonListener = new SideMoveRepeatListener(leftButton, fragmentView, tempCanvas, tempPaint, tempPathPaint, rootRadius, circlesList, pathList, 100, 2000, (button) => { }, button => { }, (button, isLongPress) => { });
                                listenerDictionnary.Add("leftListen", leftButtonListener);
                                leftButton.SetOnTouchListener(leftButtonListener);

                                ImageButton rightButton = new ImageButton(this.Context);
                                var rightParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                                rightButton.LayoutParameters = rightParams;
                                rightButton.SetScaleType(ImageView.ScaleType.Center);
                                rightButton.SetAdjustViewBounds(true);
                                rightButton.SetBackgroundResource(Resource.Drawable.next_button);
                                rightButton.SetY(calculatedDrawTop + 2 * buttonWidthHeight);
                                rightButton.SetX(0);
                                rightButton.Id = RIGHT_BUTTON_ID;
                                rightButton.Click += OnResizeSideImage;
                                rightButtonListener = new SideMoveRepeatListener(rightButton, fragmentView, tempCanvas, tempPaint, tempPathPaint, rootRadius, circlesList, pathList, 100, 2000, (button) => { }, button => { }, (button, isLongPress) => { });
                                listenerDictionnary.Add("rightListen", rightButtonListener);
                                rightButton.SetOnTouchListener(rightButtonListener);

                                ImageButton topButton = new ImageButton(this.Context);
                                var topParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                                topButton.LayoutParameters = topParams;
                                topButton.SetScaleType(ImageView.ScaleType.Center);
                                topButton.SetAdjustViewBounds(true);
                                topButton.SetBackgroundResource(Resource.Drawable.top_button);
                                topButton.SetY(calculatedDrawTop + 3 * buttonWidthHeight);
                                topButton.SetX(0);
                                topButton.Id = TOP_BUTTON_ID;
                                topButton.Click += OnResizeSideImage;
                                topButtonListener = new SideMoveRepeatListener(topButton, fragmentView, tempCanvas, tempPaint, tempPathPaint, rootRadius, circlesList, pathList, 100, 2000, (button) => { }, button => { }, (button, isLongPress) => { });
                                listenerDictionnary.Add("topListen", topButtonListener);
                                topButton.SetOnTouchListener(topButtonListener);

                                ImageButton downButton = new ImageButton(this.Context);
                                var downParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                                downButton.LayoutParameters = downParams;
                                downButton.SetScaleType(ImageView.ScaleType.Center);
                                downButton.SetAdjustViewBounds(true);
                                downButton.SetBackgroundResource(Resource.Drawable.down_button);
                                downButton.SetY(calculatedDrawTop + 4 * buttonWidthHeight);
                                downButton.SetX(0);
                                downButton.Id = DOWN_BUTTON_ID;
                                downButton.Click += OnResizeSideImage;
                                downButtonListener = new SideMoveRepeatListener(downButton, fragmentView, tempCanvas, tempPaint, tempPathPaint, rootRadius, circlesList, pathList, 100, 2000, (button) => { }, button => { }, (button, isLongPress) => { });
                                listenerDictionnary.Add("downListen", downButtonListener);
                                downButton.SetOnTouchListener(downButtonListener);

                                ImageButton zoomButton = new ImageButton(this.Context);
                                var zoomParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                                zoomButton.LayoutParameters = zoomParams;
                                zoomButton.SetScaleType(ImageView.ScaleType.Center);
                                zoomButton.SetAdjustViewBounds(true);
                                zoomButton.SetBackgroundResource(Resource.Drawable.zoomin);
                                zoomButton.SetY(calculatedDrawTop + 5 * buttonWidthHeight);
                                zoomButton.SetX(0);
                                zoomButton.Id = ZOOM_BUTTON_ID;
                                zoomButton.Click += OnResizeSideImage;
                                zoomButtonListener = new SideMoveRepeatListener(zoomButton, fragmentView, tempCanvas, tempPaint, tempPathPaint, rootRadius, circlesList, pathList, 100, 2000, (button) => { }, button => { }, (button, isLongPress) => { });
                                listenerDictionnary.Add("zoomListen", zoomButtonListener);
                                zoomButton.SetOnTouchListener(zoomButtonListener);

                                ImageButton unZoomButton = new ImageButton(this.Context);
                                var unzoomParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                                unZoomButton.LayoutParameters = unzoomParams;
                                unZoomButton.SetScaleType(ImageView.ScaleType.Center);
                                unZoomButton.SetAdjustViewBounds(true);
                                unZoomButton.SetBackgroundResource(Resource.Drawable.zoomout);
                                unZoomButton.SetY(calculatedDrawTop + 6 * buttonWidthHeight);
                                unZoomButton.SetX(0);
                                unZoomButton.Id = UNZOOM_BUTTON_ID;
                                unZoomButton.Click += OnResizeSideImage;
                                unZoomButtonListener = new SideMoveRepeatListener(unZoomButton, fragmentView, tempCanvas, tempPaint, tempPathPaint, rootRadius, circlesList, pathList, 100, 2000, (button) => { }, button => { }, (button, isLongPress) => { });
                                listenerDictionnary.Add("unZoomListen", unZoomButtonListener);
                                unZoomButton.SetOnTouchListener(unZoomButtonListener);

                                ImageButton leftPivotButton = new ImageButton(this.Context);
                                var leftPivotParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                                leftPivotButton.LayoutParameters = leftPivotParams;
                                leftPivotButton.SetScaleType(ImageView.ScaleType.Center);
                                leftPivotButton.SetAdjustViewBounds(true);
                                leftPivotButton.SetBackgroundResource(Resource.Drawable.pivotright);
                                leftPivotButton.SetY(calculatedDrawTop + 7 * buttonWidthHeight);
                                leftPivotButton.SetX(0);
                                leftPivotButton.Id = LEFT_PIVOT_BUTTON_ID;
                                leftPivotButton.Click += OnLeftPivotImage;

                                ImageButton rightPivotButton = new ImageButton(this.Context);
                                var rightPivotParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                                rightPivotButton.LayoutParameters = rightPivotParams;
                                rightPivotButton.SetScaleType(ImageView.ScaleType.Center);
                                rightPivotButton.SetAdjustViewBounds(true);
                                rightPivotButton.SetBackgroundResource(Resource.Drawable.pivotleft);
                                rightPivotButton.SetY(calculatedDrawTop + 8 * buttonWidthHeight);
                                rightPivotButton.SetX(0);
                                rightPivotButton.Id = RIGHT_PIVOT_BUTTON_ID;
                                rightPivotButton.Click += OnRightPivotImage;

                                ImageButton rotateButton = new ImageButton(this.Context);
                                var rotateParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                                rotateButton.LayoutParameters = rotateParams;
                                rotateButton.SetScaleType(ImageView.ScaleType.Center);
                                rotateButton.SetAdjustViewBounds(true);
                                rotateButton.SetBackgroundResource(Resource.Drawable.rotate);
                                rotateButton.SetY(calculatedDrawTop + 9 * buttonWidthHeight);
                                rotateButton.SetX(0);
                                rotateButton.Id = ROTATE_BUTTON_ID;
                                rotateButton.Click += OnRotateImage;

                                ImageButton zoomMagnifierIcon = new ImageButton(this.Context);
                                var zoomTextViewParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                                zoomMagnifierIcon.LayoutParameters = zoomTextViewParams;
                                zoomMagnifierIcon.SetScaleType(ImageView.ScaleType.Center);
                                zoomMagnifierIcon.SetAdjustViewBounds(true);
                                zoomMagnifierIcon.SetBackgroundResource(Resource.Drawable.magnifier);
                                zoomMagnifierIcon.SetY(calculatedDrawTop + 10 * buttonWidthHeight);
                                zoomMagnifierIcon.SetX(0);
                                zoomMagnifierIcon.Id = ZOOM_CHECKBOX_TEXT_ID;

                                zoomCheckBox = new CheckBox(this.Context);
                                var zoomCheckBoxParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                                zoomCheckBox.LayoutParameters = zoomCheckBoxParams;
                                zoomCheckBox.SetY(calculatedDrawTop + 10 * buttonWidthHeight);
                                zoomCheckBox.SetX(buttonWidthHeight);
                                zoomCheckBox.Id = ZOOM_CHECKBOX_ID;

                                buttonDictionnary.Add("left", leftButton);
                                buttonDictionnary.Add("right", rightButton);
                                buttonDictionnary.Add("top", topButton);
                                buttonDictionnary.Add("down", downButton);
                                buttonDictionnary.Add("zoom", zoomButton);
                                buttonDictionnary.Add("unzoom", unZoomButton);
                                buttonDictionnary.Add("leftpivot", leftPivotButton);
                                buttonDictionnary.Add("rightpivot", rightPivotButton);
                                buttonDictionnary.Add("rotate", rotateButton);

                                frameLayout.AddView(buttonDictionnary["left"]);
                                frameLayout.AddView(buttonDictionnary["right"]);
                                frameLayout.AddView(buttonDictionnary["top"]);
                                frameLayout.AddView(buttonDictionnary["down"]);
                                frameLayout.AddView(buttonDictionnary["zoom"]);
                                frameLayout.AddView(buttonDictionnary["unzoom"]);
                                frameLayout.AddView(buttonDictionnary["leftpivot"]);
                                frameLayout.AddView(buttonDictionnary["rightpivot"]);
                                frameLayout.AddView(buttonDictionnary["rotate"]);
                                frameLayout.AddView(zoomMagnifierIcon);
                                frameLayout.AddView(zoomCheckBox);

                                // First size coordinates
                                currentX = 0;
                                currentY = 0;
                                scaleIndicator = 0;

                                // Memory
                                GC.Collect();

                                // Picture has been loaded
                                var customDate = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                                var root = Guid.NewGuid().ToString() + "-" + customDate + "-Android";

                                var isLoaded = this.prefs.GetBoolean("picture2", false);
                                if (isLoaded)
                                {
                                    this.editor.Remove("picture2");
                                }
                                this.editor.PutBoolean("picture2", true);
                                var fileName = this.prefs.GetString("filename", null);
                                if (fileName != null)
                                {
                                    this.editor.Remove("filename");
                                }
                                this.editor.PutString("filename", root);
                                this.editor.Apply();

                                // Store prior positions
                                this.CachePositions("sidepositions");
                                this.CachePictureDimensionsAndPositions("sidepicture");
                            }
                        }
                        else
                        {
                            var message = new AlertDialog.Builder(this.Activity);
                            message.SetMessage(Languages.Resources.Res_Android_ErrorTakePictures);
                            message.Show();
                        }
                    }
                }
            }
            catch (Exception)
            {
                var message = new AlertDialog.Builder(this.Activity);
                message.SetMessage(Languages.Resources.Res_Android_ErrorTakePictures);
                message.SetCancelable(false);
                message.SetPositiveButton("OK", (okSender, okE) =>
                {
                    this.ReloadBodyShape();
                });
                message.Show();
            }
        }

        /// <summary>
        /// The persist image mthod.
        /// </summary>
        private string PersistImage(Bitmap bitmap, string name)
        {
            var fileName = System.IO.Path.Combine(App1._dir.Path, name);
            var os = new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate);
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, os);
            os.Flush();
            os.Close();
            return fileName;
        }


        /// <summary>
        /// The take button event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnTakePicture2Button_Click(object sender, EventArgs e)
        {
            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                if (ContextCompat.CheckSelfPermission(this.Context, Manifest.Permission.Camera) != Permission.Granted)
                {
                    this.RequestPermissions(new string[] { Manifest.Permission.Camera }, REQUEST_CAMERA_PERMISSION);
                }
                else
                {
                    this.CheckWriteAccess();
                }
            }
            else
            {
                var message = new AlertDialog.Builder(this.Activity);
                message.SetMessage(Languages.Resources.Res_Android_NoPictureApp);
                message.Show();
            }
        }

        /// <summary>
        /// The load button event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnLoadPicture2Button_Click(object sender, EventArgs e)
        {
            CreateDirectoryForPictures();

            this.CheckReadAccess();
        }

        /// <summary>
        /// The check write access method.
        /// </summary>
        private void CheckWriteAccess()
        {
            if (ContextCompat.CheckSelfPermission(this.Context, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
            {
                this.RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage }, REQUEST_WRITE_EXTERNAL_STORAGE);
            }
            else
            {
                this.TakePicture();
            }
        }

        /// <summary>
        /// The check read access method.
        /// </summary>
        private void CheckReadAccess()
        {
            if (ContextCompat.CheckSelfPermission(this.Context, Manifest.Permission.ReadExternalStorage) != Permission.Granted)
            {
                this.RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage }, REQUEST_READ_EXTERNAL_STORAGE);
            }
            else
            {
                this.LoadPicture();
            }
        }

        /// <summary>
        /// The load picture method.
        /// </summary>
        private void LoadPicture()
        {
            AndroidContent.Intent intent = new AndroidContent.Intent();
            intent.SetType("image/*");
            intent.SetAction(AndroidContent.Intent.ActionGetContent);
            StartActivityForResult(AndroidContent.Intent.CreateChooser(intent, Languages.Resources.Res_Android_SelectPictureTitle), loadPictureCode);
        }

        /// <summary>
        /// The take picture method.
        /// </summary>
        private void TakePicture()
        {
            var customDate = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
            var root = Guid.NewGuid().ToString() + "-" + customDate + "-Android";
            var fileName = root + "Picture_2";

            AndroidContent.Intent intent = new AndroidContent.Intent(MediaStore.ActionImageCapture);
            App2._file = File.CreateTempFile(fileName, ".png", App2._dir);
            intent.PutExtra(MediaStore.ExtraOutput, FileProvider.GetUriForFile(this.Context, this.Context.ApplicationContext.PackageName + ".provider", App2._file));

            StartActivityForResult(intent, takePictureCode);
        }



        /// <summary>
        /// The cache positions method.
        /// </summary>
        private void CachePositions(string key)
        {
            var jsonPositions = JsonConvert.SerializeObject(this.circlesList);
            var positions = this.prefs.GetString(key, null);
            if (positions != null)
            {
                this.editor.Remove(key);
            }
            this.editor.PutString(key, jsonPositions);
            this.editor.Apply();
        }

        /// <summary>
        /// The cache picture dimensions and positions.
        /// </summary>
        private void CachePictureDimensionsAndPositions(string picture)
        {
            var leftKey = picture + "_left";
            var topKey = picture + "_top";
            var widthKey = picture + "_width";
            var heightKey = picture + "_height";

            this.UpdatePictureKey(leftKey, currentX);
            this.UpdatePictureKey(topKey, currentY);
            this.UpdatePictureKey(widthKey, App2.bitmap.Width);
            this.UpdatePictureKey(heightKey, App2.bitmap.Height);

            this.editor.Apply();
        }

        /// <summary>
        /// The update key method.
        /// </summary>
        private void UpdatePictureKey(string key, int value)
        {
            var foundElement = this.prefs.GetInt(key, 0);
            if (foundElement != 0)
            {
                this.editor.Remove(key);
            }
            this.editor.PutInt(key, value);
        }

        /// <summary>
        /// The synchronize method between listeners and the view.
        /// </summary>
        private void SynchronizePositions()
        {
            var currentKey = listenerDictionnary.OrderByDescending(b => b.Value.MoveimageRunnable.LastUpdated).FirstOrDefault().Key;
            this.currentX = listenerDictionnary[currentKey].MoveimageRunnable.CurrentX;
            this.currentY = listenerDictionnary[currentKey].MoveimageRunnable.CurrentY;
            this.scaleIndicator = listenerDictionnary[currentKey].MoveimageRunnable.ScaleIndicator;
            this.circlesList = listenerDictionnary[currentKey].MoveimageRunnable.CirclesList;
            this.pathList = listenerDictionnary[currentKey].MoveimageRunnable.PathList;

            // Listeners update
            foreach (var listen in listenerDictionnary)
            {
                if (listen.Key != currentKey)
                {
                    listen.Value.MoveimageRunnable.Update(this.currentX, this.currentY, this.scaleIndicator, this.circlesList, this.pathList);
                }
            }

            // Caching
            this.CachePictureDimensionsAndPositions("sidepicture");
        }

        /// <summary>
        /// The get nearest circles method.
        /// </summary>
        private List<CircleArea> GetNearestCircles(int positionX, int positionY, int radius)
        {
            var nearest = this.circlesList.Where(f => f.PositionX >= positionX - radius && f.PositionX <= positionX + radius
                                                                    && f.PositionY >= positionY - radius && f.PositionY <= positionY + radius).ToList();

            return nearest;
        }

        /// <summary>
        /// The click event on sizing image buttons.
        /// </summary>
        private void OnResizeSideImage(object sender, EventArgs e)
        {
            this.SynchronizePositions();
        }

        /// <summary>
        /// The left rotate image click.
        /// </summary>
        private void OnLeftPivotImage(object sender, EventArgs e)
        {
            tempCanvas.DrawColor(Color.Black, PorterDuff.Mode.Clear);

            Matrix leftPivotMatrix = new Matrix();
            leftPivotMatrix.PostRotate(-90);
            App2.bitmap = Bitmap.CreateBitmap(App2.bitmap, 0, 0, App2.bitmap.Width, App2.bitmap.Height, leftPivotMatrix, false);
            tempCanvas.DrawBitmap(App2.bitmap, this.currentX, this.currentY, tempPaint);

            foreach (PathArea path in pathList)
            {
                this.DrawBodyShapePath(path.Id, path.Points, false);
            }
            foreach (CircleArea circle in circlesList)
            {
                tempPaint.Color = circle.Color;
                tempCanvas.DrawCircle(circle.PositionX, circle.PositionY, rootRadius, tempPaint);
            }
            fragmentView.Invalidate();
        }

        /// <summary>
        /// The right rotate image click.
        /// </summary>
        private void OnRightPivotImage(object sender, EventArgs e)
        {
            tempCanvas.DrawColor(Color.Black, PorterDuff.Mode.Clear);

            Matrix leftPivotMatrix = new Matrix();
            leftPivotMatrix.PostRotate(90);
            App2.bitmap = Bitmap.CreateBitmap(App2.bitmap, 0, 0, App2.bitmap.Width, App2.bitmap.Height, leftPivotMatrix, false);
            tempCanvas.DrawBitmap(App2.bitmap, this.currentX, this.currentY, tempPaint);

            foreach (PathArea path in pathList)
            {
                this.DrawBodyShapePath(path.Id, path.Points, false);
            }
            foreach (CircleArea circle in circlesList)
            {
                tempPaint.Color = circle.Color;
                tempCanvas.DrawCircle(circle.PositionX, circle.PositionY, rootRadius, tempPaint);
            }
            fragmentView.Invalidate();
        }

        /// <summary>
        /// The rotate image click.
        /// </summary>
        private void OnRotateImage(object sender, EventArgs e)
        {
            tempCanvas.DrawColor(Color.Black, PorterDuff.Mode.Clear);
            Matrix flipHorizontalMatrix = new Matrix();
            flipHorizontalMatrix.SetScale(-1, 1);
            flipHorizontalMatrix.PostTranslate(App2.bitmap.Width, 0);
            App2.bitmap = Bitmap.CreateBitmap(App2.bitmap, 0, 0, App2.bitmap.Width, App2.bitmap.Height, flipHorizontalMatrix, false);
            tempCanvas.DrawBitmap(App2.bitmap, this.currentX, this.currentY, tempPaint);
            foreach(PathArea path in pathList)
            {
                this.DrawBodyShapePath(path.Id, path.Points, false);
            }
            foreach (CircleArea circle in circlesList)
            {
                tempPaint.Color = circle.Color;
                tempCanvas.DrawCircle(circle.PositionX, circle.PositionY, rootRadius, tempPaint);
            }
            fragmentView.Invalidate();
        }

        /// <summary>
        /// The draw skeleton method.
        /// </summary>
        private void DrawSideSkeleton(bool isTakingPicture)
        {
            // Extract root example positions
            var jsonStream = new System.IO.StreamReader(this.Activity.Assets.Open("positions.json"));
            var positionsDataString = jsonStream.ReadToEnd();
            var positionData = JsonConvert.DeserializeObject<Dictionary<string, string>>(positionsDataString);

            // Extract path
            var pathJsonStream = new System.IO.StreamReader(this.Activity.Assets.Open("pathjson.json"));
            var pathJsonStreamString = pathJsonStream.ReadToEnd();
            var pathData = JsonConvert.DeserializeObject<Dictionary<string, string>>(pathJsonStreamString);

            // Extract superman (points at same positions)
            var supermanJsonStream = new System.IO.StreamReader(this.Activity.Assets.Open("supermanjson.json"));
            var supermanJsonStreamString = supermanJsonStream.ReadToEnd();
            var supermanData = JsonConvert.DeserializeObject<Dictionary<string, string>>(supermanJsonStreamString);

            // Initialization
            double myPhoneRatio = (double) 5 / 600;
            double myPhonePathRatio = (double) 20 / 600;
            double myPhoneTargetRatio = (double) 10 / 600;
            double myPhoneTakeRatio = (double)5 / 4032;
            double myPhonePathTakeRatio = (double)20 / 4032;
            double myPhoneTargetTakeRatio = (double)10 / 4032;
            circlesList = new List<CircleArea>();
            pathList = new List<PathArea>();
            supermanList = new List<string[]>();
            tempPaint = new Paint(PaintFlags.AntiAlias)
            {
                StrokeWidth = (float)((isTakingPicture ? myPhoneTakeRatio : myPhoneRatio) * imageView.Drawable.IntrinsicHeight)
            };
            tempPathPaint = new Paint(PaintFlags.AntiAlias)
            {
                StrokeWidth = (float)((isTakingPicture ? myPhonePathTakeRatio : myPhonePathRatio) * imageView.Drawable.IntrinsicHeight)
            };
            tempPathPaint.SetStyle(Paint.Style.Stroke);
            tempTargetPaint = new Paint(PaintFlags.AntiAlias)
            {
                StrokeWidth = (float)((isTakingPicture ? myPhoneTargetTakeRatio : myPhoneTargetRatio) * imageView.Drawable.IntrinsicHeight),
                Color = Color.White
            };
            tempTargetPaint.SetStyle(Paint.Style.Stroke);

            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            var heightWebSiteRatio = (double)imageView.Drawable.IntrinsicHeight / webSiteHeight;
            var centerBitmap = (tempBitmap.Width - rootRadius) / 2;
            var headInfo = positionData.Where(x => x.Key == "head_v1_1").FirstOrDefault().Value.Split(';');
            var headHeight = (float)(float.Parse(headInfo[1], NumberStyles.Any, ci) * heightWebSiteRatio);
            var bitmapRatioHead = (float.Parse(headInfo[0], NumberStyles.Any, ci)) / (float.Parse(headInfo[1], NumberStyles.Any, ci));
            var diffxPoints = centerBitmap - headHeight * bitmapRatioHead;

            // Draw points
            foreach (var pointToDraw in positionData)
            {
                var splitPointData = pointToDraw.Value.Split(';');
                if (splitPointData[3] == "side")
                {
                    var bitmapRatioTotal = (float.Parse(splitPointData[0], NumberStyles.Any, ci)) / (float.Parse(splitPointData[1], NumberStyles.Any, ci));
                    var pointHeight = (float)(float.Parse(splitPointData[1], NumberStyles.Any, ci) * heightWebSiteRatio);
                    circlesList.Add(this.DrawCircleArea(Color.ParseColor(splitPointData[2]),
                        pointHeight * bitmapRatioTotal + diffxPoints,
                        pointHeight,
                        pointToDraw.Key));
                }
            }


            // Draw paths
            foreach (var pathToDraw in pathData)
            {
                var pathKey = pathToDraw.Key;
                var pathValues = pathToDraw.Value.Split(';');
                if (pathValues.All(r => r.Contains("_v")))
                {
                    this.DrawBodyShapePath(pathKey, pathValues, true);
                }
            }

            // Store superman
            foreach (var superman in supermanData)
            {
                var supermanValues = superman.Value.Split(';');
                if (supermanValues.All(r => r.Contains("_v")))
                {
                    supermanList.Add(new string[] { supermanValues[0], supermanValues[1] });
                }
            }

            // Redraw to put paths under circles
            this.ReDrawAll(currentX, currentY, scaleIndicator, false, false, null);
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
        /// The draw target method.
        /// </summary>
        private void DrawTarget(float x, float y)
        {
            Path targetPath = new Path();
            targetPath.SetFillType(Path.FillType.Winding);
            targetPath.MoveTo(x, 0);
            targetPath.LineTo(x, tempCanvas.Height);
            targetPath.MoveTo(0, y);
            targetPath.LineTo(tempCanvas.Width, y);
            tempCanvas.DrawPath(targetPath, tempTargetPaint);
        }

        /// <summary>
        /// The draw path area method for 2 points.
        /// </summary>
        private PathArea Draw2PathArea(string id, Color color, CircleArea[] points)
        {
            var smallRadius = rootRadius / 2;
            tempPathPaint.Color = color;
            Path path = new Path();
            path.SetFillType(Path.FillType.Winding);
            path.SetLastPoint(points[0].PositionX + smallRadius * RightSide(points[0].PositionX), points[0].PositionY);
            path.MoveTo(points[0].PositionX + smallRadius * RightSide(points[0].PositionX), points[0].PositionY);
            path.LineTo(points[1].PositionX + smallRadius * RightSide(points[1].PositionX), points[1].PositionY);
            tempCanvas.DrawPath(path, tempPathPaint);
            return new PathArea(id, color, points.Select(p => p.Id).ToArray());
        }

        /// <summary>
        /// The draw path area method for 3 points.
        /// </summary>
        private PathArea Draw3PathArea(string id, Color color, CircleArea[] points)
        {
            var smallRadius = rootRadius / 2;
            tempPathPaint.Color = color;
            Path path = new Path();
            path.SetFillType(Path.FillType.Winding);
            path.SetLastPoint(points[0].PositionX + smallRadius * RightSide(points[0].PositionX), points[0].PositionY);
            path.MoveTo(points[0].PositionX + smallRadius * RightSide(points[0].PositionX), points[0].PositionY);
            path.LineTo(points[1].PositionX + smallRadius * RightSide(points[1].PositionX), points[1].PositionY);
            path.QuadTo(points[1].PositionX + smallRadius * RightSide(points[1].PositionX), points[1].PositionY, points[2].PositionX + smallRadius * RightSide(points[2].PositionX), points[2].PositionY);
            tempCanvas.DrawPath(path, tempPathPaint);
            return new PathArea(id, color, points.Select(p => p.Id).ToArray());
        }

        /// <summary>
        /// The draw path area method for 4 points.
        /// </summary>
        private PathArea Draw4PathArea(string id, Color color, CircleArea[] points)
        {
            var smallRadius = rootRadius / 2;
            tempPathPaint.Color = color;
            Path path = new Path();
            path.SetFillType(Path.FillType.Winding);
            path.SetLastPoint(points[0].PositionX + smallRadius * RightSide(points[0].PositionX), points[0].PositionY);
            path.MoveTo(points[0].PositionX + smallRadius * RightSide(points[0].PositionX), points[0].PositionY);
            path.LineTo(points[1].PositionX + smallRadius * RightSide(points[1].PositionX), points[1].PositionY);
            path.QuadTo(points[1].PositionX + smallRadius * RightSide(points[1].PositionX), points[1].PositionY, points[2].PositionX + smallRadius * RightSide(points[2].PositionX), points[2].PositionY);
            path.MoveTo(points[2].PositionX + smallRadius * RightSide(points[2].PositionX), points[2].PositionY);
            path.LineTo(points[3].PositionX + smallRadius * RightSide(points[3].PositionX), points[3].PositionY);
            tempCanvas.DrawPath(path, tempPathPaint);
            return new PathArea(id, color, points.Select(p => p.Id).ToArray());
        }

        /// <summary>
        /// The draw path area method for 5 points.
        /// </summary>
        private PathArea Draw5PathArea(string id, Color color, CircleArea[] points)
        {
            var smallRadius = rootRadius / 2;
            tempPathPaint.Color = color;
            Path path = new Path();
            path.SetFillType(Path.FillType.Winding);
            path.SetLastPoint(points[0].PositionX + smallRadius * RightSide(points[0].PositionX), points[0].PositionY);
            path.MoveTo(points[0].PositionX + smallRadius * RightSide(points[0].PositionX), points[0].PositionY);
            path.LineTo(points[1].PositionX + smallRadius * RightSide(points[1].PositionX), points[1].PositionY);
            path.QuadTo(points[1].PositionX + smallRadius * RightSide(points[1].PositionX), points[1].PositionY, points[2].PositionX + smallRadius * RightSide(points[2].PositionX), points[2].PositionY);
            path.MoveTo(points[2].PositionX + smallRadius * RightSide(points[2].PositionX), points[2].PositionY);
            path.LineTo(points[3].PositionX + smallRadius * RightSide(points[3].PositionX), points[3].PositionY);
            path.MoveTo(points[3].PositionX + smallRadius * RightSide(points[3].PositionX), points[3].PositionY);
            path.LineTo(points[4].PositionX + smallRadius * RightSide(points[4].PositionX), points[4].PositionY);
            tempCanvas.DrawPath(path, tempPathPaint);
            return new PathArea(id, color, points.Select(p => p.Id).ToArray());
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
        /// The get twin superman twin circles.
        /// </summary>
        private List<CircleArea> GetTwinCircles(string id)
        {
            var realTwins = new List<CircleArea>();
            var twins = supermanList.Where(i => i[0] == id || i[1] == id);
            foreach (var twin in twins)
            {
                if (twin[0] == id)
                {
                    realTwins.Add(circlesList.Where(t => t.Id == twin[1]).FirstOrDefault());
                }
                else if (twin[1] == id)
                {
                    realTwins.Add(circlesList.Where(t => t.Id == twin[0]).FirstOrDefault());
                }
            }
            var firstTwinsId = realTwins.Select(b => b.Id);
            var secondaryTwins = supermanList.Where(i => firstTwinsId.Contains(i[0]) || firstTwinsId.Contains(i[1]));
            foreach(var secondTwin in secondaryTwins)
            {
                if(firstTwinsId.Contains(secondTwin[0]))
                {
                    realTwins.Add(circlesList.Where(t => t.Id == secondTwin[1]).FirstOrDefault());
                }
                else if(firstTwinsId.Contains(secondTwin[1]))
                {
                    realTwins.Add(circlesList.Where(t => t.Id == secondTwin[0]).FirstOrDefault());
                }
            }
            return realTwins;
        }

        /// <summary>
        /// The ReDrawAll method at every move.
        /// </summary>
        private void ReDrawAll(int x, int y, double scale, bool scaling, bool moving, float[] endZoomPoints)
        {
            tempCanvas.DrawColor(Color.Black, PorterDuff.Mode.Clear);

            if (scaling)
            {
                var ratio = (double)App2.bitmap.Width / App2.bitmap.Height;
                var newHeight = (int)(App2.bitmap.Height + scale);
                App2.bitmap = BitmapHelpers.ResizeCurrentBitmap(App2.bitmap, newHeight, (int)(newHeight * ratio));
            }

            tempCanvas.DrawBitmap(App2.bitmap, x, y, tempPaint);
            if (moving)
            {
                this.DrawTarget(currentCircle.PositionX, currentCircle.PositionY);
            }

            // Paths
            foreach (PathArea path in pathList)
            {
                this.DrawBodyShapePath(path.Id, path.Points, false);
            }

            // Circles
            foreach (CircleArea circle in circlesList)
            {
                tempPaint.Color = circle.Color;
                tempCanvas.DrawCircle(circle.PositionX, circle.PositionY, rootRadius, tempPaint);
                //if (circle.Id != currentCircle.Id)
                //{
                //    tempCanvas.DrawCircle(circle.PositionX, circle.PositionY, rootRadius, tempPaint);
                //}
                //else
                //{
                //    if (endZoomPoints != null)
                //    {
                //        circle.PositionX = circleCenter.X + endZoomPoints[0];
                //        circle.PositionY = circleCenter.Y + endZoomPoints[1];
                //        tempCanvas.DrawCircle(circle.PositionX, circle.PositionY, rootRadius, tempPaint);
                //    }
                //}
            }
        }

        /// <summary>
        /// The draw bodyshape path method.
        /// </summary>
        private void DrawBodyShapePath(string key, string[] points, bool create)
        {
            if (points.Length == 2)
            {
                var firstPoint = circlesList.Where(b => b.Id == points[0]).FirstOrDefault();
                var secondPoint = circlesList.Where(b => b.Id == points[1]).FirstOrDefault();

                var result = this.Draw2PathArea(key, firstPoint.Color, new CircleArea[] { firstPoint, secondPoint });
                if (create)
                {
                    pathList.Add(result);
                }
            }
            else if (points.Length == 3)
            {
                var firstPoint = circlesList.Where(b => b.Id == points[0]).FirstOrDefault();
                var secondPoint = circlesList.Where(b => b.Id == points[1]).FirstOrDefault();
                var thirdPoint = circlesList.Where(b => b.Id == points[2]).FirstOrDefault();

                var result = this.Draw3PathArea(key, firstPoint.Color, new CircleArea[] { firstPoint, secondPoint, thirdPoint });
                if (create)
                {
                    pathList.Add(result);
                }
            }
            else if (points.Length == 4)
            {
                var firstPoint = circlesList.Where(b => b.Id == points[0]).FirstOrDefault();
                var secondPoint = circlesList.Where(b => b.Id == points[1]).FirstOrDefault();
                var thirdPoint = circlesList.Where(b => b.Id == points[2]).FirstOrDefault();
                var fourthPoint = circlesList.Where(b => b.Id == points[3]).FirstOrDefault();

                var result = this.Draw4PathArea(key, firstPoint.Color, new CircleArea[] { firstPoint, secondPoint, thirdPoint, fourthPoint });
                if (create)
                {
                    pathList.Add(result);
                }
            }
            else if (points.Length == 5)
            {
                var firstPoint = circlesList.Where(b => b.Id == points[0]).FirstOrDefault();
                var secondPoint = circlesList.Where(b => b.Id == points[1]).FirstOrDefault();
                var thirdPoint = circlesList.Where(b => b.Id == points[2]).FirstOrDefault();
                var fourthPoint = circlesList.Where(b => b.Id == points[3]).FirstOrDefault();
                var fifthPoint = circlesList.Where(b => b.Id == points[4]).FirstOrDefault();

                var result = this.Draw5PathArea(key, firstPoint.Color, new CircleArea[] { firstPoint, secondPoint, thirdPoint, fourthPoint, fifthPoint });
                if (create)
                {
                    pathList.Add(result);
                }
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

            // Lags
            float calculatedDrawLeft = 0;
            float calculatedDrawTop = 0;
            float calculatedDrawHeight = 0;
            float calculatedDrawWidth = 0;
            float calculatedBitmapRatio = (float)tempCanvas.Width / tempCanvas.Height;
            float calculatedimageViewRatio = (float)viewer.Width / viewer.Height;
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

            // Ratio updates
            if (widthRatio == 0)
            {
                widthRatio = (float)viewer.Width / tempCanvas.Width;
            }
            if (heightRatio == 0)
            {
                heightRatio = (float)viewer.Height / tempCanvas.Height;
            }

            // Zoom coordinates
            var eventX = e.Event.GetX();
            var eventY = e.Event.GetY();
            zoomPoint.X = (int)eventX;
            zoomPoint.Y = (int)eventY;
            var halfHeight = Math.Round((double)viewer.Height / 2);
            var sideCondition = eventY >= halfHeight;
            var diffRatioHeight = Math.Abs(eventY - halfHeight) / (viewer.Height / 2);
            var heightSideBool = sideCondition ? 1 : -1;
            var x = (int)Math.Round((double)eventX / widthRatio);
            var y = (int)Math.Round((eventY + calculatedDrawTop * heightSideBool * diffRatioHeight) / heightRatio);
            zoomBitmapPoint.X = x;
            zoomBitmapPoint.Y = y;
            
            // Limits
            if (x < 0)
            {
                x = 0;
            }
            if (x > tempCanvas.Width)
            {
                x = tempCanvas.Width;
            }
            if (y < 0)
            {
                y = 0;
            }
            if (y > tempCanvas.Height)
            {
                y = tempCanvas.Height;
            }

            // Actions
            if (e.Event.Action == MotionEventActions.Down)
            {
                // Get nearest circle
                currentCircle = this.GetNearestCircle(x, y);

                if(currentCircle != null)
                {
                    // Vibration
                    Vibrator vibrator = (Vibrator)Activity.GetSystemService(AndroidContent.Context.VibratorService);
                    vibrator.Vibrate(50);

                    // Zoom
                    circleCenter.X = (int) currentCircle.PositionX;
                    circleCenter.Y = (int)currentCircle.PositionY;
                    bufferCircles = this.GetNearestCircles(circleCenter.X, circleCenter.Y, ZOOM_RADIUS);
                    this.twinCircles = this.GetTwinCircles(currentCircle.Id);
                    if (zoomCheckBox.Checked)
                    {
                        imageView.EnableZoom(zoomPoint, circleCenter, bufferCircles, currentCircle, (double)imageView.Width / tempCanvas.Width, (double)imageView.Height / tempCanvas.Height);
                    }
                    viewPager.SetSwipeEnabled(false);
                }                
            }
            else if (e.Event.Action == MotionEventActions.Move)
            {
                if (currentCircle != null)
                {
                    // Redraw
                    if (zoomCheckBox.Checked)
                    {
                        imageView.MovePivotPoint(eventX, eventY);
                    }
                    else
                    {
                        currentCircle.UpdatePosition(x, y);
                        foreach (var twiny in this.twinCircles)
                        {
                            circlesList.Where(b => b.Id == twiny.Id).FirstOrDefault()?.UpdatePosition(x, y);
                        }
                        this.ReDrawAll(currentX, currentY, scaleIndicator, false, true, null);
                    }        
                }
            }
            else if (e.Event.Action == MotionEventActions.Up)
            {
                // UnZoom
                if (currentCircle != null)
                {
                    if (zoomCheckBox.Checked)
                    {
                        if (imageView.Distances == null)
                        {
                            var newPivotPoint = imageView.GetNewPivotPosition();
                            if (newPivotPoint != null)
                            {
                                // Update pivot position.
                                var halfHeightNewPivot = Math.Round((double)viewer.Height / 2);
                                var sideConditionNewPivot = newPivotPoint.Y >= halfHeightNewPivot;
                                var diffRatioHeightNewPivot = Math.Abs(newPivotPoint.Y - halfHeightNewPivot) / (viewer.Height / 2);
                                var heightSideBoolNewPivot = sideConditionNewPivot ? 1 : -1;
                                var newPivotX = (int)Math.Round((double)newPivotPoint.X / widthRatio);
                                var newPivotY = (int)Math.Round((newPivotPoint.Y + calculatedDrawTop * heightSideBoolNewPivot * diffRatioHeightNewPivot) / heightRatio);
                                currentCircle.PositionX = newPivotX;
                                currentCircle.PositionY = newPivotY;
                            }

                            imageView.DisableZoom();
                        }
                    }

                    this.ReDrawAll(currentX, currentY, scaleIndicator, false, false, imageView.Distances);
                    this.CachePositions("sidepositions");
                }

                currentCircle = null;
                bufferCircles = new List<CircleArea>();
                twinCircles = new List<CircleArea>();
                viewPager.SetSwipeEnabled(true);
            }

            fragmentView.Invalidate();
        }

        /// <summary>
        /// Create a directory for the pictures if this is the first time
        /// </summary>
        private void CreateDirectoryForPictures()
        {
            App1._dir = new File(AndroidOS.Environment.GetExternalStoragePublicDirectory(AndroidOS.Environment.DirectoryPictures), "BodyShapePictures");
            App2._dir = new File(AndroidOS.Environment.GetExternalStoragePublicDirectory(AndroidOS.Environment.DirectoryPictures), "BodyShapePictures");
            if (!App2._dir.Exists())
            {
                App2._dir.Mkdirs();
            }
        }

        /// <summary>
        /// Checks whether or not the phone has a camera app
        /// </summary>
        /// <returns></returns>
        private bool IsThereAnAppToTakePictures()
        {
            AndroidContent.Intent intent = new AndroidContent.Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = Context.PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        /// <summary>
        /// The right side position from center canvas.
        /// </summary>
        private int RightSide(float position)
        {
            if (position > tempCanvas.Width / 2)
            {
                return 1;
            }
            else if (position < tempCanvas.Width / 2)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// The reload bodyshape app.
        /// </summary>
        private void ReloadBodyShape()
        {
            AndroidContent.Intent i = this.Activity.PackageManager.GetLaunchIntentForPackage(this.Context.PackageName);
            i.AddFlags(AndroidContent.ActivityFlags.ClearTop);
            this.StartActivity(i);
        }
    }
}