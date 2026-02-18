using SkiaSharp;
using yoksdotnet.logic;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.drawing.painters;

public static class BubblePainter
{
    private static readonly SKRuntimeEffect _shader;
    private static readonly SKRuntimeEffectUniforms _uniforms;

    private static readonly SKPaint _shinePaint = new()
    {
        IsStroke = true,
        Color = SKColors.White,
        StrokeWidth = 5,
        StrokeCap = SKStrokeCap.Round,
    };

    static BubblePainter()
    {
        _shader = SKRuntimeEffect.Create(@"
            uniform half size;
            uniform half2 origin;
            uniform half3 highlight_color;
            uniform half3 shadow_color;

            half4 main(vec2 real_coord) {
                half radius = size / 2;
                half scale = size / 2;

                half2 coord = ((real_coord - origin) - radius) / scale;

                half dist = sqrt(coord.x * coord.x + coord.y * coord.y);
                half score = pow(dist, 8);
                half opacity = score > 1 ? 0 : score;
                opacity = clamp(opacity, 0, 0.8);

                half mix_t = (coord.x + coord.y) / 2;
                half3 color = mix(highlight_color + 0.1, shadow_color, mix_t) * opacity;                

                return half4(color.r, color.g, color.b, opacity);
            }
        ", out var _errorText);

        _uniforms = new(_shader);
    }

    public static void Draw(SKCanvas canvas, PhysicalBasis basis, Skin skin, Bubble bubble)
    {
        var radius = (float)bubble.radius;
        var originX = (float)basis.Final.X - radius;
        var originY = (float)basis.Final.Y - radius;

        _uniforms["size"] = radius * 2.0f;
        _uniforms["origin"] = new float[] { originX, originY };
        _uniforms["highlight_color"] = ColorConversion.ToFloatArray(skin.palette.scales);
        _uniforms["shadow_color"] = ColorConversion.ToFloatArray(skin.palette.scalesShadow);

        var paint = new SKPaint()
        {
            Shader = _shader.ToShader(isOpaque: false, uniforms: _uniforms),
        };

        canvas.DrawRect(originX, originY, radius * 2.0f, radius * 2.0f, paint);
        canvas.DrawArc(new SKRect(originX + 8.0f, originY + 8.0f, originX + radius * 2.0f - 8.0f, originY + radius * 2.0f - 8.0f), 360 * 3 / 5, 20, false, _shinePaint);
    }
}
