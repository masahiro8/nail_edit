using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ObjectLineToArea
{
    public Color[] buffer2;

    [System.NonSerialized] public int textureWidth;
    [System.NonSerialized] public int textureHeight;
    [System.NonSerialized] public Data2[] result = new Data2[0];
    // public float[] resultRange = new float[4];

    private int[,] area;
    private const int max = 5; // スコア順に5個まで
    private int debugPointLength = 30; // デバッグ用の四角の大きさ

    // 検出用データ構造帯
    public class Data1 {
        public int[] v;

        public Data1()
        {

        }

        public Data1(Data2 d, int[,] area)
        {
            // 一辺の分割数
            var num = 8;
            v = new int[num * 4 * 2];

            // 上辺列を足す
            for (int i = 0; i < num; i++) {
                v[(i + num * 0) * 2 + 0] = d.x1 + (d.x2 - d.x1) * i / num;
                v[(i + num * 0) * 2 + 1] = d.y1;
            }
            // 右縦列を足す
            for (int i = 0; i < num; i++) {
                v[(i + num * 1) * 2 + 0] = d.x2;
                v[(i + num * 1) * 2 + 1] = d.y1 + (d.y2 - d.y1) * i / num;
            }
            // 下辺列を足す
            for (int i = 0; i < num; i++) {
                v[(i + num * 2) * 2 + 0] = d.x2 + (d.x1 - d.x2) * i / num;
                v[(i + num * 2) * 2 + 1] = d.y2;
            }
            // 左縦列を足す
            for (int i = 0; i < num; i++) {
                v[(i + num * 3) * 2 + 0] = d.x1;
                v[(i + num * 3) * 2 + 1] = d.y2 + (d.y1 - d.y2) * i / num;
            }

            // 中央
            var cx = (d.x1 + d.x2) / 2;
            var cy = (d.y1 + d.y2) / 2;

            // 存在するところまで近づける
            // 中央までなので凹のような中心が欠けた形状は検出できない
            var div = 8;
            for (int n = 0; n < v.Length; n += 2) {
                // 外側から中央まで徐々に近づき、ドットがある間は繰り返す
                for (int i = 0; i < div; i++) {
                    var rx = cx + (v[n + 0] - cx) * (div - i) / div;
                    var ry = cy + (v[n + 1] - cy) * (div - i) / div;
                    // 空白になったのでやめる
                    if (area[ry, rx] > -1) {
                        v[n + 0] = rx;
                        v[n + 1] = ry;
                        break;
                    }
                }
            }
        }
    }

    // 検出用データ構造帯
    public class Data2 {
        public int idx = 0;
        public bool refs = false;
        public int cnt = 1;
        public int score = 0;
        public int x1 = 0;
        public int y1 = 0;
        public int x2 = 0;
        public int y2 = 0;
        public int textureWidth;
        public int textureHeight;
        public float[] range = null;

        public Data2(int idx, int x, int y, int tw, int th)
        {
            this.idx = idx;
            x1 = x;
            y1 = y;
            x2 = x;
            y2 = y;
            textureWidth = tw;
            textureHeight = th;
        }

        public void AddX(int x)
        {
            cnt++;
            x2 = x;
        }

        public void AddY(int y)
        {
            cnt++;
            y2 = y;
        }

        public void Reference(Data2 dst)
        {
            idx = dst.idx;
            refs = true;
        }

        public void Merge(Data2 dst)
        {
            cnt += dst.cnt;
            x1 = Mathf.Min(x1, dst.x1);
            y1 = Mathf.Min(y1, dst.y1);
            x2 = Mathf.Max(x2, dst.x2);
            y2 = Mathf.Max(y2, dst.y2);
        }

        public float[] Range {
        //     get { return range; }
        //     set { range = value; }
        // }

        // public float[] RangeOrg {
            get {
                var cx = (x1 + x2) / 2;
                var cy = (y1 + y2) / 2;
                var w = (x2 - x1);
                var h = (y2 - y1);
                if (SROptions.Current.NailCombineDetect) {
                    w /= 2;
                    h /= 2;
                }
                
                var rx1 = cx - w;
                var ry1 = cy - h;
                var rx2 = cx + w;
                var ry2 = cy + h;

                // // マージンに踏み込んだ分は削る
                // if (textureWidth < textureHeight) {
                //     var lm = 320 * (textureHeight - textureWidth) / textureHeight / 2;
                //     rx1 = Mathf.Max(lm, Mathf.Min(320 - 1 - lm, rx1));
                //     rx2 = Mathf.Max(lm, Mathf.Min(320 - 1 - lm, rx2));
                // } else {
                //     var lm = 320 * (textureWidth - textureHeight) / textureWidth / 2;
                //     ry1 = Mathf.Max(lm, Mathf.Min(320 - 1 - lm, ry1));
                //     ry2 = Mathf.Max(lm, Mathf.Min(320 - 1 - lm, ry2));
                // }

                return new float[] {
                    (float)rx1 / 320f,
                    (float)ry1 / 320f,
                    (float)rx2 / 320f,
                    (float)ry2 / 320f,
                    // Mathf.Max(0, (float)rx1 / 320f),
                    // Mathf.Max(0, (float)ry1 / 320f),
                    // Mathf.Min(1, (float)rx2 / 320f),
                    // Mathf.Min(1, (float)ry2 / 320f),
                };
            }
        }

        public float Scale {
            get {
                var s = Mathf.Max(x2 - x1, y2 - y1);
                if (SROptions.Current.NailCombineDetect) {
                    s /= 2;
                }
                return (float)s / 320f * 2f;
            }
        }

        public Vector3 Center {
            get {
                return new Vector3(
                    (float)(x1 + x2) / 2f / 320f - 0.5f,
                    (float)(y1 + y2) / 2f / 320f - 0.5f,
                    0);
            }
        }
    }

    public ObjectLineToArea(int width, int height)
    {
        area = new int[width, height];
    }

    // 範囲検出
    public void ToArea2(float[,,] outputs, float[,] scores)
    {
        int len0 = outputs.GetLength(1);
        List<Data2> data2 = new List<Data2>();
        var width = 320; // 256
        var height = 320; // 256

        // エリアを作成
        for (int i = 0; i < len0; i++) {
            // var nn = i / 15;
            // nn = 0;
            // var xx = nn % 8;
            // var yy = nn / 8;
            var x1 = (int)(outputs[0, i, 1] * width);
            var y1 = (int)(outputs[0, i, 0] * height);
            var x2 = (int)(outputs[0, i, 3] * width);
            var y2 = (int)(outputs[0, i, 2] * height);
            // var p = (int)outputs[0, i, 5] * 1000;
            var item = new Data2(i, x1, y1, textureWidth, textureHeight);
            item.AddX(x2);
            item.AddY(y2);
            // item.cnt = p;
            item.score = (int)(scores[0, i] * 10000f);

            // // 縦横を同じにする
            // var lx = x2 - x1;
            // var ly = y2 - y1;
            // if (lx < ly) {
            //     x1 = Mathf.Max(0, x1 - lx / 2);
            //     x2 = Mathf.Min(width - 1, x2 + lx / 2);
            // } else {
            //     y1 = Mathf.Max(0, y1 - ly / 2);
            //     y2 = Mathf.Min(height - 1, y2 + ly / 2);
            // }

            item.x1 = Mathf.Max(0, Mathf.Min(width - 1, item.x1));
            item.y1 = Mathf.Max(0, Mathf.Min(height - 1, item.y1));
            item.x2 = Mathf.Max(0, Mathf.Min(width - 1, item.x2));
            item.y2 = Mathf.Max(0, Mathf.Min(height - 1, item.y2));

            data2.Add(item);
        }

        // 確率順にソートする
        // data2.Sort((v1, v2) => v1.cnt - v2.cnt);
        data2.Sort((v1, v2) => v2.score - v1.score);
        var data3 = data2
            .Where(v => !v.refs && v.x1 != v.x2 && v.y1 != v.y2)
            .Take(max)
            .ToArray();

        // データがあるかどうか
        if (data3.Length == 0) {
            return;
        }

        DebugLine(data3, width, height, true);

        if (SROptions.Current.NailCombineDetect) {
            for (int i = 1; i < data3.Length; i++) {
                // data3[i].Reference(data3[0]);
                data3[0].Merge(data3[i]);
            }
            data3 = data3.Take(1).ToArray();
        }

        DebugLine(data3, width, height, false);

        result = data3;

        // // 5つの要素から範囲を作成
        // resultRange[0] = (float)data3.Select(v => v.x1).Min() / (float)width;
        // resultRange[1] = (float)data3.Select(v => v.y1).Min() / (float)height;
        // resultRange[2] = (float)data3.Select(v => v.x2).Max() / (float)width;
        // resultRange[3] = (float)data3.Select(v => v.y2).Max() / (float)height;

        // {
        //     var nx = i % width;
        //     var ny = i / width;
        //     buffer2[i] = area[ny, nx] > -1 ? colors[data2[area[ny, nx]].idx % colors.Length] : Color.clear;
        // }
    }

    // 範囲検出
    public void ToArea2Fit()
    {
        var item = new Data2(0, 0, 0, textureWidth, textureHeight);
        item.AddX(320);
        item.AddY(320);
        result = new Data2[] {
            item,
        };
    }

    // 範囲検出
    public void ToArea2Fill()
    {
        var m1 = Mathf.Min(textureWidth, textureHeight);
        var m2 = Mathf.Max(textureWidth, textureHeight);
        var m3x = (m2 - m1) / 2;
        var m3y = m3x;
        m3y -= m2 / 8;
        var m4x = m3x + m1;
        var m4y = m3y + m1;
        m3x = m3x * 320 / m2;
        m3y = m3y * 320 / m2;
        m4x = m4x * 320 / m2;
        m4y = m4y * 320 / m2;
        var item = new Data2(0, m3x, m3y, textureWidth, textureHeight);
        item.AddX(m4x);
        item.AddY(m4y);
        result = new Data2[] {
            item,
        };
    }

    private void DebugLine(Data2[] data3, int width, int height, bool clean)
    {
        // クリア
        if (clean) {
            for (int i = 0; i < buffer2.Length; i++) {
                buffer2[i] = Color.clear;
            }
        }

        // デバッグ用のラインを引く
        var max2 = Mathf.Min(data3.Length, max);
        for (int i = 0; i < max2; i++) {
            var d = data3[i];
            // マージされたものは除外
            if (!d.refs) {
                // var c = d.cnt > 100 ? Color.white : Color.blue;
                var c = clean ? Color.green : Color.cyan;
                c.g = d.score;
                // 横線
                if (d.y2 - d.y1 - 1 > 0) {
                    for (int y = d.y1; y <= d.y2 - 1; y += d.y2 - d.y1 - 1) {
                        for (int x = d.x1; x <= d.x2; x++) {
                            buffer2[x + (height - 1 - y) * width] = c;
                            buffer2[x + (height - 1 - (y + 1)) * width] = c;
                        }
                    }
                }
                // 縦線
                if (d.x2 - d.x1 > 0) {
                    for (int y = d.y1; y <= d.y2; y++) {
                        for (int x = d.x1; x <= d.x2 - 1; x += d.x2 - d.x1 - 1) {
                            buffer2[x + (height - 1 - y) * width] = c;
                            buffer2[x + 1 + (height - 1 - y) * width] = c;
                        }
                    }
                }
            }
        }
    }
}
