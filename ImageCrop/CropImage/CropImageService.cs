using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Sitecore.Resources.Media;
using Sitecore.Data.Items;
using System.Drawing.Drawing2D;
using FitchHK.SitecoreCommon.Enums;

namespace FitchHK.CropImageLibrary.CropImage
{
    public class CropImageService
    {
        public static Stream CenterCrop(Stream imageStream, int width, int height, ImageFormat format, HorizontalAlignment cha, VerticalAlignment cva)
        {
            var bitmap = new System.Drawing.Bitmap(imageStream);
            if (bitmap.Width == width && bitmap.Height == height)
                return imageStream;
            var cropWidth = width < bitmap.Width && width != 0 ? width : bitmap.Width;
            var cropHeight = height < bitmap.Height && height != 0 ? height : bitmap.Height;

            var x = 0;
            if (cha == HorizontalAlignment.Center)
                x = cropWidth == bitmap.Width ? 0 : (bitmap.Width / 2) - (cropWidth / 2);
            else if (cha == HorizontalAlignment.Left)
                x = 0;
            else if(cha == HorizontalAlignment.Right)
                x = cropWidth == bitmap.Width ? 0 : (bitmap.Width - cropWidth);

            var y = 0;
            if (cva == VerticalAlignment.Middle)
                y = cropHeight == bitmap.Width ? 0 : (bitmap.Height / 2) - (cropHeight / 2);
            else if (cva == VerticalAlignment.Top)
                y = 0;
            else if (cva == VerticalAlignment.Bottom)
                y = cropHeight == bitmap.Height ? 0 : (bitmap.Height - cropHeight);

            var croppedBitmap = CropImage(bitmap, x, y, cropWidth, cropHeight);
            var memoryStream = new MemoryStream();
            croppedBitmap.Save(memoryStream, format);
            return memoryStream;
        }
        public static System.Drawing.Bitmap CropImage(System.Drawing.Bitmap originalImage, int x, int y, int width, int height)
        {
            if (x < 0 || ((x + width) > originalImage.Width) || (y < 0) || ((y + height) > originalImage.Height))
                return originalImage;
            var rectangle = new Rectangle(x, y, width, height);
            var croppedImage = new System.Drawing.Bitmap(rectangle.Width, rectangle.Height);
            using (var g = Graphics.FromImage(croppedImage))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.DrawImage(originalImage, new Rectangle(0, 0, croppedImage.Width, croppedImage.Height), rectangle, GraphicsUnit.Pixel);
            }
            return croppedImage;
        }
    }
}
