/**********************************************************/
/*************** The picture 2 fragment
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
using MyBodyShape.Android.Listeners;
using Android.App;
using Android.Graphics.Drawables;

namespace MyBodyShape.Android.Fragments
{
    /// <summary>
    /// Static class for image informations.
    /// </summary>
    public static class App2
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
    }

    /// <summary>
    /// The picture 2 fragment.
    /// </summary>
    public class SecondPictureFragment : V4App.Fragment
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
        /// The current moving circle area.
        /// </summary>
        private CircleArea currentCircle;

        /// <summary>
        /// The buffer circles.
        /// </summary>
        private List<CircleArea> bufferCircles;

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
        private const int rootRadius = 50;

        /// <summary>
        /// The nearest distance.
        /// </summary>
        private const int nearestDistance = 100;

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
                loadPictureButton = fragmentView.FindViewById<Button>(Resource.Id.loadpicture2Button);
                takePictureButton.Click += OnTakePicture2Button_Click;
                loadPictureButton.Click += OnLoadPicture2Button_Click;
            }
            
            return fragmentView;
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

                var customDate = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                var root = Guid.NewGuid().ToString() + "-" + customDate + "-Android";
                var fileName = root + "Picture_2";

                Intent intent = new Intent(MediaStore.ActionImageCapture);
                App2._file = new File(App2._dir, $"{ fileName }.png");
                intent.PutExtra(MediaStore.ExtraOutput, AndroidNet.Uri.FromFile(App2._file));
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
        private void OnLoadPicture2Button_Click(object sender, EventArgs e)
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

                // For further zooms
                zoomPoint = new Point();
                zoomBitmapPoint = new Point();
                circleCenter = new Point();
                bufferCircles = new List<CircleArea>();

                // The height
                int height = Resources.DisplayMetrics.HeightPixels;

                // The take picture result
                if (requestCode == takePictureCode)
                {               
                    // Result
                    base.OnActivityResult(requestCode, resultCode, data);

                    // Image data
                    Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                    AndroidNet.Uri contentUri = AndroidNet.Uri.FromFile(App2._file);
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
                    if (data != null)
                    {
                        // Get the loaded image
                        AndroidNet.Uri uri = data.Data;

                        // Resize and display
                        int width = fragmentView.Width;
                        Bitmap resizedBitmap = MediaStore.Images.Media.GetBitmap(this.Activity.ContentResolver, uri);
                        var loadedBitmap = uri.Path.LoadInGalleryAndResizeBitmap(width, height, resizedBitmap);
                        if (loadedBitmap != null)
                        {
                            drawOnPicture = true;
                            App2.bitmap = loadedBitmap;
                        }
                    }
                    else
                    {
                        var message = new AlertDialog.Builder(this.Activity);
                        message.SetMessage("No picture was found.");
                        message.Show();
                    }
                }

                if (drawOnPicture)
                {
                    tempBitmap = Bitmap.CreateBitmap(App2.bitmap.Width, App2.bitmap.Height, Bitmap.Config.Rgb565);
                    bitmapRatio = (float)tempBitmap.Height / tempBitmap.Width;
                    tempCanvas = new Canvas(tempBitmap);
                    tempCanvas.DrawBitmap(App2.bitmap, 0, 0, null);
                    imageView.SetImageDrawable(new BitmapDrawable(this.Resources, tempBitmap));
                    imageView.SetPaintShader(new BitmapShader(App2.bitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp));
                    imageView.Touch += OnBodyShapeTouchEvent;

                    // Original image dimensions
                    Drawable drawable = imageView.Drawable;
                    intrinsicWidth = drawable.IntrinsicWidth;
                    intrinsicHeight = drawable.IntrinsicHeight;

                    // Draw front skeleton
                    this.DrawSideSkeleton();

                    // Buttons dimensions calculations
                    var buttonWidthHeight = (int)height / 25;
                    float calculatedDrawTop = 0;
                    float calculatedDrawHeight = 0;
                    float calculatedBitmapRatio = (float)tempBitmap.Width / tempBitmap.Height;
                    float calculatedimageViewRatio = (float)fragmentView.Width / fragmentView.Height;
                    calculatedDrawHeight = (calculatedimageViewRatio / calculatedBitmapRatio) * fragmentView.Height;
                    calculatedDrawTop = (fragmentView.Height - calculatedDrawHeight) / 2;
                    var downPosition = fragmentView.Height - calculatedDrawTop;

                    // Pictures buttons
                    ImageButton leftButton = new ImageButton(this.Context);
                    var leftParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                    leftButton.LayoutParameters = leftParams;
                    leftButton.SetScaleType(ImageView.ScaleType.Center);
                    leftButton.SetAdjustViewBounds(true);
                    leftButton.SetBackgroundResource(Resource.Drawable.previous_button);
                    leftButton.SetY(downPosition - buttonWidthHeight);
                    leftButton.SetX(0);
                    leftButton.Id = 2000;
                    leftButton.Click += OnResizeSideImage;
                    leftButtonListener = new SideMoveRepeatListener(leftButton, fragmentView, tempCanvas, tempPaint, rootRadius, circlesList, 100, 2000, (button) => { }, button => { }, (button, isLongPress) => { });
                    listenerDictionnary.Add("leftListen", leftButtonListener);
                    leftButton.SetOnTouchListener(leftButtonListener);

                    ImageButton rightButton = new ImageButton(this.Context);
                    var rightParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                    rightButton.LayoutParameters = rightParams;
                    rightButton.SetScaleType(ImageView.ScaleType.Center);
                    rightButton.SetAdjustViewBounds(true);
                    rightButton.SetBackgroundResource(Resource.Drawable.next_button);
                    rightButton.SetY(downPosition - buttonWidthHeight);
                    rightButton.SetX(4 * buttonWidthHeight);
                    rightButton.Id = 2001;
                    rightButton.Click += OnResizeSideImage;
                    rightButtonListener = new SideMoveRepeatListener(rightButton, fragmentView, tempCanvas, tempPaint, rootRadius, circlesList, 100, 2000, (button) => { }, button => { }, (button, isLongPress) => { });
                    listenerDictionnary.Add("rightListen", rightButtonListener);
                    rightButton.SetOnTouchListener(rightButtonListener);

                    ImageButton topButton = new ImageButton(this.Context);
                    var topParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                    topButton.LayoutParameters = topParams;
                    topButton.SetScaleType(ImageView.ScaleType.Center);
                    topButton.SetAdjustViewBounds(true);
                    topButton.SetBackgroundResource(Resource.Drawable.top_button);
                    topButton.SetY(downPosition - 2 * buttonWidthHeight);
                    topButton.SetX(2 * buttonWidthHeight);
                    topButton.Id = 2002;
                    topButton.Click += OnResizeSideImage;
                    topButtonListener = new SideMoveRepeatListener(topButton, fragmentView, tempCanvas, tempPaint, rootRadius, circlesList, 100, 2000, (button) => { }, button => { }, (button, isLongPress) => { });
                    listenerDictionnary.Add("topListen", topButtonListener);
                    topButton.SetOnTouchListener(topButtonListener);

                    ImageButton downButton = new ImageButton(this.Context);
                    var downParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                    downButton.LayoutParameters = downParams;
                    downButton.SetScaleType(ImageView.ScaleType.Center);
                    downButton.SetAdjustViewBounds(true);
                    downButton.SetBackgroundResource(Resource.Drawable.down_button);
                    downButton.SetY(downPosition);
                    downButton.SetX(2 * buttonWidthHeight);
                    downButton.Id = 2003;
                    downButton.Click += OnResizeSideImage;
                    downButtonListener = new SideMoveRepeatListener(downButton, fragmentView, tempCanvas, tempPaint, rootRadius, circlesList, 100, 2000, (button) => { }, button => { }, (button, isLongPress) => { });
                    listenerDictionnary.Add("downListen", downButtonListener);
                    downButton.SetOnTouchListener(downButtonListener);

                    ImageButton zoomButton = new ImageButton(this.Context);
                    var zoomParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                    zoomButton.LayoutParameters = zoomParams;
                    zoomButton.SetScaleType(ImageView.ScaleType.Center);
                    zoomButton.SetAdjustViewBounds(true);
                    zoomButton.SetBackgroundResource(Resource.Drawable.zoomin);
                    zoomButton.SetY(downPosition - buttonWidthHeight);
                    zoomButton.SetX(3 * buttonWidthHeight);
                    zoomButton.Id = 2004;
                    zoomButton.Click += OnResizeSideImage;
                    zoomButtonListener = new SideMoveRepeatListener(zoomButton, fragmentView, tempCanvas, tempPaint, rootRadius, circlesList, 100, 2000, (button) => { }, button => { }, (button, isLongPress) => { });
                    listenerDictionnary.Add("zoomListen", zoomButtonListener);
                    zoomButton.SetOnTouchListener(zoomButtonListener);

                    ImageButton unZoomButton = new ImageButton(this.Context);
                    var unzoomParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                    unZoomButton.LayoutParameters = unzoomParams;
                    unZoomButton.SetScaleType(ImageView.ScaleType.Center);
                    unZoomButton.SetAdjustViewBounds(true);
                    unZoomButton.SetBackgroundResource(Resource.Drawable.zoomout);
                    unZoomButton.SetY(downPosition - buttonWidthHeight);
                    unZoomButton.SetX(buttonWidthHeight);
                    unZoomButton.Id = 2005;
                    unZoomButton.Click += OnResizeSideImage;
                    unZoomButtonListener = new SideMoveRepeatListener(unZoomButton, fragmentView, tempCanvas, tempPaint, rootRadius, circlesList, 100, 2000, (button) => { }, button => { }, (button, isLongPress) => { });
                    listenerDictionnary.Add("unZoomListen", unZoomButtonListener);
                    unZoomButton.SetOnTouchListener(unZoomButtonListener);

                    ImageButton rotateButton = new ImageButton(this.Context);
                    var rotateParams = new FrameLayout.LayoutParams(buttonWidthHeight, buttonWidthHeight);
                    rotateButton.LayoutParameters = rotateParams;
                    rotateButton.SetScaleType(ImageView.ScaleType.Center);
                    rotateButton.SetAdjustViewBounds(true);
                    rotateButton.SetBackgroundResource(Resource.Drawable.rotate);
                    rotateButton.SetY(downPosition - buttonWidthHeight);
                    rotateButton.SetX(2 * buttonWidthHeight);
                    rotateButton.Id = 2006;
                    rotateButton.Click += OnRotateImage;

                    buttonDictionnary.Add("left", leftButton);
                    buttonDictionnary.Add("right", rightButton);
                    buttonDictionnary.Add("top", topButton);
                    buttonDictionnary.Add("down", downButton);
                    buttonDictionnary.Add("zoom", zoomButton);
                    buttonDictionnary.Add("unzoom", unZoomButton);
                    buttonDictionnary.Add("rotate", rotateButton);

                    frameLayout.AddView(buttonDictionnary["left"]);
                    frameLayout.AddView(buttonDictionnary["right"]);
                    frameLayout.AddView(buttonDictionnary["top"]);
                    frameLayout.AddView(buttonDictionnary["down"]);
                    frameLayout.AddView(buttonDictionnary["zoom"]);
                    frameLayout.AddView(buttonDictionnary["unzoom"]);
                    frameLayout.AddView(buttonDictionnary["rotate"]);

                    // First size coordinates
                    currentX = 0;
                    currentY = 0;
                    scaleIndicator = 0;

                    // Memory
                    GC.Collect();
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
        /// The synchronize method between listeners and the view.
        /// </summary>
        private void SynchronizePositions()
        {
            var currentKey = listenerDictionnary.OrderByDescending(b => b.Value.MoveimageRunnable.LastUpdated).FirstOrDefault().Key;
            this.currentX = listenerDictionnary[currentKey].MoveimageRunnable.CurrentX;
            this.currentY = listenerDictionnary[currentKey].MoveimageRunnable.CurrentY;
            this.scaleIndicator = listenerDictionnary[currentKey].MoveimageRunnable.ScaleIndicator;
            this.circlesList = listenerDictionnary[currentKey].MoveimageRunnable.CirclesList;

            // Listeners update
            foreach (var listen in listenerDictionnary)
            {
                if (listen.Key != currentKey)
                {
                    listen.Value.MoveimageRunnable.Update(this.currentX, this.currentY, this.scaleIndicator, this.circlesList);
                }
            }
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
        private void DrawSideSkeleton()
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
            circlesList.Add(this.DrawCircleArea(Color.Red, centerBitmap - 100, 150, "head3"));
            circlesList.Add(this.DrawCircleArea(Color.Red, centerBitmap + 100, 150, "head3"));
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
        private void ReDrawAll(int x, int y, double scale, bool scaling)
        {
            tempCanvas.DrawColor(Color.Black, PorterDuff.Mode.Clear);

            if (scaling)
            {
                var ratio = (double)App2.bitmap.Width / App2.bitmap.Height;
                var newHeight = (int)(App2.bitmap.Height + scale);
                App2.bitmap = BitmapHelpers.ResizeCurrentBitmap(App2.bitmap, newHeight, (int)(newHeight * ratio));
            }

            tempCanvas.DrawBitmap(App2.bitmap, x, y, tempPaint);
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
            float calculatedBitmapRatio = (float)tempBitmap.Width / tempBitmap.Height;
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
            zoomPoint.X = (int)eventX;
            zoomPoint.Y = (int)eventY;
            zoomBitmapPoint.X = (int)x;
            zoomBitmapPoint.Y = (int)y;

            if (e.Event.Action == MotionEventActions.Down)
            {
                // Vibration
                Vibrator vibrator = (Vibrator)Activity.GetSystemService(Context.VibratorService);
                vibrator.Vibrate(50);

                // Get nearest circle
                currentCircle = this.GetNearestCircle(x, y);

                if(currentCircle != null)
                {
                    // Zoom
                    circleCenter.X = (int) currentCircle.PositionX;
                    circleCenter.Y = (int)currentCircle.PositionY;
                    bufferCircles = this.GetNearestCircles(circleCenter.X, circleCenter.Y, 400);
                    imageView.EnableZoom(zoomPoint, circleCenter, bufferCircles);
                    viewPager.SetSwipeEnabled(false);
                }                
            }
            else if (e.Event.Action == MotionEventActions.Move)
            {
                if (currentCircle != null)
                {
                    // Redraw
                    currentCircle.UpdatePosition(x, y);
                    this.ReDrawAll(currentX, currentY, scaleIndicator, false);
                }
            }
            else if (e.Event.Action == MotionEventActions.Up)
            {
                // UnZoom
                imageView.DisableZoom();

                currentCircle = null;
                bufferCircles = new List<CircleArea>();
                viewPager.SetSwipeEnabled(true);
            }

            fragmentView.Invalidate();
        }

        /// <summary>
        /// Create a directory for the pictures if this is the first time
        /// </summary>
        private void CreateDirectoryForPictures()
        {
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
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = Context.PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
    }
}