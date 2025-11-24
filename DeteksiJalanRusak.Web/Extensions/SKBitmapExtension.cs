using SkiaSharp;

namespace DeteksiJalanRusak.Web.Extensions;

public static class SKBitmapExtension
{
    public static string ToBase64String(this SKBitmap bitmap) =>
        $"data:image/png;base64,{Convert.ToBase64String(bitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray())}";
}
