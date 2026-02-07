using SkiaSharp;
using yoksdotnet.common;

namespace yoksdotnet.drawing.painters;

public static class ColorSelectPainter
{
    private static readonly SKRuntimeEffect _shader;
    private static readonly SKRuntimeEffectUniforms _shaderUniforms;

    static ColorSelectPainter()
    {
        _shader = SKRuntimeEffect.CreateShader(@"
            uniform half2 resolution;
            uniform half hue;

            half mod(half a, half b) {
                return a - b * floor(a / b);
            }

            half4 coord_to_rgb(vec2 coord) {
                half h = hue;
                half l = coord.x;
                half s = coord.y;

                half c = (1 - abs(2.0 * l - 1)) * s;
                half x = c * (1 - abs(mod(h / 60, 2.0) - 1));
                half m = l - c / 2.0;

                if (h < 60.0) {
                    return half4(c + m, x + m, m, 1.0);
                } else if (h < 120.0) {
                    return half4(x + m, c + m, m, 1.0);
                } else if (h < 180.0) {
                    return half4(m, c + m, x + m, 1.0);
                } else if (h < 240.0) {
                    return half4(m, x + m, c + m, 1.0);
                } else if (h < 300.0) {
                    return half4(x + m, m, c + m, 1.0);
                } else {
                    return half4(c + m, m, x + m, 1.0);
                }
            }

            half4 main(vec2 coord) {
                half4 rgb = coord_to_rgb(coord / resolution);
                return half4(rgb.r, rgb.g, rgb.b, 1.0);
            }
        ", out var _errorText);

        _shaderUniforms = new(_shader);
    }

    public static void Draw(SKCanvas canvas, HslColor selectedColor)
    {
        canvas.GetLocalClipBounds(out var canvasBounds);
        
        _shaderUniforms["resolution"] = new float[] {canvasBounds.Width, canvasBounds.Height};
        _shaderUniforms["hue"] = (float)selectedColor.H;

        var selectPaint = new SKPaint()
        {
            Shader = _shader.ToShader(uniforms: _shaderUniforms),
        };

        canvas.DrawRect(0, 0, canvasBounds.Width, canvasBounds.Height, selectPaint);

        const float selectedBoxSize = 30;

        var (selectedX, selectedY) = SlXyConverter.SlToCoord(
            (float)selectedColor.S, 
            (float)selectedColor.L, 
            canvasBounds.Width, 
            canvasBounds.Height
        );

        var selectedBoxFill = new SKPaint()
        {
            Color = SKColor.FromHsl(
                (float)selectedColor.H, 
                (float)selectedColor.S, 
                (float)selectedColor.L
            ),
        };

        var selectedBoxStroke = new SKPaint()
        {
            Color = SKColors.White,
            Style = SKPaintStyle.Stroke,
        };

        var selectedBoxBounds = SKRect.Create(
            x: selectedX - selectedBoxSize / 2,
            y: selectedY - selectedBoxSize / 2,
            width: selectedBoxSize,
            height: selectedBoxSize
        );

        canvas.DrawRect(selectedBoxBounds, selectedBoxFill);
        canvas.DrawRect(selectedBoxBounds, selectedBoxStroke);
    }
}

public static class SlXyConverter
{
    public static (float, float) CoordToSl(float x, float y, float width, float height)
    {
        var saturation = (float)Interp.Linear(y, 0.0, height, 0.0, 100.0);
        var lightness = (float)Interp.Linear(x, 0.0, width, 0.0, 100.0);

        return (saturation, lightness);
    }
    
    public static (float, float) SlToCoord(float saturation, float lightness, float width, float height)
    {
        var x = (float)Interp.Linear(lightness, 0, 100, 0, width);
        var y = (float)Interp.Linear(saturation, 0, 100, 0, height);

        return (x, y);
    }
}
