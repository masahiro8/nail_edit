using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class NailDotToArea
{
    public Color[] buffer;
    public Color[] buffer2;

    [System.NonSerialized] public Data1[] result = new Data1[0];
    [System.NonSerialized] public Data1[] resultZero = new Data1[0];

    private int[,] area;
    private const int max = 10; // 10個まで
    private int debugPointLength = 30; // デバッグ用の四角の大きさ

    // 検出用データ構造帯
    public class Data1 {
        public int[] v;
        public int count;
        public float r = 0;
        public float ofs = 0;
        public bool thumb = false;
        public int cx = 0;
        public int cy = 0;

        public Data1()
        {

        }

        public Data1(Data2 d, int[,] area)
        {
            // 整理用にドット数を受け渡しておく
            count = d.cnt;

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
            cx = (d.x1 + d.x2) / 2;
            cy = (d.y1 + d.y2) / 2;

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

        public float Rotate
        {
            get {
                return r + (SROptions.Current.NailRotateAdjust ? ofs : 0f);
            }
        }
    }

    // 検出用データ構造帯
    public class Data2 {
        public int idx = 0;
        public bool refs = false;
        public int cnt = 1;
        public int x1 = 0;
        public int y1 = 0;
        public int x2 = 0;
        public int y2 = 0;

        public Data2(int idx, int x, int y)
        {
            this.idx = idx;
            x1 = x;
            y1 = y;
            x2 = x;
            y2 = y;
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
    }

    public NailDotToArea(int width, int height)
    {
        area = new int[width, height];
    }

    // デバッグ用にテクスチャへ変換
    public void ToTexture(System.Int32[,] outputs)
    {
        int width = outputs.GetLength(1);
        int height = outputs.GetLength(0);

        // 赤で表示
        for (int i = 0; i < buffer.Length; i++) {
            var nx = i % width;
            var ny = height - 1 - (i / width) % height;
            buffer[i].r = (float)outputs[ny, nx] / 25;
            buffer[i].g = 0;
            buffer[i].b = 0;
        }
    }

    // デバッグ用にテクスチャへ変換
    public void ToTexture(System.Int64[,] outputs)
    {
        int width = outputs.GetLength(1);
        int height = outputs.GetLength(0);

        // 赤で表示
        for (int i = 0; i < buffer.Length; i++) {
            var nx = i % width;
            var ny = height - 1 - (i / width) % height;
            buffer[i].r = (float)outputs[ny, nx] / 25;
            buffer[i].g = 0;
            buffer[i].b = 0;
        }
    }

    // 範囲検出
    public void ToArea()
    {
        int width = area.GetLength(1);
        int height = area.GetLength(0);

        // リセット
        List<Data2> data2 = new List<Data2>();
        for (int i = 0; i < area.GetLength(0); i++) {
            for (int j = 0; j < area.GetLength(1); j++) {
                area[i, j] = -1;
            }
        }

        // 塗りつぶす
        for (int y = 0; y < area.GetLength(0); y++) {
            for (int x = 0; x < area.GetLength(1); x++) {
                var n = x + (height - 1 - y) * width;
                if (buffer[n].r > 0) {
                    var a1 = x > 0 && area[y, x - 1] > -1 ? area[y, x - 1] : -1;
                    var a2 = y > 0 && area[y - 1, x] > -1 ? area[y - 1, x] : -1;
                    if (a2 > -1) {
                        // 直上と同じなので同じ色にする
                        area[y, x] = a2;
                        data2[a2].AddY(y);
                        // 上と前が違う色なので揃える
                        if (a1 > -1 && data2[a1].idx != data2[a2].idx) {
                            data2[a1].Reference(data2[a2]);
                        }
                    } else if (a1 > -1) {
                        // 直前と同じなので同じ色にする
                        area[y, x] = a1;
                        data2[a1].AddX(x);
                        // 上と前が違う色なので揃える
                        if (a2 > -1 && data2[a1].idx != data2[a2].idx) {
                            data2[a2].Reference(data2[a1]);
                        }
                    } else {
                        // 周囲から独立しているので新規の色
                        area[y, x] = data2.Count;
                        data2.Add(new Data2(data2.Count, x, y));
                    }
                }
            }
        }

        // 参照先を一番上まで遡る
        for (int i = 0; i < data2.Count; i++) {
            var n = i;
            while (data2[n].refs) {
                n = data2[n].idx;
                data2[i].idx = n;
            }
        }

        // ドット数をマージする
        for (int i = 0; i < data2.Count; i++) {
            if (data2[i].idx != i) {
                data2[data2[i].idx].Merge(data2[i]);
            }
        }

        // 色分け
        var colors = new Color[] {
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.cyan,
            Color.magenta,
            Color.gray,
            Color.black,
            Color.white,
        };

        // エリアごとに色を変える
        for (int i = 0; i < buffer.Length; i++) {
            var nx = i % width;
            var ny = height - 1 - (i / width) % height;
            // buffer2[i] = area[ny, nx] > -1 ? colors[data2[area[ny, nx]].idx % colors.Length] : Color.clear;
            buffer2[i] = area[ny, nx] > -1 ? colors[data2[area[ny, nx]].idx % colors.Length] : buffer[i];
        }

        // 大きい順にソートする
        data2.Sort((v1, v2) => v2.cnt - v1.cnt);
        var data3 = data2.Where(v => !v.refs && v.x1 != v.x2 && v.y1 != v.y2).ToArray();

        // データがあるかどうか
        if (data3.Length == 0) {
            return;
        }

        CreatePath(data3, width, height);
        DebugLine(data3, width, height);
    }

    // 初期化
    public void Clear()
    {
        result = resultZero;
        if (buffer != null) {
            for (int i = 0; i < buffer.Length; i++) {
                buffer[i] = Color.clear;
                // buffer[i].a = 0;
            }
        }
    }

    // マージ
    public void Merge2(NailDotToArea d2a, float[] ranges)
    {
        int width = area.GetLength(1);
        int height = area.GetLength(0);

        var ranges2 = new float[ranges.Length];
        ranges.CopyTo(ranges2, 0);

        // 縦横比率で変える
        var fwidth = ranges2[2] - ranges2[0];
        var fheight = ranges2[3] - ranges2[1];
        if (fwidth < fheight) {
            var s = fheight / fwidth;
            ranges2[0] = (ranges2[0] - 0.5f) * s + 0.5f;
            ranges2[2] = (ranges2[2] - 0.5f) * s + 0.5f;
        } else if (fwidth > fheight) {
            var s = fwidth / fheight;
            ranges2[1] = (ranges2[1] - 0.5f) * s + 0.5f;
            ranges2[3] = (ranges2[3] - 0.5f) * s + 0.5f;
        }

        var rx1 = (int)(ranges2[0] * (float)width);
        var ry1 = (int)(ranges2[1] * (float)height);
        var rx2 = (int)(ranges2[2] * (float)width);
        var ry2 = (int)(ranges2[3] * (float)height);

        rx1 = Mathf.Max(0, Mathf.Min(width - 1, rx1));
        ry1 = Mathf.Max(0, Mathf.Min(height - 1, ry1));
        rx2 = Mathf.Max(0, Mathf.Min(width - 1, rx2));
        ry2 = Mathf.Max(0, Mathf.Min(height - 1, ry2));

        // var rx1 = (int)(ranges[0] * (float)width);
        // var ry1 = (int)(ranges[1] * (float)height);
        // var rx2 = (int)(ranges[2] * (float)width);
        // var ry2 = (int)(ranges[3] * (float)height);

        var rw = rx2 - rx1;
        var rh = ry2 - ry1;
        var ax = rw;
        var ay = rh;
        if (ax < ay) {
            ax = (ay - ax) * width / ay;
            ay = 0;
        } else {
            ay = (ax - ay) * height / ax;
            ax = 0;
        }
        ax = 0;
        ay = 0;

        for (int y = 0; y < rh; y++) {
            for (int x = 0; x < rw; x++) {
                var x2 = x * width / rw;
                var y2 = y * height / rh;
                if (ax > 0) {
                    x2 = (x2 + ax) * rw / rh;
                }
                if (ay > 0) {
                    y2 = (y2 + ay) * rh / rw;
                }
                // var x2 = x + rx1 + ax*0;
                // var y2 = y + ry1 + ay*0;
                var n1 = x + rx1 + (height - 1 - (y + ry1)) * width;
                var n2 = x2 + (height - 1 - y2) * width;
                n1 = Mathf.Max(0, Mathf.Min(buffer.Length - 1, n1));
                n2 = Mathf.Max(0, Mathf.Min(d2a.buffer.Length - 1, n2));

                // 赤いところだけコピー
                // if (d2a.buffer[n2].r > 0) {
                    buffer[n1] = d2a.buffer[n2];
                // }
            }
        }

        // for (int i = 0; i < buffer.Length; i++) {
        //     var nx = i % width;
        //     var ny = height - 1 - (i / width) % height;
        //     if (d2a.buffer[i].r > 0) {
        //         buffer[i] = d2a.buffer[i];
        //     }
        // }
    }

    // マージ
    public void Merge(NailDotToArea d2a, float[] ranges)
    {
        int width = area.GetLength(1);
        int height = area.GetLength(0);

        var ranges2 = new float[ranges.Length];
        ranges.CopyTo(ranges2, 0);

        // 縦横比率で変える
        var fwidth = ranges2[2] - ranges2[0];
        var fheight = ranges2[3] - ranges2[1];
        if (fwidth < fheight) {
            var s = fheight / fwidth;
            var c = (ranges2[2] + ranges2[0]) / 2f;
            ranges2[0] = (ranges2[0] - c) * s + c;
            ranges2[2] = (ranges2[2] - c) * s + c;
        } else if (fwidth > fheight) {
            var s = fwidth / fheight;
            var c = (ranges2[3] + ranges2[1]) / 2f;
            ranges2[1] = (ranges2[1] - c) * s + c;
            ranges2[3] = (ranges2[3] - c) * s + c;
        }

        var rx1 = (int)(ranges2[0] * (float)width);
        var ry1 = (int)(ranges2[1] * (float)height);
        var rx2 = (int)(ranges2[2] * (float)width);
        var ry2 = (int)(ranges2[3] * (float)height);

        var rw = rx2 - rx1;
        var rh = ry2 - ry1;

        for (int y = 0; y < rh; y++) {
            for (int x = 0; x < rw; x++) {
                var x1 = x + rx1;
                var y1 = y + ry1;
                var x2 = x * width / rw;
                var y2 = y * height / rh;
                var n1 = x1 + (height - 1 - y1) * width;
                var n2 = x2 + (height - 1 - y2) * width;
                if (x1 >= 0 && x1 < width && y1 >= 0 && y1 < height
                    && x2 >= 0 && x2 < width && y2 >= 0 && y2 < height) {
                    n1 = Mathf.Max(0, Mathf.Min(buffer.Length - 1, n1));
                    n2 = Mathf.Max(0, Mathf.Min(d2a.buffer.Length - 1, n2));

                    // 赤いところだけコピー
                    if (d2a.buffer[n2].r > 0) {
                        buffer[n1] = d2a.buffer[n2];
                    }
                }
            }
        }

        // for (int i = 0; i < buffer.Length; i++) {
        //     var nx = i % width;
        //     var ny = height - 1 - (i / width) % height;
        //     if (d2a.buffer[i].r > 0) {
        //         buffer[i] = d2a.buffer[i];
        //     }
        // }
    }

    // // デバッグ用にテクスチャへ変換
    // public void ToTexture2(float[,] outputs)
    // {
    //     int width = outputs.GetLength(1);
    //     int height = outputs.GetLength(0);

    //     // 赤で表示
    //     for (int i = 0; i < buffer.Length; i++) {
    //         // var nx = i % width;
    //         // var ny = i / height;
    //         buffer[i].r = (float)outputs[i % width, 5] * 255;
    //         buffer[i].g = 0;
    //         buffer[i].b = 0;
    //     }
    // }

    // // 範囲検出
    // public void ToArea2(float[,] outputs)
    // {
    //     int len0 = outputs.GetLength(0);
    //     List<Data2> data2 = new List<Data2>();
    //     var height = 416; // 256

    //     // エリアを作成
    //     for (int i = 0; i < len0; i++) {
    //         if (outputs[i, 5] < 1) {
    //             var nn = i / 15;
    //             nn = 0;
    //             var xx = nn % 8;
    //             var yy = nn / 8;
    //             var x = (int)outputs[i, 0] + xx * 32;
    //             var y = (int)outputs[i, 1] + yy * 32;
    //             var w = (int)outputs[i, 2];
    //             var h = (int)outputs[i, 3];
    //             var p = (int)outputs[i, 5] * 1000;
    //             var item = new Data2(i, x, y);
    //             item.AddX(x + w);
    //             item.AddY(y + h);
    //             item.cnt = p;

    //             item.x1 = Mathf.Max(0, Mathf.Min(height - 1, item.x1));
    //             item.y1 = Mathf.Max(0, Mathf.Min(height - 1, item.y1));
    //             item.x2 = Mathf.Max(0, Mathf.Min(height - 1, item.x2));
    //             item.y2 = Mathf.Max(0, Mathf.Min(height - 1, item.y2));

    //             data2.Add(item);
    //         }
    //     }

    //     // 確率順にソートする
    //     // data2.Sort((v1, v2) => v2.cnt - v1.cnt);
    //     data2.Sort((v1, v2) => v1.cnt - v2.cnt);
    //     var data3 = data2
    //         .Where(v => !v.refs && v.x1 != v.x2 && v.y1 != v.y2)
    //         // .Take(10)
    //         .ToArray();

    //     // データがあるかどうか
    //     if (data3.Length == 0) {
    //         return;
    //     }

    //     DebugLine(data3, height);

    //     // {
    //     //     var nx = i % width;
    //     //     var ny = i / width;
    //     //     buffer2[i] = area[ny, nx] > -1 ? colors[data2[area[ny, nx]].idx % colors.Length] : Color.clear;
    //     // }
    // }

    private void CreatePath(Data2[] data3, int width, int height)
    {
        // データ生成
        var max2 = Mathf.Min(data3.Length, max);
        var cornerNum = SROptions.Current.DispNailMeshCorner ? 3 : 0;

        result = new Data1[max2 + cornerNum];
        for (int i = 0; i < max2; i++) {
            result[i] = new Data1(data3[i], area);
        }

        // デバッグ用、4角の確認
        for (int i = 0; i < cornerNum; i++) {
            var x = width / 2;
            var y = (height - debugPointLength * 2 - 1) * i / (cornerNum - 1) + debugPointLength;
            var d = new Data1();
            d.v = new int[] {
                x,
                y - debugPointLength,
                x + debugPointLength,
                y,
                x,
                y + debugPointLength,
                x - debugPointLength,
                y,
            };
            result[i + max2] = d;
        }
    }

    private void DebugLine(Data2[] data3, int width, int height)
    {
        // デバッグ用のラインを引く
        var max2 = Mathf.Min(data3.Length, max);
        for (int i = 0; i < max2; i++) {
            var d = data3[i];
            // マージされたものは除外
            if (!d.refs) {
                var c = d.cnt > 100 ? Color.white : Color.blue;
                // 横線
                if (d.y2 > d.y1) {
                    for (int y = d.y1; y <= d.y2; y += d.y2 - d.y1) {
                        for (int x = d.x1; x <= d.x2; x++) {
                            buffer2[x + (height - 1 - y) * width] = c;
                        }
                    }
                }
                // 縦線
                if (d.x2 > d.x1) {
                    for (int y = d.y1; y <= d.y2; y++) {
                        for (int x = d.x1; x <= d.x2; x += d.x2 - d.x1) {
                            buffer2[x + (height - 1 - y) * width] = c;
                        }
                    }
                }
            }
        }
    }

    // public void DebugLog2(float[,] outputs)
    // {
    //     int len0 = outputs.GetLength(0);
    //     List<Data2> data2 = new List<Data2>();

    //     // エリアを作成
    //     var res = new Dictionary<string, int>();
    //     for (int i = 0; i < len0; i++) {
    //         var n = (int)(outputs[i, 5] * 10);
    //         var key = n.ToString();
    //         if (!res.ContainsKey(key)) {
    //             res[key] = 0;
    //         }
    //         res[key]++;
    //     }

    //     var res2 = res.Select(v => v.Key + ":" + v.Value).ToArray();
    //     Debug.Log(string.Join(",", res));
    // }
}
