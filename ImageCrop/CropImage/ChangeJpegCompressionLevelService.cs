using Sitecore.Resources.Media;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitchHK.CropImageLibrary.CropImage
{
    public class ChangeJpegCompressionLevelService
    {
        public static Stream Change(MediaStream mediaStream, int jpegQuality)
        {
            var jpegEncoder = GetEncoder(ImageFormat.Jpeg);
            var qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
            var encoderParameters = new EncoderParameters(1);
            var bitmap = new Bitmap(mediaStream.Stream);
            encoderParameters.Param[0] = new EncoderParameter(qualityEncoder, jpegQuality);
            var stream = new MemoryStream();
            bitmap.Save(stream, jpegEncoder, encoderParameters);
            return stream;
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
    }
}
