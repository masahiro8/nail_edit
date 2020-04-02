using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NailObject : MonoBehaviour
{
    public Texture2D patternTexture = null;
    public Texture2D normalTexture = null;
    public int texWidth = 512; // テクスチャの幅
    public int texHeight = 512; // テクスチャの高さ
    public int seed = 12345; // ランダムの種
    public int count = 100; // 個数

    // テクスチャを更新
    public void UpdateTexture(NailMaterialRecord data)
    {
        // 前回作成されている場合はテクスチャ作り直し
        if (patternTexture != null) {
            Object.Destroy(patternTexture);
        }
        if (normalTexture != null) {
            Object.Destroy(normalTexture);
        }

        // テクスチャ作成
        patternTexture = new Texture2D(texWidth, texHeight);
        var pixels = patternTexture.GetPixels(0, 0, texWidth, texHeight);

        // 法線マップ
        normalTexture = new Texture2D(texWidth, texHeight);
        var normals = normalTexture.GetPixels(0, 0, texWidth, texHeight);

        // 塗りつぶして初期化
        var clearColor = new Color(1, 1, 1, 0);
        // var clearColor = new Color(1, 1, 1, 1);
        var normalClearColor = new Color(0.5f, 0.5f, 1, 0);
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = clearColor;
            normals[i] = normalClearColor;
        }

        // 辺の大きさ
        var minSize = (int)((float)texWidth * data.minSize);
        var maxSize = (int)((float)texWidth * data.maxSize);

        // 乱数の初期化
        Random.InitState(seed);
        for (int i = 0; i < count; i++) {
            var w = Random.Range(minSize, maxSize);
            var h = Random.Range(minSize, maxSize);
            switch (data.textureType) {
                case NailTextureType.Triangle:
                    DrawTriangle(
                        ref pixels,
                        ref normals,
                        Random.Range(0, texWidth - w),
                        Random.Range(0, texHeight - h),
                        w,
                        h);
                    break;
                case NailTextureType.Circle:
                    DrawCircle(
                        ref pixels,
                        ref normals,
                        Random.Range(0, texWidth - w),
                        Random.Range(0, texHeight - w),
                        w);
                    break;
                case NailTextureType.Line:
                    DrawLine(
                        ref pixels,
                        ref normals);
                    break;
            }
        }

        // 完了
        patternTexture.SetPixels(pixels);
        patternTexture.Apply();

        normalTexture.SetPixels(normals);
        normalTexture.Apply();
    }

    private void DrawRect(ref Color[] pixels, ref Color[] normals, int x, int y, int w, int h)
    {
        for (int iy = 0; iy < h; iy++) {
            for (int ix = 0; ix < w; ix++) {
                pixels[(iy + y) * texWidth + ix + x] = Color.white;
            }
        }
    }

    private void DrawTriangle(ref Color[] pixels, ref Color[] normals, int x, int y, int w, int h)
    {
        var r = Mathf.PI * 2 * 30 / 360;
        var normal11 = new Color(
            0.5f - Mathf.Sin(r) * 0.5f,
            0.5f,
            0.5f + Mathf.Cos(r) * 0.5f,
            1);
        var normal12 = new Color(
            0.5f + Mathf.Sin(r) * 0.5f,
            0.5f,
            0.5f + Mathf.Cos(r) * 0.5f,
            1);
        for (int iy = 0; iy < h; iy++) {
            for (int ix = 0; ix < w * (iy + 1) / h; ix++) {
                pixels[(iy + y) * texWidth + ix + x].a = 1;
            }
            normals[(iy + y) * texWidth + x + 0] = normal11;
            // normals[(iy + y) * texWidth + x + 1] = normal11;

            if (iy > 0) {
                normals[(iy + y) * texWidth + x + w * (iy + 1) / h - 1] = normal12;
            }
            // if (iy > 1) {
            //     normals[(iy + y) * texWidth + x + w * (iy + 1) / h - 2] = normal12;
            // }
        }

        var normal2 = new Color(
            0.5f,
            0.5f + Mathf.Sin(r) * 0.5f,
            0.5f + Mathf.Cos(r) * 0.5f,
            1);
        for (int ix = 0; ix < w; ix++) {
            normals[(h - 1 + y) * texWidth + ix + x] = normal2;
            // normals[(h - 2 + y) * texWidth + ix + x] = normal2;
        }
    }

    private void DrawCircle(ref Color[] pixels, ref Color[] normals, int x, int y, int size)
    {
        var c = (float)size / 2;
        var c2 = c * c;
        for (int iy = 0; iy < size; iy++) {
            for (int ix = 0; ix < size; ix++) {
                var fx = (float)ix - c;
                var fy = (float)iy - c;
                var r2 = fx * fx + fy * fy;
                if (r2 <  c2) {
                    var n = (iy + y) * texWidth + ix + x;
                    pixels[n].a = 1;
                    var v = new Vector3(fx, fy, Mathf.Sqrt(size * size - r2));
                    // v.z *= 4;
                    v = v.normalized * 0.5f;
                    normals[n].r = 0.5f + v.x;
                    normals[n].g = 0.5f + v.y;
                    normals[n].b = 0.5f + v.z;
                }
            }
        }
    }

    private void DrawLine(ref Color[] pixels, ref Color[] normals)
    {
        var lineWidth = 2;
        var r = (float)Random.Range(0, 90) / 180 * Mathf.PI;
        var w = (int)(Mathf.Cos(r) * 20) + lineWidth * 2;
        var h = (int)(Mathf.Sin(r) * 20) + lineWidth * 2;
        var x = Random.Range(0, texWidth - w);
        var y = Random.Range(0, texHeight - h);

        for (int iy = 0; iy < h; iy++) {
            for (int ix = 0; ix < w; ix++) {
                var n = (iy + y) * texWidth + ix + x;
                var p = new Vector3(ix, iy);
                var b = new Vector3(w, h);
                var d = Vector3.Distance(p, Vector3.Project(p, b));
                if (d < lineWidth) {
                    pixels[n].a = 1;
                }
                if (d < lineWidth + 1 && d > lineWidth - 1) {
                    var v = new Vector3(p.x - b.x, p.y - b.y, 0);
                    v = v.normalized;
                    v.z = 2;
                    v = v.normalized * 0.5f;
                    normals[n].r = 0.5f + v.x;
                    normals[n].g = 0.5f + v.y;
                    normals[n].b = 0.5f + v.z;
                }
            }
        }
    }
}
