using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitchHK.CropImageLibrary.CropImage
{
    public class SetJpegCompressionProcessor
    {
        private string JpegCompressionLevelQueryKey
        {
            get { return Settings.GetSetting("LaubPlusCo.JpegCompressionLevelQueryKey", "jq"); }
        }
        public void Process(GetMediaStreamPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.Options.Thumbnail || !IsJpegImageRequest(args.MediaData.MimeType))
                return;
            if (args.OutputStream == null || !args.OutputStream.AllowMemoryLoading)
                return;
            var jpegQualityQuery = WebUtil.GetQueryString(JpegCompressionLevelQueryKey);
            if (string.IsNullOrEmpty(jpegQualityQuery))
                return;
            int jpegQuality;
            if (!int.TryParse(jpegQualityQuery, out jpegQuality) || jpegQuality <= 0 || jpegQuality > 100)
                return;
            var compressedStream = ChangeJpegCompressionLevelService.Change(args.OutputStream, jpegQuality);
            args.OutputStream = new MediaStream(compressedStream, args.MediaData.Extension, args.OutputStream.MediaItem);
        }
        protected bool IsJpegImageRequest(string mimeType)
        {
            return mimeType.Equals("image/jpeg", StringComparison.InvariantCultureIgnoreCase)
            || mimeType.Equals("image/pjpeg", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
