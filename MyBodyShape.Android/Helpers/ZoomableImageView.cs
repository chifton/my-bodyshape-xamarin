/**********************************************************/
/*************** The zoomable image view
/**********************************************************/

using Android.Content;
using Android.Graphics;
using Android.Widget;

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
        /// The canvas matrix.
        /// </summary>
        private Matrix matrix;

        /// <summary>
        /// The paint.
        /// </summary>
        private Paint mPaint;

        /// <summary>
        /// The constructor.
        /// </summary>
        public ZoomableImageView(Context context) : base(context)
        {
            matrix = new Matrix();
            mPaint = new Paint();
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
                var ratioX = (double) currentPointBitmapZooming.X / currentPointZooming.X;
                var ratioY = (double)currentPointBitmapZooming.Y / currentPointZooming.Y;
                matrix.Reset();
                matrix.PostScale(2f, 2f, currentPointBitmapZooming.X + currentPointZooming.X, currentPointBitmapZooming.Y + currentPointZooming.Y);
                // , currentPointZooming.X * 2, currentPointZooming.Y * 2 - 200);
                mPaint.Shader.SetLocalMatrix(matrix);
                
                canvas.DrawCircle(currentPointZooming.X, currentPointZooming.Y, 400, mPaint);
            }
        }

        /// <summary>
        /// The method to enable zoom.
        /// </summary>
        public void EnableZoom(Point viewPoint, Point bitmapPoint)
        {
            zooming = true;
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