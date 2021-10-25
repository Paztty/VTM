using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace HVT.Utility
{
        public static class BitmapHelper
        {
            public static BitmapImage BitmapToImageSource(Bitmap bitmap)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();
                    bitmapimage.Freeze();
                    return bitmapimage;
                }
            }
        }
}
