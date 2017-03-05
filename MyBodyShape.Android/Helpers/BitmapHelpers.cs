/**********************************************************/
/*************** The bitmap helpers
/**********************************************************/

using Android.Graphics;
using Android.Media;
using Android.Provider;

namespace MyBodyShape.Android.Helpers
{
    public static class BitmapHelpers
    {
        /// <summary>
        /// Helper method for taken pictures
        /// </summary>
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // Source dimensions
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Ratio
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Resize
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            // Rotation
            Matrix mtx = new Matrix();
            ExifInterface exif = new ExifInterface(fileName);
            string orientation = exif.GetAttribute(ExifInterface.TagOrientation);
            switch (orientation)
            {
                case "6":
                    mtx.PreRotate(90);
                    resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, mtx, false);
                    mtx.Dispose();
                    mtx = null;
                    break;
                case "1":
                    break;
                default:
                    mtx.PreRotate(90);
                    resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, mtx, false);
                    mtx.Dispose();
                    mtx = null;
                    break;
            }

            return resizedBitmap;
        }

        /// <summary>
        /// Helper method for gallery pictures
        /// </summary>
        public static Bitmap LoadInGalleryAndResizeBitmap(this System.IO.Stream fileName, int width, int height, Bitmap bitmap)
        {
            // Source dimensions
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeStream(fileName, new Rect(), options);

            // Ratio
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Resize
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = bitmap;
            
            // Rotation
            Matrix mtx = new Matrix();
            ExifInterface exif = new ExifInterface(fileName);
            string orientation = exif.GetAttribute(ExifInterface.TagOrientation);
            switch (orientation)
            {
                case "6":
                    mtx.PreRotate(90);
                    resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, mtx, false);
                    mtx.Dispose();
                    mtx = null;
                    break;
                case "1":
                    break;
                default:
                    mtx.PreRotate(90);
                    resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, mtx, false);
                    mtx.Dispose();
                    mtx = null;
                    break;
            }

            return resizedBitmap;
        }

        /// <summary>
        /// Resizes the current bitmap.
        /// </summary>
        public static Bitmap ResizeCurrentBitmap(Bitmap bm, int newHeight, int newWidth)
        {
            int width = bm.Width;
            int height = bm.Height;
            float scaleWidth = ((float)newWidth) / width;
            float scaleHeight = ((float)newHeight) / height;
            Matrix matrix = new Matrix();
            matrix.PostScale(scaleWidth, scaleHeight);
            Bitmap resizedBitmap = Bitmap.CreateBitmap(bm, 0, 0, width, height, matrix, false);
            bm.Dispose();
            return resizedBitmap;
        }
    }
}