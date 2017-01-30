/**********************************************************/
/*************** The side move image runnable
/**********************************************************/

using Android.Graphics;
using Android.OS;
using Android.Widget;
using Java.Lang;
using ViewImageView = Android.Views;
using MyBodyShape.Android.Fragments;
using MyBodyShape.Android.Helpers;
using System.Collections.Generic;
using System;

namespace MyBodyShape.Android.Listeners
{
    /// <summary>
    /// The side move image runnable.
    /// </summary>
    public class SideMoveImageRunnable : Java.Lang.Object, IRunnable
    {
        /// <summary>
        /// The image button.
        /// </summary>
        private ImageButton _imageButton;

        /// <summary>
        /// The image button.
        /// </summary>
        private ViewImageView.View _fragmentView;

        /// <summary>
        /// The repeated handler.
        /// </summary>
        private Handler _repeatedHandler;

        /// <summary>
        /// The canvas.
        /// </summary>
        private Canvas _canvas;

        /// <summary>
        /// The paint.
        /// </summary>
        private Paint _paint;

        /// <summary>
        /// The circles' radius.
        /// </summary>
        private float _radius;

        /// <summary>
        /// The last updated date time.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// The current X coordinate.
        /// </summary>
        public int CurrentX { get; set; }

        /// <summary>
        /// The current Y coordinate.
        /// </summary>
        public int CurrentY { get; set; }

        /// <summary>
        /// The scale indicator.
        /// </summary>
        public double ScaleIndicator { get; set; }

        /// <summary>
        /// The circle list.
        /// </summary>
        public List<CircleArea> CirclesList { get; set; }

        /// <summary>
        /// The constructor.
        /// </summary>
        public SideMoveImageRunnable(ViewImageView.View fragmentView, Handler repeatedHandler, ImageButton currentButton, Canvas canvas, Paint paint, float radius, List<CircleArea> circlesList)
        {
            _imageButton = currentButton;
            _repeatedHandler = repeatedHandler;
            _fragmentView = fragmentView;
            _canvas = canvas;
            _paint = paint;
            _radius = radius;
            CirclesList = circlesList;
            LastUpdated = DateTime.Now;
        }
        
        /// <summary>
        /// The Run method.
        /// </summary>
        public void Run()
        {
            var scaling = false;
            switch (_imageButton.Id)
            {
                case 2000:
                    CurrentX -= 20;
                    ScaleIndicator = 0;
                    break;
                case 2001:
                    CurrentX += 20;
                    ScaleIndicator = 0;
                    break;
                case 2002:
                    CurrentY -= 20;
                    ScaleIndicator = 0;
                    break;
                case 2003:
                    CurrentY += 20;
                    ScaleIndicator = 0;
                    break;
                case 2004:
                    ScaleIndicator = 30;
                    scaling = true;
                    break;
                case 2005:
                    ScaleIndicator = -30;
                    scaling = true;
                    break;
                default:
                    break;
            }

            this.ReDrawAll(CurrentX, CurrentY, ScaleIndicator, scaling);
            _fragmentView.Invalidate();

            LastUpdated = DateTime.Now;
            _repeatedHandler.PostDelayed(this, 100);
        }

        /// <summary>
        /// The Update method.
        /// </summary>
        public void Update(int x, int y, double scale, List<CircleArea> list)
        {
            this.CurrentX = x;
            this.CurrentY = y;
            this.ScaleIndicator = scale;
            this.CirclesList = list;
        }

        /// <summary>
        /// The ReDrawAll method at every move.
        /// </summary>
        private void ReDrawAll(int x, int y, double scale, bool scaling)
        {
            _canvas.DrawColor(Color.Black, PorterDuff.Mode.Clear);

            if (scaling)
            {
                var ratio = (double)App2.bitmap.Width / App2.bitmap.Height;
                var newHeight = (int)(App2.bitmap.Height + scale);
                App2.bitmap = BitmapHelpers.ResizeCurrentBitmap(App2.bitmap, newHeight, (int)(newHeight * ratio));
            }

            _canvas.DrawBitmap(App2.bitmap, x, y, _paint);
            foreach (CircleArea circle in CirclesList)
            {
                _paint.Color = circle.Color;
                _canvas.DrawCircle(circle.PositionX, circle.PositionY, _radius, _paint);
            }
        }
    }
}