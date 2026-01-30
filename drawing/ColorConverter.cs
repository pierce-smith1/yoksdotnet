using System;
using System.Linq;

namespace yoksdotnet.drawing;

public static class ColorConverter
{
    public static RgbColor FromHex(string hex)
    {
        hex = hex.Trim();

        if (hex.StartsWith('#'))
        {
            hex = hex[1..];
        }

        try
        {
            if (hex.Length != 6)
            {
                throw new InvalidOperationException($"Color hex code '{hex}' is not valid");
            }

            var red = Convert.ToByte(hex[..2], 16);
            var green = Convert.ToByte(hex[2..4], 16);
            var blue = Convert.ToByte(hex[4..6], 16);

            return new(red, green, blue);
        } 
        catch (FormatException)
        {
            throw new InvalidOperationException($"Color hex code '{hex}' is not valid");
        }
    }
    public static string ToHex(RgbColor color)
    {
        var hex = $"#{color.R:x2}{color.G:x2}{color.B:x2}";
        return hex;
    }

    public static HslColor ToHsl(RgbColor color)
    {
        var rp = color.R / 255.0;
        var gp = color.G / 255.0;
        var bp = color.B / 255.0;

        var cmax = new[] { rp, gp, bp }.Max();
        var cmin = new[] { rp, gp, bp }.Min();
        var delta = cmax - cmin;

        var h = (delta == 0) ? 0
            : (cmax == rp) ? 60 * (((gp - bp) / delta) % 6)
            : (cmax == gp) ? 60 * (((bp - rp) / delta) + 2)
            : (cmax == bp) ? 60 * (((rp - gp) / delta) + 4)
            : 0;

        while (h < 0)
        {
            h += 360;
        }

        var l = (cmax + cmin) / 2;

        var s = (delta == 0) ? 0
            : delta / (1 - Math.Abs(2 * l - 1));

        return new(h, s * 100, l * 100);
    }
    public static RgbColor FromHsl(HslColor color)
    {
        var (h, s, l) = color;

        s /= 100;
        l /= 100;

        var c = (1 - Math.Abs(2 * l - 1)) * s;
        var x = c * (1 - Math.Abs((h / 60 % 2) - 1));
        var m = l - c / 2;

        var (rp, gp, bp) = h switch
        {
            < 60 => (c, x, 0),
            >= 60 and < 120 => (x, c, 0),
            >= 120 and < 180 => (0, c, x),
            >= 180 and < 240 => (0, x, c),
            >= 240 and < 300 => (x, 0, c),
            >= 300 => (c, 0, x),
            _ => (0.0, 0.0, 0.0)
        };

        var rc = rp + m;
        var gc = gp + m;
        var bc = bp + m;

        var r = (byte)Math.Round(rc * 255);
        var g = (byte)Math.Round(gc * 255);
        var b = (byte)Math.Round(bc * 255);

        return new(r, g, b);
    }
}
