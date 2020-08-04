using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NailRotate
{
    private int degreeRange = 30; // 補正する角度
    private int minCount = 100; // 使用する最小ドット数

    // 回転させる
    public void Rotate(NailDotToArea d2a)
    {
        // サイズが小さいものを消す
        var prevCount = d2a.result.Length;
        // d2a.result = RemoveSmallArea(d2a.result);
        d2a.result = d2a.result.Where(v => v.count > minCount).ToArray();
        Debug.Log("RemoveSmallArea(" + minCount + "): " + prevCount + " -> " + d2a.result.Length);

        // 検出結果がない場合は処理しない
        if (d2a.result.Length == 0) {
            return;
        }

        // 親指の推定
        var allRot = RotateAll(d2a.result);
        // if (DataTable.Param.shootMode.Value == ShootModeType.HandGoo) {
        CheckThumb(d2a.result, allRot);
        // }

        // 親指を外して再計算
        var allRot2 = RotateAll(d2a.result.Where(v => !v.thumb).ToArray());
        allRot = (allRot + allRot2) / 2f;

        // それぞれの角度を求める
        foreach (var d in d2a.result) {
            d.r = CalcArea(d);
        }

        // 補正値の計算（親指を使うので親指のあと）
        foreach (var d in d2a.result) {
            d.ofs = CalcAdjust(d, allRot);
        }

        // 爪の全体の向きから大きく外れる場合に揃える
        if (DataTable.Param.shootMode.Value == ShootModeType.Foot) {
            foreach (var d in d2a.result) {
                CalcAdjust2(d, allRot);
            }
        }

        // その他の指の補正
        // CalcOther(d2a.result);
    }

    // // 小さいものを消す
    // private NailDotToArea.Data1[] RemoveSmallArea(NailDotToArea.Data1[] data)
    // {
    //     return data.Where(v => v.count > minCount).ToArray();
    // }

    // 爪全体での角度を求める
    private float RotateAll(NailDotToArea.Data1[] data)
    {
        // 爪全体での角度を求めるための合成インスタンスを作成
        var res = new NailDotToArea.Data1();
        res.v = new int[data.Length * 2];
        for (var i = 0; i < data.Length; i++) {
            var d = data[i];
            res.v[i * 2 + 0] = (int)d.cx;
            res.v[i * 2 + 1] = (int)d.cy;
        }

        var r = CalcArea(res) - 90f;
        Debug.Log("CalcAllRotate: " + r);
        return r;
    }

    // 親指の推定
    private void CheckThumb(NailDotToArea.Data1[] data, float r)
    {
        // 回転させる
        var q = Quaternion.Euler(0, 0, r);
        var data2 = data.Select(v => {
            Vector2 p = q * new Vector2(v.cx, v.cy);
            // Debug.Log("CalcAllRotate: " + v.cx + "," + v.cy + " " + p);
            return new { Value = v, Y = p.y };
        });

        // Y座標でソートして最下端のものを選択する
        var data3 = data2.OrderByDescending(v => v.Y).ToArray();
        var n = DataTable.Param.shootHand.Value == ShootHandType.Left
            ? 0 // 左手の時は左端
            : data3.Length - 1; // 右手の時は右端
        data3[n].Value.thumb = true;
    }

    // 個別の最長部分を探して角度を確定
    private float CalcArea(NailDotToArea.Data1 data)
    {
        // 頂点の個数が不正
        if (data.v.Length % 2 > 0) {
            return 0;
        }

        var res = 0f;
        var v1 = new Vector2[data.v.Length / 2];
        var v2 = new Vector2[v1.Length];
        for (var i = 0; i < v1.Length; i++) {
            v1[i].x = (float)data.v[i * 2 + 0];
            v1[i].y = (float)data.v[i * 2 + 1];
        }

        var area = 0f;
        for (var i = 0; i < 18 * 2; i++) {
            var r = (float)i * 5f;
            // 座標に角度を加える
            var q = Quaternion.Euler(0, 0, r);
            for (var j = 0; j < v1.Length; j++) {
                v2[j] = q * v1[j];
            }
            var f = CalcRotateArea(v2, r);
            if (i == 0 || f < area) {
                area = f;
                res = r;
            }
        }
        return res;
    }

    // 個別の最長部分を探して角度を確定
    private float CalcRotateArea(Vector2[] v, float r)
    {
        var x1 = 0f;
        var x2 = 0f;
        var y1 = 0f;
        var y2 = 0f;
        for (var i = 0; i < v.Length; i++) {
            if (i == 0) {
                x1 = v[i].x;
                x2 = v[i].x;
                y1 = v[i].y;
                y2 = v[i].y;
            } else {
                x1 = Mathf.Min(x1, v[i].x);
                x2 = Mathf.Max(x2, v[i].x);
                y1 = Mathf.Min(y1, v[i].y);
                y2 = Mathf.Max(y2, v[i].y);
            }
        }
        // return (x2 - x1) * (y2 - y1);
        return x2 - x1;
    }

    // 角度の補正値の計算
    private float CalcAdjust(NailDotToArea.Data1 data, float r)
    {
        var rot90 = false;
        var rotNormal = false;

        // パーの時
        switch (DataTable.Param.shootMode.Value) {
            case ShootModeType.HandPaa:
                rotNormal = true;
                break;
            case ShootModeType.HandGoo:
                rot90 = data.thumb;
                rotNormal = !data.thumb;
                break;
            case ShootModeType.Foot:
                rotNormal = true;
                break;
            case ShootModeType.Free:
            default:
                break;
        }

        var r2 = data.r - r + GetOffset(!rot90);
        var diff = GetDiff(r2);
        Debug.Log("diff: " + data.r + " - " + r + " + " + (rot90 ? 0 : 90) +  " = " + r2 + " -> " + diff);
        if (diff > -degreeRange && data.r < degreeRange) {
            if (rot90) {
                return 90f;
            } else if (rotNormal) {
                return 90f;
            }
        }

        return 0f;
    }

    // 角度の補正値の計算
    private void CalcAdjust2(NailDotToArea.Data1 data, float r)
    {
        var target = r + GetOffset(data.thumb);
        var r2 = data.r + data.ofs - target;
        var diff = GetDiff(r2);
        Debug.Log("diff2: " + data.r + " + " + data.ofs + " - " + r + " - " + (data.thumb ? 90 : 0) +  " = " + r2 + " -> " + diff);
        if (diff < -degreeRange || diff > degreeRange) {
            Debug.Log("    " + data.ofs + " -> " + (target - data.r));
            data.ofs = target - data.r;
        }
    }

    // 角度を揃える
    private void CalcOther(NailDotToArea.Data1[] data)
    {
        var res = 0f;
        var n = 0;
        foreach (var d in data) {
            if (!d.thumb) {
                res += d.r + d.ofs;
                n++;
            }
        }
        if (n == 0) {
            return;
        }
        res /= (float)n;
        foreach (var d in data) {
            if (!d.thumb) {
                d.ofs += res;
            }
        }
    }

    // オフセットの取得
    private float GetOffset(bool b)
    {
        if (DataTable.Param.shootMode.Value == ShootModeType.HandGoo) {
            return b ? 90f : 0f;
        } else {
            return 0f;
        }
    }

    // 角度差を正規化
    private float GetDiff(float r)
    {
        while (r <= -90) {
            r += 180;
        }
        while (r >= 90) {
            r -= 180;
        }
        return r;
    }
}
