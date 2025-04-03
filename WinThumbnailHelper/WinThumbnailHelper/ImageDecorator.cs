using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.WebSockets;
using System.Reflection;
using WinThumbnailHelper.Extensions;

namespace WinThumbnailHelper
{
    public class ImageDecorator: IImageDecorator,IDisposable
    {
        internal static readonly ReaderWriterLockSlim imageLock = new ReaderWriterLockSlim();
        private string _imagePath;
        private Image _thumbnail;

        public Image Thumbnail => _thumbnail;

        public Exception? Exceptions { get;private set; }

        public string ImageFilePath => _imagePath;

        private ImageDecorator(string imagePath, Image thumbnail, Exception? exceptions)
        {
            _imagePath = imagePath;
            _thumbnail = thumbnail;
            Exceptions = exceptions;
        }

        public static async Task<ImageDecorator> CreateAsync(string bitmapPath,int maxsidelength)
        {
            var result=await Task.Run(() =>
            {
                try
                {
                    var isSuccess = bitmapPath is not (null or "") && System.IO.File.Exists(bitmapPath);
                    Image bitmap = isSuccess ? new Bitmap(bitmapPath!) : GetDefaultImage();
                    var thumbnail = bitmap.CreateThumbnail(maxsidelength);
                    bitmap.Dispose();
                    return new ImageDecorator(bitmapPath??string.Empty, thumbnail, isSuccess ? null : new FileNotFoundException(bitmapPath));
                }
                catch (Exception ex)
                {
                    return new ImageDecorator(bitmapPath??string.Empty, new Bitmap(0, 0), ex);
                }
            });
            return result;
        }

        public static async Task<ImageDecorator> CreateAsync(string bitmapPath,Rectangle rectangle, int maxsidelength)
        {
            var result = await Task.Run(() =>
            {
                try
                {
                    var isSuccess = bitmapPath is not (null or "") && System.IO.File.Exists(bitmapPath);
                    var  baseImage = isSuccess ? new Bitmap(bitmapPath!) : GetDefaultImage();
                    var bitmap=baseImage.Clone(rectangle, baseImage.PixelFormat);
                    var thumbnail = bitmap.CreateThumbnail(maxsidelength);
                    baseImage.Dispose();
                    bitmap.Dispose();
                    return new ImageDecorator(bitmapPath ?? string.Empty, thumbnail, isSuccess ? null : new FileNotFoundException(bitmapPath));
                }
                catch (Exception ex)
                {
                    return new ImageDecorator(bitmapPath ?? string.Empty, new Bitmap(0, 0), ex);
                }
            });
            return result;
        }

        public static async Task<ImageDecorator> CreateAsync(Image image, int maxsidelength)
        {
            var result = await Task.Run(() =>
            {
                try
                {
                    using var memstream = new MemoryStream();
                    image.Save(memstream, System.Drawing.Imaging.ImageFormat.Bmp);
                    var bitmap = new Bitmap(memstream);
                    var thumbnail = bitmap.CreateThumbnail(maxsidelength);
                    return new ImageDecorator(string.Empty, thumbnail, null);
                }
                catch (Exception ex)
                {
                    return new ImageDecorator(string.Empty, new Bitmap(0, 0), ex);
                }
            });
            return result;
        }

        private static Bitmap GetDefaultImage()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(WinThumbnailHelper)}.Assets.noimage.bmp");
            return new Bitmap(stream!);
        }

        public void Dispose()
        {
            if(_thumbnail is not null)
            {
                _thumbnail.Dispose(); 
            }
            try{
                imageLock.EnterWriteLock();
                File.Delete(_imagePath);
            }finally 
            {
                imageLock.ExitWriteLock(); 
            }
        }
    }
}
