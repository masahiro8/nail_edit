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

    private const int max = 5; // 10個まで
    private int[,] area;

    // 検出用データ構造帯
    public class Data1 {
        public int[] v;

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

        // var res = new Dictionary<string, int>();
        // for (int i = 0; i < outputs.GetLength(0); i++) {
        //     for (int j = 0; j < outputs.GetLength(1); j++) {
        //         var key = outputs[i, j].ToString();
        //         if (!res.ContainsKey(key)) {
        //             res[key] = 0;
        //         }
        //         res[key]++;
        //     }
        // }

        // 赤で表示
        for (int i = 0; i < buffer.Length; i++) {
            var nx = i % width;
            var ny = i / height;
            buffer[i].r = (float)outputs[ny, nx] / 25;
            buffer[i].g = 0;
            buffer[i].b = 0;
        }
    }

    // 範囲検出
    public void ToArea(System.Int32[,] outputs)
    {
        int width = outputs.GetLength(1);
        int height = outputs.GetLength(0);

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
                if (outputs[y, x] > 0) {
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
            var ny = i / width;
            buffer2[i] = area[ny, nx] > -1 ? colors[data2[area[ny, nx]].idx % colors.Length] : Color.clear;
        }

        // 大きい順にソートする
        data2.Sort((v1, v2) => v2.cnt - v1.cnt);
        var data3 = data2.Where(v => !v.refs && v.x1 != v.x2 && v.y1 != v.y2).ToArray();

        // データがあるかどうか
        if (data3.Length == 0) {
            return;
        }

        CreatePath(data3, height);
        DebugLine(data3, height);
    }

    private void CreatePath(Data2[] data3, int height)
    {
        // データ生成
        var max2 = Mathf.Min(data3.Length, max);
        result = new Data1[max2];
        for (int i = 0; i < max2; i++) {
            result[i] = new Data1(data3[i], area);
        }
    }

    private void DebugLine(Data2[] data3, int height)
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
                            buffer2[x + y * height] = c;
                        }
                    }
                }
                // 縦線
                if (d.x2 > d.x1) {
                    for (int y = d.y1; y <= d.y2; y++) {
                        for (int x = d.x1; x <= d.x2; x += d.x2 - d.x1) {
                            buffer2[x + y * height] = c;
                        }
                    }
                }
            }
        }
    }
}
