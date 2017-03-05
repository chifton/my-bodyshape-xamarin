/**********************************************************/
/*************** The front move image runnable
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
using System.Linq;

namespace MyBodyShape.Android.Listeners
{
    /// <summary>
    /// The move image runnable.
    /// </summary>
    public class FrontMoveImageRunnable : Java.Lang.Object, IRunnable
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
        /// The paint.
        /// </summary>
        private Paint _pathPaint;

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
        /// The constants.
        /// </summary>
        private const int LEFT_BUTTON_ID = 1000;
        private const int RIGHT_BUTTON_ID = 1001;
        private const int TOP_BUTTON_ID = 1002;
        private const int DOWN_BUTTON_ID = 1003;
        private const int ZOOM_BUTTON_ID = 1004;
        private const int UNZOOM_BUTTON_ID = 1005;

        /// <summary>
        /// The circle list.
        /// </summary>
        public List<CircleArea> CirclesList { get; set; }

        /// <summary>
        /// The circle list.
        /// </summary>
        public List<PathArea> PathList { get; set; }

        /// <summary>
        /// The constructor.
        /// </summary>
        public FrontMoveImageRunnable( ViewImageView.View fragmentView, Handler repeatedHandler, ImageButton currentButton, Canvas canvas, Paint paint, Paint pathPaint, float radius, List<CircleArea> circlesList, List<PathArea> pathList)
        {
            _imageButton = currentButton;
            _repeatedHandler = repeatedHandler;
            _fragmentView = fragmentView;
            _canvas = canvas;
            _paint = paint;
            _pathPaint = pathPaint;
            _radius = radius;
            CirclesList = circlesList;
            PathList = pathList;
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
                case LEFT_BUTTON_ID:
                    CurrentX -= 20;
                    ScaleIndicator = 0;
                    break;
                case RIGHT_BUTTON_ID:
                    CurrentX += 20;
                    ScaleIndicator = 0;
                    break;
                case TOP_BUTTON_ID:
                    CurrentY -= 20;
                    ScaleIndicator = 0;
                    break;
                case DOWN_BUTTON_ID:
                    CurrentY += 20;
                    ScaleIndicator = 0;
                    break;
                case ZOOM_BUTTON_ID:
                    ScaleIndicator = 30;
                    scaling = true;
                    break;
                case UNZOOM_BUTTON_ID:
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
        public void Update(int x, int y, double scale, List<CircleArea> list, List<PathArea> pathList)
        {
            this.CurrentX = x;
            this.CurrentY = y;
            this.ScaleIndicator = scale;
            this.CirclesList = list;
            this.PathList = pathList;
        }

        /// <summary>
        /// The ReDrawAll method at every move.
        /// </summary>
        private void ReDrawAll(int x, int y, double scale, bool scaling)
        {
            _canvas.DrawColor(Color.Black, PorterDuff.Mode.Clear);

            if (scaling)
            {
                var ratio = (double)App1.bitmap.Width / App1.bitmap.Height;
                var newHeight = (int)(App1.bitmap.Height + scale);
                App1.bitmap = BitmapHelpers.ResizeCurrentBitmap(App1.bitmap, newHeight, (int)(newHeight * ratio));
            }

            _canvas.DrawBitmap(App1.bitmap, x, y, _paint);

            // Paths
            foreach (PathArea path in PathList)
            {
                this.DrawBodyShapePath(path.Id, path.Points, false);
            }

            // Circles
            foreach (CircleArea circle in CirclesList)
            {
                _paint.Color = circle.Color;
                _canvas.DrawCircle(circle.PositionX, circle.PositionY, _radius, _paint);
            }
        }

        /// <summary>
        /// The draw bodyshape path method.
        /// </summary>
        private void DrawBodyShapePath(string key, string[] points, bool create)
        {
            if (points.Length == 2)
            {
                var firstPoint = CirclesList.Where(b => b.Id == points[0]).FirstOrDefault();
                var secondPoint = CirclesList.Where(b => b.Id == points[1]).FirstOrDefault();

                var result = this.Draw2PathArea(key, firstPoint.Color, new CircleArea[] { firstPoint, secondPoint });
                if (create)
                {
                    PathList.Add(result);
                }
            }
            else if (points.Length == 3)
            {
                var firstPoint = CirclesList.Where(b => b.Id == points[0]).FirstOrDefault();
                var secondPoint = CirclesList.Where(b => b.Id == points[1]).FirstOrDefault();
                var thirdPoint = CirclesList.Where(b => b.Id == points[2]).FirstOrDefault();

                var result = this.Draw3PathArea(key, firstPoint.Color, new CircleArea[] { firstPoint, secondPoint, thirdPoint });
                if (create)
                {
                    PathList.Add(result);
                }
            }
            else if (points.Length == 4)
            {
                var firstPoint = CirclesList.Where(b => b.Id == points[0]).FirstOrDefault();
                var secondPoint = CirclesList.Where(b => b.Id == points[1]).FirstOrDefault();
                var thirdPoint = CirclesList.Where(b => b.Id == points[2]).FirstOrDefault();
                var fourthPoint = CirclesList.Where(b => b.Id == points[3]).FirstOrDefault();

                var result = this.Draw4PathArea(key, firstPoint.Color, new CircleArea[] { firstPoint, secondPoint, thirdPoint, fourthPoint });
                if (create)
                {
                    PathList.Add(result);
                }
            }
            else if (points.Length == 5)
            {
                var firstPoint = CirclesList.Where(b => b.Id == points[0]).FirstOrDefault();
                var secondPoint = CirclesList.Where(b => b.Id == points[1]).FirstOrDefault();
                var thirdPoint = CirclesList.Where(b => b.Id == points[2]).FirstOrDefault();
                var fourthPoint = CirclesList.Where(b => b.Id == points[3]).FirstOrDefault();
                var fifthPoint = CirclesList.Where(b => b.Id == points[4]).FirstOrDefault();

                var result = this.Draw5PathArea(key, firstPoint.Color, new CircleArea[] { firstPoint, secondPoint, thirdPoint, fourthPoint, fifthPoint });
                if (create)
                {
                    PathList.Add(result);
                }
            }
        }

        /// <summary>
        /// The draw path area method for 2 points.
        /// </summary>
        private PathArea Draw2PathArea(string id, Color color, CircleArea[] points)
        {
            var smallRadius = _radius / 2;
            _pathPaint.Color = color;
            Path path = new Path();
            path.SetFillType(Path.FillType.Winding);
            path.SetLastPoint(points[0].PositionX + smallRadius * RightSide(points[0].PositionX), points[0].PositionY);
            path.MoveTo(points[0].PositionX + smallRadius * RightSide(points[0].PositionX), points[0].PositionY);
            path.LineTo(points[1].PositionX + smallRadius * RightSide(points[1].PositionX), points[1].PositionY);
            _canvas.DrawPath(path, _pathPaint);
            return new PathArea(id, color, points.Select(p => p.Id).ToArray());
        }

        /// <summary>
        /// The draw path area method for 3 points.
        /// </summary>
        private PathArea Draw3PathArea(string id, Color color, CircleArea[] points)
        {
            var smallRadius = _radius / 2;
            _pathPaint.Color = color;
            Path path = new Path();
            path.SetFillType(Path.FillType.Winding);
            path.SetLastPoint(points[0].PositionX + smallRadius * RightSide(points[0].PositionX), points[0].PositionY);
            path.MoveTo(points[0].PositionX + smallRadius * RightSide(points[0].PositionX), points[0].PositionY);
            path.LineTo(points[1].PositionX + smallRadius * RightSide(points[1].PositionX), points[1].PositionY);
            path.QuadTo(points[1].PositionX + smallRadius * RightSide(points[1].PositionX), points[1].PositionY, points[2].PositionX + smallRadius * RightSide(points[2].PositionX), points[2].PositionY);
            _canvas.DrawPath(path, _pathPaint);
            return new PathArea(id, color, points.Select(p => p.Id).ToArray());
        }

        /// <summary>
        /// The draw path area method for 4 points.
        /// </summary>
        private PathArea Draw4PathArea(string id, Color color, CircleArea[] points)
        {
            var smallRadius = _radius / 2;
            _pathPaint.Color = color;
            Path path = new Path();
            path.SetFillType(Path.FillType.Winding);
            path.SetLastPoint(points[0].PositionX + smallRadius * RightSide(points[0].PositionX), points[0].PositionY);
            path.MoveTo(points[0].PositionX + smallRadius * RightSide(points[0].PositionX), points[0].PositionY);
            path.LineTo(points[1].PositionX + smallRadius * RightSide(points[1].PositionX), points[1].PositionY);
            path.QuadTo(points[1].PositionX + smallRadius * RightSide(points[1].PositionX), points[1].PositionY, points[2].PositionX + smallRadius * RightSide(points[2].PositionX), points[2].PositionY);
            path.MoveTo(points[2].PositionX + smallRadius * RightSide(points[2].PositionX), points[2].PositionY);
            path.LineTo(points[3].PositionX + smallRadius * RightSide(points[3].PositionX), points[3].PositionY);
            _canvas.DrawPath(path, _pathPaint);
            return new PathArea(id, color, points.Select(p => p.Id).ToArray());
        }

        /// <summary>
        /// The draw path area method for 5 points.
        /// </summary>
        private PathArea Draw5PathArea(string id, Color color, CircleArea[] points)
        {
            var smallRadius = _radius / 2;
            _pathPaint.Color = color;
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
            _canvas.DrawPath(path, _pathPaint);
            return new PathArea(id, color, points.Select(p => p.Id).ToArray());
        }

        /// <summary>
        /// The right side position from center canvas.
        /// </summary>
        private int RightSide(float position)
        {
            if (position > _canvas.Width / 2)
            {
                return 1;
            }
            else if (position < _canvas.Width / 2)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}