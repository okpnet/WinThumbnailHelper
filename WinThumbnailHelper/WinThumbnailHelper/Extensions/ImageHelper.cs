using System.Drawing;

namespace WinThumbnailHelper.Extensions
{
    public static class ImageHelper
    {
        internal static Size ChangeScaleRatio(this Image image, int maxSide)
        {
            var resultSize = image.Width > image.Height ?
                new Size(maxSide, (int)(image.Height * (float)maxSide / image.Width)) : new Size((int)(image.Width * (float)maxSide / image.Width), maxSide);
            return resultSize;
        }
        /// <summary>
        /// サムネイル生成
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxsize"></param>
        /// <returns></returns>
        public static Image CreateThumbnail(this Image image, int maxsize)
        {
            var size = image.ChangeScaleRatio(maxsize);
            return image.GetThumbnailImage(size.Width, size.Height, null, nint.Zero);
        }
    }
}
