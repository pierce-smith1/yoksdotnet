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

            var bitmapName = bitmap.Match(
                classic => $"classic-{classic.Name}",
                refined => $"new-{refined.Name}"
            );
            var imagePath = Path.Combine(_exportPath, $"{bitmapName}.png");

            var size = ClassicBitmap.Size;

            var recorder = new SKPictureRecorder();
            var canvas = recorder.BeginRecording(SKRect.Create(size, size));

            canvas.Clear();

            var subject = CreatureCreation.NewPreviewYokin(bitmap);
            subject.basis.home.X += size / 2.0;
            subject.basis.home.Y += size / 2.0;
            subject.skin!.palette = palette;

            SpritePainter.Draw(canvas, subject);

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
