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
        /// The zoom radius.
        /// </summary>
        private const int ZOOM_RADIUS = 400;

        /// <summary>
        /// The circle weight.
        /// </summary>
        private const int CIRCLE_WEIGHT = 50;

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
        /// The zoom twin superman circles.
        /// </summary>
        private List<CircleArea> _zoomTwinCircles;

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
            _zoomTwinCircles = new List<CircleArea>();
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
                //matrix.Reset();
                //matrix.SetScale(1, 1, ToCanvasCoordinates(canvas.Height / 2, canvas.Height, currentPointZooming.X, currentPointBitmapZooming.Y + currentPointZooming.Y);
                //mPaint.Shader.SetLocalMatrix(matrix);
                //canvas.DrawRect(0, 0, canvas.Width, canvas.Height, mPaint);


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
            _zoomTwinCircles = new List<CircleArea>();
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