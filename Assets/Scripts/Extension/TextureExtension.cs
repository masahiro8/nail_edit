using UnityEngine;
using Unity.Mathematics;

public static class TextureExtension
{
    public static float[] AdjustClipping(this Texture tex, float[] range)
    {
        var range2 = new float[range.Length];
        range.CopyTo(range2, 0);

        // 縦横比率で変える
        if (tex.width < tex.height) {
            var s = (float)tex.height / (float)tex.width;
            range2[0] = (range2[0] - 0.5f) * s + 0.5f;
            range2[2] = (range2[2] - 0.5f) * s + 0.5f;
        } else if (tex.width > tex.height) {
            var s = (float)tex.width / (float)tex.height;
            range2[1] = (range2[1] - 0.5f) * s + 0.5f;
            range2[3] = (range2[3] - 0.5f) * s + 0.5f;
        }

        return range2;
    }

    public static Texture2D Clipping2(this Texture tex, float[] range)
    {
        var range2 = new float[range.Length];
        range.CopyTo(range2, 0);

        // 縦横比率で変える
        if (tex.width < tex.height) {
            var s = (float)tex.height / (float)tex.width;
            range2[0] = (range2[0] - 0.5f) * s + 0.5f;
            range2[2] = (range2[2] - 0.5f) * s + 0.5f;
        } else if (tex.width > tex.height) {
            var s = (float)tex.width / (float)tex.height;
            range2[1] = (range2[1] - 0.5f) * s + 0.5f;
            range2[3] = (range2[3] - 0.5f) * s + 0.5f;
        }

        var ox1 = (int)(range2[0] * (float)tex.width);
        var oy1 = (int)(range2[1] * (float)tex.height);
        var ox2 = (int)(range2[2] * (float)tex.width);
        var oy2 = (int)(range2[3] * (float)tex.height);

        var ow = ox2 - ox1;
        var oh = oy2 - oy1;

        if (ow == 0 || oh == 0) {
            return null;
        }

        var x1 = Mathf.Max(0, Mathf.Min(tex.width - 1, ox1));
        var y1 = Mathf.Max(0, Mathf.Min(tex.height - 1, oy1));
        var x2 = Mathf.Max(0, Mathf.Min(tex.width - 1, ox2));
        var y2 = Mathf.Max(0, Mathf.Min(tex.height - 1, oy2));

        var x = x1;
        var y = y1;
        var w = x2 - x1;
        var h = y2 - y1;

        if (w == 0 || h == 0) {
            return null;
            // x = 0;
            // y = 0;
            // w = tex.width;
            // h = tex.height;
        }

        var ax = x1 - ox1;
        var ay = y1 - oy1;
        if (ow < oh) {
            ax += (oh - ow) / 2;
            ow = oh;
        } else {
            ay += (ow - oh) / 2;
            oh = ow;
        }

        var res = new Texture2D(ow, oh);
        var tex2d = tex as Texture2D;
        if (tex2d) {
            // tex2d.SetPixels(tex2d.GetPixels(x, tex.height - h - y, w, h));
            var opixels = tex2d.GetPixels(x, tex.height - h - y, w, h);
            // var opixels = tex2d.GetPixels(0, 0, tex2d.width, tex2d.height);
            var pixels = res.GetPixels(0, 0, ow, oh);
            for (var iy = 0; iy < h; iy++) {
                for (var ix = 0; ix < w; ix++) {
                    if (ix + ax >= 0 && ix + ax < ow && iy + ay >= 0 && iy + ay < oh) {
                        pixels[ix + ax + (iy + ay) * ow] = opixels[ix + iy * w];
                    }
                }
            }
            res.SetPixels(pixels);
            res.Apply();
        }

        return res;
    }

    public static Texture2D Clipping(this Texture tex, float[] range)
    {
        var range2 = new float[range.Length];
        range.CopyTo(range2, 0);

        // 縦横比率で変える
        if (tex.width < tex.height) {
            var s = (float)tex.height / (float)tex.width;
            range2[0] = (range2[0] - 0.5f) * s + 0.5f;
            range2[2] = (range2[2] - 0.5f) * s + 0.5f;
        } else if (tex.width > tex.height) {
            var s = (float)tex.width / (float)tex.height;
            range2[1] = (range2[1] - 0.5f) * s + 0.5f;
            range2[3] = (range2[3] - 0.5f) * s + 0.5f;
        }

        var ox1 = (int)(range2[0] * (float)tex.width);
        var oy1 = (int)(range2[1] * (float)tex.height);
        var ox2 = (int)(range2[2] * (float)tex.width);
        var oy2 = (int)(range2[3] * (float)tex.height);

        var ow = ox2 - ox1;
        var oh = oy2 - oy1;

        if (ow == 0 || oh == 0) {
            return null;
        }

        if (ow < oh) {
            ox1 -= (oh - ow) / 2;
            ow = oh;
        } else {
            oy1 -= (ow - oh) / 2;
            oh = ow;
        }

        var res = new Texture2D(ow, oh);
        var tex2d = tex as Texture2D;
        if (tex2d) {
            var opixels = tex2d.GetPixels(0, 0, tex2d.width, tex2d.height);
            var pixels = res.GetPixels(0, 0, ow, oh);
            for (var iy = 0; iy < oh; iy++) {
                for (var ix = 0; ix < ow; ix++) {
                    var x = ix + ox1;
                    var y = iy + oy1;
                    if (x >= 0 && x < tex2d.width && y >= 0 && y < tex2d.height) {
                        pixels[ix + (oh - 1 - iy) * ow] = opixels[x + (tex2d.height - 1 - y) * tex2d.width];
                    }
                }
            }
            res.SetPixels(pixels);
            res.Apply();
        }

        return res;
    }
}
