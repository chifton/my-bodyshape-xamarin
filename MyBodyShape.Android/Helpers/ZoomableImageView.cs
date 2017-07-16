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
        private const int ZOOM_RADIUS = 500;

        /// <summary>
        /// The circle weight.
        /// </summary>
        private const int CIRCLE_WEIGHT = 50;

        /// <summary>
        /// The zoomable image view boolean.
        /// </summary>
        private bool zooming;

        /// <summary>
        /// The zoomable image view current point.
        /// </summary>
        private Point currentPointZooming;
        
        /// <summary>
        /// The zoomable canvas current point.
        /// </summary>
        private Point currentPointBitmapZooming;

        /// <summary>
        /// The static zoomable image view current point.
        /// </summary>
        private Point staticCurrentPointZooming;

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
        /// The canvas X ratio.
        /// </summary>
        private double canvasRatioX;

        /// <summary>
        /// The canvas Y ratio.
        /// </summary>
        private double canvasRatioY;
        
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
            staticCurrentPointZooming = new Point();
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
                var focusElement = _bufferPoints.Where(a => a.Id == _currentCircle.Id).FirstOrDefault();

                var newPivotX = 2 * (currentPointBitmapZooming.X - ZOOM_RADIUS);
                var newPivotY = 2 * (currentPointBitmapZooming.Y - ZOOM_RADIUS);

                matrix.Reset();
                matrix.PostScale(2f, 2f, newPivotX, newPivotY);
                mPaint.Shader.SetLocalMatrix(matrix);
                canvas.DrawRect(0, 0, canvas.Width, canvas.Height, mPaint);
                foreach (var circle in _bufferPoints)
                {
                    pastPaint.Color = circle.Color;
                    if (_currentCircle.Id == circle.Id)
                    { 
                        canvas.DrawCircle(currentPointZooming.X, currentPointZooming.Y, CIRCLE_WEIGHT, pastPaint);
                    }
                    else
                    {
                        var diffX = 2 * (currentPointBitmapZooming.X - circle.PositionX) * canvasRatioX;
                        var diffY = 2 * (currentPointBitmapZooming.Y - circle.PositionY) * canvasRatioY;
                        canvas.DrawCircle((float) (staticCurrentPointZooming.X - diffX), (float) (staticCurrentPointZooming.Y - diffY), CIRCLE_WEIGHT, pastPaint);
                    }
                }
                
                this.Invalidate();
            }
        }
        
        /// <summary>
        /// The method to enable zoom.
        /// </summary>
        public void EnableZoom(Point viewPoint, Point bitmapPoint, List<CircleArea> bufferPoints, CircleArea currentCircle, double ratioX, double ratioY)
        {
            zooming = true;
            this.Distances = null;
            _bufferPoints = bufferPoints;
            _currentCircle = currentCircle;
            currentPointZooming.X = viewPoint.X;
            currentPointZooming.Y = viewPoint.Y;
            staticCurrentPointZooming.X = viewPoint.X;
            staticCurrentPointZooming.Y = viewPoint.Y;
            currentPointBitmapZooming.X = bitmapPoint.X;
            currentPointBitmapZooming.Y = bitmapPoint.Y;
            canvasRatioX = ratioX;
            canvasRatioY = ratioY;
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
            staticCurrentPointZooming = new Point();
            _bufferPoints = new List<CircleArea>();
            _zoomTwinCircles = new List<CircleArea>();
            _currentCircle = null;
            canvasRatioX = 0;
            canvasRatioY = 0;
        }

        /// <summary>
        /// The move pivot point method.
        /// </summary>
        public void MovePivotPoint(float x, float y)
        {
            if (zooming)
            {
                currentPointZooming.X = (int)x;
                currentPointZooming.Y = (int)y;
            }
        }

        /// <summary>
        /// The get new pivot position method.
        /// </summary>
        public Point GetNewPivotPosition()
        {
            return new Point(currentPointZooming.X, currentPointZooming.Y);
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