using Microsoft.AspNetCore.Http;
using System.Drawing.Imaging;

public static class ImageProcessing
{
    public static string GetThumbnailPath(string originalFullPath, int width = 80, int height = 80)
    {
        var dir = Path.GetDirectoryName(originalFullPath)
                  ?? throw new ArgumentException("Ścieżka nie zawiera katalogu.", nameof(originalFullPath));

        var name = Path.GetFileNameWithoutExtension(originalFullPath);
        var ext = Path.GetExtension(originalFullPath);

        return Path.Combine(dir,"thumbnails", $"{name}_thumb_{width}x{height}{DateTime.Now.Minute}{ext}");
    }

    public static async Task CreateCenterCroppedThumbnailAsync(
        IFormFile file,
        string thumbnailFullPath,
        int width = 80,
        int height = 80,
        System.Threading.CancellationToken ct = default)
    {
        if (file == null) throw new System.ArgumentNullException(nameof(file));
        if (file.Length <= 0) throw new System.ArgumentException("Plik jest pusty.", nameof(file));
        if (string.IsNullOrWhiteSpace(thumbnailFullPath)) throw new System.ArgumentException("Brak ścieżki wyjściowej.", nameof(thumbnailFullPath));
        if (width <= 0) throw new System.ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new System.ArgumentOutOfRangeException(nameof(height));

        ct.ThrowIfCancellationRequested();

        await using var input = file.OpenReadStream();

        // Jawnie System.Drawing.Image (odporne na konflikt z System.Net.Mime.MediaTypeNames.Image)
        using var source = System.Drawing.Image.FromStream(input, useEmbeddedColorManagement: true, validateImageData: true);

        int side = System.Math.Min(source.Width, source.Height);
        int cropX = (source.Width - side) / 2;
        int cropY = (source.Height - side) / 2;

        var srcRect = new System.Drawing.Rectangle(cropX, cropY, side, side);

        using var dest = new System.Drawing.Bitmap(width, height, PixelFormat.Format32bppArgb);

        using (var g = System.Drawing.Graphics.FromImage(dest))
        {
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            using var wrap = new ImageAttributes();
            wrap.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);

            g.DrawImage(
                source,
                new System.Drawing.Rectangle(0, 0, width, height),
                srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height,
                System.Drawing.GraphicsUnit.Pixel,
                wrap
            );
        }

        var outDir = System.IO.Path.GetDirectoryName(thumbnailFullPath);
        if (!string.IsNullOrWhiteSpace(outDir))
            System.IO.Directory.CreateDirectory(outDir);

        var format = GetImageFormatByExtension(System.IO.Path.GetExtension(thumbnailFullPath));

        try
        {
            dest.Save(thumbnailFullPath, format);
        }
        catch (Exception ex)
        {

        }
    }

    private static ImageFormat GetImageFormatByExtension(string? ext)
    {
        ext = (ext ?? "").ToLowerInvariant();
        return ext switch
        {
            ".png" => ImageFormat.Png,
            ".bmp" => ImageFormat.Bmp,
            ".gif" => ImageFormat.Gif,
            ".tif" or ".tiff" => ImageFormat.Tiff,
            ".jpg" or ".jpeg" => ImageFormat.Jpeg,
            _ => ImageFormat.Jpeg
        };
    }
}
