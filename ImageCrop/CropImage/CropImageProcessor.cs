using FitchHK.SitecoreCommon.Enums;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitchHK.CropImageLibrary.CropImage
{
    public class CropImageProcessor
    {
        private string CropQueryKey
        {
            get { return Settings.GetSetting("LaubPlusCo.CropQueryKey", "c"); }
        }
        private string CropWidthQueryKey
        {
            get { return Settings.GetSetting("LaubPlusCo.CropWidthQueryKey", "cw"); }
        }
        private string CropHeightQueryKey
        {
            get { return Settings.GetSetting("LaubPlusCo.CropHeightQueryKey", "ch"); }
        }

        private string CropVerticalAlignmentKey
        {
            get { return Settings.GetSetting("LaubPlusCo.VerticalAlignment", "cva"); }
        }

        private string CropHorizontalAlignmentKey
        {
            get { return Settings.GetSetting("LaubPlusCo.HorizontalAlignment", "cha"); }
        }

        public IEnumerable<string> ValidMimeTypes
        {
            get
            {
                var validMimetypes = Settings.GetSetting("LaubPlusCo.CropValidMimeTypes", "image/jpeg|image/pjpeg|image/png|image/gif|image/tiff|image/bmp");
                return validMimetypes.Split(new[] { ",", "|", ";" }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        public void Process(GetMediaStreamPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.Options.Thumbnail || !IsValidImageRequest(args.MediaData.MimeType))
                return;
            if (args.OutputStream == null || !args.OutputStream.AllowMemoryLoading)
                return;
            var cropKey = GetQueryOrCustomOption(CropQueryKey, args.Options.CustomOptions);
            if (string.IsNullOrEmpty(cropKey))
                return;
            var cropWidthOption = GetQueryOrCustomOption(CropWidthQueryKey, args.Options.CustomOptions);
            var cropHeightOption = GetQueryOrCustomOption(CropHeightQueryKey, args.Options.CustomOptions);
            if (string.IsNullOrEmpty(cropWidthOption) && string.IsNullOrEmpty(cropHeightOption))
                return;
            int cropWidth;
            if (!int.TryParse(cropWidthOption, out cropWidth))
                cropWidth = args.Options.Width;
            int cropHeight;
            if (!int.TryParse(cropHeightOption, out cropHeight))
                cropHeight = args.Options.Height;

            var cha = GetQueryOrCustomOption(CropHorizontalAlignmentKey, args.Options.CustomOptions);

            var cropHorizontalAlignment = HorizontalAlignment.Center;
            if (string.IsNullOrEmpty(cha))
                cropHorizontalAlignment = HorizontalAlignment.Center;
            else
            {
                cropHorizontalAlignment = (HorizontalAlignment) Enum.Parse(typeof(HorizontalAlignment), cha, true);   
            }

            var cva = GetQueryOrCustomOption(CropVerticalAlignmentKey, args.Options.CustomOptions);

            var cropVerticalAlignment = VerticalAlignment.Middle;
            if (string.IsNullOrEmpty(cva))
                cropVerticalAlignment = VerticalAlignment.Middle;
            else
            {
                cropVerticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), cva, true);
            }

            var croppedStream = CropImageService.CenterCrop(
                args.OutputStream.Stream, 
                cropWidth, 
                cropHeight, 
                GetImageFormat(args.MediaData.MimeType.ToLower()),
                cropHorizontalAlignment,
                cropVerticalAlignment);
            args.OutputStream = new MediaStream(croppedStream, args.MediaData.Extension, args.OutputStream.MediaItem);
        }
        private ImageFormat GetImageFormat(string mimeType)
        {
            switch (mimeType)
            {
                case "image/jpeg":
                    return ImageFormat.Jpeg;
                case "image/pjpeg":
                    return ImageFormat.Jpeg;
                case "image/png":
                    return ImageFormat.Png;
                case "image/gif":
                    return ImageFormat.Gif;
                case "image/tiff":
                    return ImageFormat.Tiff;
                    ;
                case "image/bmp":
                    return ImageFormat.Bmp;
                default:
                    return ImageFormat.Jpeg;
            }
        }
        protected bool IsValidImageRequest(string mimeType)
        {
            return ValidMimeTypes.Any(v => v.Equals(mimeType, StringComparison.InvariantCultureIgnoreCase));
        }
        protected string GetQueryOrCustomOption(string key, StringDictionary customOptions)
        {
            var value = WebUtil.GetQueryString(key);
            return string.IsNullOrEmpty(value) ? customOptions[key] : value;
        }
    }
}
