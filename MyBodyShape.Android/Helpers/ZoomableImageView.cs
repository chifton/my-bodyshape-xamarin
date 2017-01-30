/**********************************************************/
/*************** The zoomable image view
/**********************************************************/

using Android.Content;
using Android.Graphics;
using Android.Widget;
using System.Collections.Generic;

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
                var zoomRadius = 400;
                matrix.Reset();
                matrix.PostScale(2f, 2f, currentPointBitmapZooming.X + currentPointZooming.X, currentPointBitmapZooming.Y + currentPointZooming.Y);
                mPaint.Shader.SetLocalMatrix(matrix);
                canvas.DrawCircle(currentPointZooming.X, currentPointZooming.Y, zoomRadius, mPaint);
                foreach (var circle in _bufferPoints)
                {
                    var diffx = currentPointBitmapZooming.X - currentPointZooming.X;
                    var diffy = currentPointBitmapZooming.Y - currentPointZooming.Y;
                    pastPaint.Color = circle.Color;
                    canvas.DrawCircle(circle.PositionX - diffx, circle.PositionY - diffy, 50, pastPaint);
                }
                this.Invalidate();
            }
        }

        /// <summary>
        /// The method to enable zoom.
        /// </summary>
        public void EnableZoom(Point viewPoint, Point bitmapPoint, List<CircleArea> bufferPoints)
        {
            zooming = true;
            _bufferPoints = bufferPoints; 
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
            zooming = false;
            currentPointZooming = new Point();
            currentPointBitmapZooming = new Point();
            _bufferPoints = new List<CircleArea>();
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