using SkiaSharp;
using System;
using System.IO;
using yoksdotnet.drawing.painters;
using yoksdotnet.logic.scene;

namespace yoksdotnet.drawing;

public class ImageExporter(string _exportPath)
{
    public ImageExportResult Export(Bitmap bitmap, Palette palette)
    {
        try
        {
            Directory.CreateDirectory(_exportPath);

            var imagePath = Path.Combine(_exportPath, $"{bitmap.Name}.png");

            var size = Bitmap.BitmapSize();

            var recorder = new SKPictureRecorder();
            var canvas = recorder.BeginRecording(SKRect.Create(size, size));

            canvas.Clear();
            SpritePainter.Draw(canvas, new Sprite()
            {
                palette = palette,
                addons = new()
                {
                    fixedBitmap = bitmap,
                },
            });

            var picture = recorder.EndRecording();
            var image = SKImage.FromPicture(picture, new SKSizeI(size, size));
            var encodedImage = image.Encode(SKEncodedImageFormat.Png, 100);

            using var stream = new FileStream(imagePath, FileMode.Create);
            encodedImage.SaveTo(stream);

            return ImageExportResult.Ok;
        }
        catch (UnauthorizedAccessException)
        {
            return ImageExportResult.NoPermission;
        }
        catch (IOException)
        {
            return ImageExportResult.UnknownError;
        }
    }
}

public enum ImageExportResult
{
    Ok,
    NoPermission,
    UnknownError,
}
