using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailTexture : MonoBehaviour
{
    public Texture2D patternTexture = null;
    public Texture2D normalTexture = null;
    public int texWidth = 512; // テクスチャの幅
    public int texHeight = 512; // テクスチャの高さ

    private NailMaterialRecord backupRecord = new NailMaterialRecord();

    // テクスチャを更新
    public void UpdateTexture(NailMaterialRecord data)
    {
        // テクスチャ用パラメータが同じ場合は更新しない
        if (backupRecord.IsSameTexture(data)) {
            return;
        }
        // バックアップ
        backupRecord.CopyTexture(data);

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
        switch (data.textureType) {
            case NailTextureType.Streaks:
                clearColor.a = 1;
                break;
            default:
                break;
        }
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
        Random.InitState(data.randomSeed);
        for (int i = 0; i < data.randomCount; i++) {
            var size = Random.Range(minSize, maxSize);
            var aspect = Random.Range(minSize, maxSize);
            var r = (float)Random.Range(0, 360) / 180 * Mathf.PI;
            var x = Random.Range(0, texWidth - size);
            var y = Random.Range(0, texHeight - size);
            switch (data.textureType) {
                case NailTextureType.Triangle:
                    DrawTriangle(
                        ref pixels,
                        ref normals,
                        x, y, size, r);
                    break;
                case NailTextureType.Circle:
                    DrawCircle(
                        ref pixels,
                        ref normals,
                        x, y, size);
                    break;
                case NailTextureType.Line:
                    DrawLine(
                        ref pixels,
                        ref normals,
                        x, y, size, r);
                    break;
                case NailTextureType.Streaks:
                    DrawStreaks(
                        ref pixels,
                        ref normals,
                        x);
                    break;
                default:
                    break;
            }
        }

        // 完了
        patternTexture.SetPixels(pixels);
        patternTexture.Apply();

        normalTexture.SetPixels(normals);
        normalTexture.Apply();
    }

    private void DrawRect(ref Color[] pixels, ref Color[] normals, int x, int y, int size)
    {
        for (int iy = 0; iy < size; iy++) {
            for (int ix = 0; ix < size; ix++) {
                pixels[(iy + y) * texWidth + ix + x] = Color.white;
            }
        }
    }

    private void DrawTriangle(ref Color[] pixels, ref Color[] normals, int x, int y, int size, float r)
    {
        var c = (float)size / 2;
        var c2 = c * c;

        // var lineWidth = 8;
        var l1 = Random.Range(0f, 1f);
        var l2 = Random.Range(0.5f, 1f) * c;

        // 中心の線の法線
        var nv1 = new Vector3(Mathf.Cos(r), Mathf.Sin(r), 0);
        var nv2 = nv1.Rotate270(); // -90度で法線化

        // 座標
        var p0 = new Vector3(c, c, 0);
        var pr = nv1 * c;
        var p1 = p0 - pr;
        var p2 = p0 + pr;

        // 三角形の頂点
        var t1 = p1 + nv1 * l1 + nv2 * l2;

        // 法線2
        var nv3 = (t1 - p1).normalized.Rotate270();
        var nv4 = (p2 - t1).normalized.Rotate270();

        for (int iy = 0; iy < size; iy++) {
            for (int ix = 0; ix < size; ix++) {
                var n = (iy + y) * texWidth + ix + x;
                var p = new Vector3(ix, iy, 0); // 現在の点
                var q = Vector3.Project(p - p1, p2 - p1) + p1; // 垂線の交点
                var d1 = Vector3.Distance(p, q); // 線分との距離
                var d2 = Vector3.Distance(p0, q); // 中心から垂線までの距離
                var d3 = Vector3.Distance(p1, q) / (c * 2); // 線分上の割合
                var cr = Vector3.Cross(p - p1, p2 - p1); // 線分のどちらにいるかの判定用
                var v = Vector3.zero; // 法線用

                var l = 0f;
                if (d2 > c) {
                    // 線分の範囲外
                    l = 0;
                } else if (cr.z < 0) {
                    // 反対側
                    l = 0;
                    v = -nv2;
                } else if (d3 < 0) {
                    l = 0;
                } else if (d3 < l1 && l1 > 0) {
                    l = l2 * d3 / l1;
                    v = nv3;
                } else if (d3 < 1 && l1 < 1) {
                    l = l2 * (1 - d3) / (1 - l1);
                    v = nv4;
                } else {
                    l = 0;
                }

                if (d1 < l) {
                    pixels[n].a = 1;
                }
                if (d1 < l + 1 && d1 > l - 1 && v != Vector3.zero) {
                    // var v = new Vector3(p.x - p0.x, p.y - p0.y, 0);
                    // v = v.normalized;
                    // v.z = 2;
                    // v = v.normalized * 0.5f;
                    normals[n].r = 0.5f + v.x;
                    normals[n].g = 0.5f + v.y;
                    normals[n].b = 0.5f + v.z;
                } else {
                    normals[n].r = 0.5f;
                    normals[n].g = 0.5f;
                    normals[n].b = 1;
                }
            }
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
                    var v = new Vector3(fx, fy, Mathf.Sqrt(c2 - r2));
                    // v.z *= 4;
                    v = v.normalized * 0.5f;
                    normals[n].r = 0.5f + v.x;
                    normals[n].g = 0.5f + v.y;
                    normals[n].b = 0.5f + v.z;
                }
            }
        }
    }

    private void DrawLine(ref Color[] pixels, ref Color[] normals, int x, int y, int size, float r)
    {
        var c = (float)size / 2;
        var c2 = c * c;

        var lineWidth = 3f;
        var l1 = Random.Range(0f, 1f);
        var l2 = Random.Range(0.5f, 1f) * c;

        // 中心の線の法線
        var nv1 = new Vector3(Mathf.Cos(r), Mathf.Sin(r), 0);
        var nv2 = nv1.Rotate270(); // -90度で法線化

        // 座標
        var p0 = new Vector3(c, c, 0);
        var pr = nv1 * c;
        var p1 = p0 - pr;
        var p2 = p0 + pr;

        // 三角形の頂点
        var t1 = p1 + nv1 * l1 + nv2 * l2;

        // 法線2
        var nv3 = (t1 - p1).normalized.Rotate270();
        var nv4 = (p2 - t1).normalized.Rotate270();

        for (int iy = 0; iy < size; iy++) {
            for (int ix = 0; ix < size; ix++) {
                var n = (iy + y) * texWidth + ix + x;
                var p = new Vector3(ix, iy, 0); // 現在の点
                var q = Vector3.Project(p - p1, p2 - p1) + p1; // 垂線の交点
                var d1 = Vector3.Distance(p, q); // 線分との距離
                var d2 = Vector3.Distance(p0, q); // 中心から垂線までの距離
                var d3 = Vector3.Distance(p1, q) / (c * 2); // 線分上の割合
                var cr = Vector3.Cross(p - p1, p2 - p1); // 線分のどちらにいるかの判定用
                var v = Vector3.zero; // 法線用

                var l = lineWidth;
                if (d2 > c) {
                    // 線分の範囲外
                    l = 0;
                } else if (cr.z < 0) {
                    // 反対側
                    v = -nv2;
                } else {
                    v = nv2;
                }

                if (d1 < l) {
                    pixels[n].a = 1;
                }
                if (d1 < l + 1 && d1 > l - 1 && v != Vector3.zero) {
                    // var v = new Vector3(p.x - p0.x, p.y - p0.y, 0);
                    // v = v.normalized;
                    // v.z = 2;
                    // v = v.normalized * 0.5f;
                    normals[n].r = 0.5f + v.x;
                    normals[n].g = 0.5f + v.y;
                    normals[n].b = 0.5f + v.z;
                } else {
                    normals[n].r = 0.5f;
                    normals[n].g = 0.5f;
                    normals[n].b = 1;
                }
            }
        }
    }

    private void DrawStreaks(ref Color[] pixels, ref Color[] normals, int x)
    {
        // 中心の線の法線
        var r = (float)0 / 180 * Mathf.PI;
        var nv1 = new Vector3(Mathf.Cos(r), Mathf.Sin(r), 0);
        var nv2 = -nv1;

        for (int iy = 0; iy < texHeight; iy++) {
            var n1 = (iy + 0) * texWidth + x;
            var n2 = n1 + 1;
            normals[n1].r = 0.5f + nv1.x;
            normals[n1].g = 0.5f + nv1.y;
            normals[n1].b = 0.5f + nv1.z;
            normals[n2].r = 0.5f + nv2.x;
            normals[n2].g = 0.5f + nv2.y;
            normals[n2].b = 0.5f + nv2.z;
        }
    }
}

public static partial class EnumExtensions
{
    // スワイプの方向を補正
    public static Vector3 Rotate270(this Vector3 v) {
        return new Vector3(v.y, -v.x, 0);
    }
}