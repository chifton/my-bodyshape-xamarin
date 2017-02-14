/**********************************************************/
/*************** The zoomable image view
/**********************************************************/

using Android.Content;
using Android.Graphics;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyBodyShape.Android.Helpers
{
    public class ZoomableImageView : ImageView
    {
        /// <summary>
        /// The zoomable image view boolean.
        /// </summary>
        private bool zooming;

        /// <summary>
        /// The zoomable image view boolean.
        /// </summary>
        private Point currentPointZooming;

        /// <summary>
        /// The zoomable image view boolean.
        /// </summary>
        private Point currentPointBitmapZooming;

        /// <summary>
        /// The list of buffered points.
        /// </summary>
        private List<CircleArea> _bufferPoints;

        /// <summary>
        /// The current circle.
        /// </summary>
        private CircleArea _currentCircle;

        /// <summary>
        /// The canvas matrix.
        /// </summary>
        private Matrix matrix;

        /// <summary>
        /// The paint.
        /// </summary>
        private Paint mPaint;

        /// <summary>
        /// The paint.
        /// </summary>
        private Paint pastPaint;

        /// <summary>
        /// The distances x and y from start.
        /// </summary>
        public float[] Distances { get; set; }

        /// <summary>
        /// The constructor.
        /// </summary>
        public ZoomableImageView(Context context) : base(context)
        {
            matrix = new Matrix();
            mPaint = new Paint();
            pastPaint = new Paint(PaintFlags.AntiAlias)
            {
                StrokeWidth = 60
            };
            _bufferPoints = new List<CircleArea>();
            _currentCircle = null;
            currentPointZooming = new Point();
            currentPointBitmapZooming = new Point();
        }

        /// <summary>
        /// The onDraw event.
        /// </summary>
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (zooming)
            {
                // If we go out of the 
                var zoomRadius = 400;
                var circleWeight = 50;
                var focusElement = _bufferPoints.Where(a => a.Id == _currentCircle.Id).FirstOrDefault();
                var distanceX = (focusElement.PositionX - currentPointBitmapZooming.X) / 2;
                var distanceY = (focusElement.PositionY - currentPointBitmapZooming.Y) / 2;
                var xSquare = Math.Pow(Math.Abs(distanceX), 2);
                var ySquare = Math.Pow(Math.Abs(distanceY) - 2 * circleWeight, 2);
                if (Math.Sqrt(xSquare + ySquare) > zoomRadius + circleWeight / 2)
                {
                    this.DisableZoom();
                }

                var diffx = currentPointBitmapZooming.X - currentPointZooming.X;
                var diffy = currentPointBitmapZooming.Y - currentPointZooming.Y;
                matrix.Reset();
                matrix.PostScale(2f, 2f, currentPointBitmapZooming.X + currentPointZooming.X, currentPointBitmapZooming.Y + currentPointZooming.Y);
                mPaint.Shader.SetLocalMatrix(matrix);
                canvas.DrawCircle(currentPointZooming.X, currentPointZooming.Y, zoomRadius, mPaint);
                foreach (var circle in _bufferPoints)
                {    
                    pastPaint.Color = circle.Color;
                    var xPos = _currentCircle.Id == circle.Id ? circle.PositionX / 2 : circle.PositionX - diffx;
                    canvas.DrawCircle(xPos, circle.PositionY / 2 - circleWeight - diffy, circleWeight, pastPaint);
                }
                this.Invalidate();
            }
        }

        /// <summary>
        /// The method to enable zoom.
        /// </summary>
        public void EnableZoom(Point viewPoint, Point bitmapPoint, List<CircleArea> bufferPoints, CircleArea currentCircle)
        {
            zooming = true;
            this.Distances = null;
            _bufferPoints = bufferPoints;
            _currentCircle = currentCircle;
            currentPointZooming.X = viewPoint.X;
            currentPointZooming.Y = viewPoint.Y;
            currentPointBitmapZooming.X = bitmapPoint.X;
            currentPointBitmapZooming.Y = bitmapPoint.Y;
        }
        
        /// <summary>
        /// The method to disable zoom.
        /// </summary>
        public void DisableZoom()
        {
            var focusElement = _bufferPoints.Where(a => a.Id == _currentCircle.Id).FirstOrDefault();
            var diffX = (focusElement.PositionX - currentPointBitmapZooming.X) / 2;
            var diffY = (focusElement.PositionY - currentPointBitmapZooming.Y) / 2;
            this.Distances = new float[] { diffX, diffY };
            zooming = false;
            currentPointZooming = new Point();
            currentPointBitmapZooming = new Point();
            _bufferPoints = new List<CircleArea>();
            _currentCircle = null;
        }

        /// <summary>
        /// The set paint shader method.
        /// </summary>
        public void SetPaintShader(BitmapShader sentShader)
        {
            mPaint.SetShader(sentShader);
        }
    }
}