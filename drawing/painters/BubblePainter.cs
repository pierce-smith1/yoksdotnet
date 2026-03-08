using SkiaSharp;
using System;
using yoksdotnet.common;
using yoksdotnet.data.entities;

namespace yoksdotnet.drawing.painters;

public static class BubblePainter
{
    private static readonly SKRuntimeShaderBuilder _shader;

    private static readonly SKPaint _shinePaint = new()
    {
        IsStroke = true,
        Color = SKColors.White,
        StrokeWidth = 5,
        StrokeCap = SKStrokeCap.Round,
    };

    private static readonly SKPaint _shaderPaint = new();

    private static readonly TimeSpan _fadeTime = TimeSpan.FromMilliseconds(500.0);

    static BubblePainter()
    {
        _shader = SKRuntimeEffect.BuildShader(@"
            uniform half size;
            uniform half2 origin;
            uniform half3 highlight_color;
            uniform half3 shadow_color;
            uniform half final_opacity;

            half4 main(vec2 real_coord) {
                half radius = size / 2;
                half scale = size / 2;

                half2 coord = ((real_coord - origin) - radius) / scale;

                half dist = sqrt(coord.x * coord.x + coord.y * coord.y);
                half score = pow(dist, 8);
                half opacity = (score > 1 ? 0 : score) * final_opacity;
                opacity = clamp(opacity, 0, 0.8);

                half mix_t = (coord.x + coord.y) / 2;
                half3 color = mix(highlight_color + 0.1, shadow_color, mix_t) * opacity;                

                return half4(color.r, color.g, color.b, opacity);
            }
        ");
    }

    public static void Draw(SKCanvas canvas, (Basis, Skin, Bubble) entity)
    {
        var (basis, skin, bubble) = entity;

        var fadeDelta = DateTimeOffset.Now - bubble.LastVisibilityChange;
        var fadeProgress = fadeDelta.TotalMilliseconds / _fadeTime.TotalMilliseconds;

        if (!bubble.IsVisible && fadeProgress > 1.0)
        {
            return;
        }

        var opacity = Interp.Sqrt(fadeProgress, 0.0, 1.0, 0.0, 1.0);

        if (!bubble.IsVisible)
        {
            opacity = 1.0 - opacity;
        }

        if (opacity <= 0.0)
        {
            return;
        }

        var radius = (float)bubble.radius;
        var originX = (float)basis.Final.X - radius;
        var originY = (float)basis.Final.Y - radius;

        _shader.Uniforms["size"] = radius * 2.0f;
        _shader.Uniforms["origin"] = new float[] { originX, originY };
        _shader.Uniforms["highlight_color"] = ColorConversion.ToFloatArray(skin.palette.scales);
        _shader.Uniforms["shadow_color"] = ColorConversion.ToFloatArray(skin.palette.scalesShadow);
        _shader.Uniforms["final_opacity"] = (float)opacity;

        _shaderPaint.Shader = _shader.Build();
        canvas.DrawRect(originX, originY, radius * 2.0f, radius * 2.0f, _shaderPaint);

        _shinePaint.Color = new SKColor(255, 255, 255, (byte)(opacity * 255.0));
        canvas.DrawArc(new SKRect(originX + 8.0f, originY + 8.0f, originX + radius * 2.0f - 8.0f, originY + radius * 2.0f - 8.0f), 360 * 3 / 5, 20, false, _shinePaint);
    }
}
