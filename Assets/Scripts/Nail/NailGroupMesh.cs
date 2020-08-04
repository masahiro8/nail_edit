using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NailGroupMesh
{
    public Mesh mesh = null;

    // public float edgePer1 = 1.0f;
    // public float edgePer2 = 1.0f;
    public Color edgeColor = Color.clear;

    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Vector3> normals;
    private List<Vector2> uv;
    private List<Color> colors;
    private int spiralNum = 8;

    // メッシュ作成
    public void CreateMesh(NailGroup g, int[] data)
    {
        if (mesh != null) {
            Object.Destroy(mesh);
        }
        // メッシュの初期化
        // if (mesh == null) {
            mesh = new Mesh();
        //     meshFilter.mesh = mesh;
        // }

        vertices = new List<Vector3>();
        triangles = new List<int>();
        normals = new List<Vector3>();
        uv = new List<Vector2>();
        colors = new List<Color>();

        // vertices.Add(Vector3.zero);
        // normals.Add(new Vector3(0, 0, -1));
        // colors.Add(Color.white);
        // uv.Add(Vector2.one / 2);

        // float化
        var data2 = data.Select(v => (float)v).ToArray();
        data2 = RotateVertex(g, data2, g.rz);
        g.ReCalcRect(data2);
        data2 = SmoothVertex(data2);
        // data2 = SmoothVertex(data2);

        // 螺旋状のデータを作る
        var data3 = MakeSpiral(g, data2);

        for (int i = 0; i < spiralNum - 1; i++) {
            CreateFan(g, data3, i);
        }
        // if (edgePer1 < edgePer2) {
        //     CreateEdge(g, data2);
        // }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();
        mesh.colors = colors.ToArray();
    }

    // 中心の円を作る
    private void CreateFan(NailGroup g, Vector3[,,] data, int spn)
    {
        // if (spn == 1) {
        //     return;
        // }
        var m = data.GetLength(2);
        for (int i = 0; i < m; i++) {
            var n1 = i;
            var n2 = (i + 1) % m;
            var s1 = (spn - 1) * m + 1;
            var s2 = (spn + 0) * m + 1;
            var t1 = spn == 0 ? 0 : s1 + n1;
            var t2 = spn == 0 ? 0 : s1 + n2;
            var t3 = s2 + n1;
            var t4 = s2 + n2;

            // 追加
            if (spn > 0) {
                // triangles.Add(n + 0);
                // triangles.Add(n + 1);
                // triangles.Add(n + 2);
                triangles.Add(t1);
                triangles.Add(t3);
                triangles.Add(t2);
            }

            triangles.Add(t2);
            triangles.Add(t3);
            triangles.Add(t4);
            // triangles.Add(n + 1);
            // triangles.Add(n + 3);
            // triangles.Add(n + 2);
        }
    }

    // 回転させる
    private float[] RotateVertex(NailGroup g, float[] data, float rz)
    {
        var q = Quaternion.Euler(0, 0, rz);
        var v = Vector3.zero;
        for (int i = 0; i < data.Length; i += 2) {
            var n1 = i;

            v.x = data[n1 + 0] - g.cx;
            v.y = data[n1 + 1] - g.cy;

            // 回転させる
            v = q * v;
            data[n1 + 0] = v.x + g.cx;
            data[n1 + 1] = v.y + g.cy;
        }
        return data;
    }

    // 滑らかにする
    private float[] SmoothVertex(float[] data)
    {
        // 個数が足りない、あるいは個数が奇数個
        if (data.Length < 3 * 2 || data.Length % 2 > 0) {
            return data;
        }

        var res = new float[data.Length];
        for (int i = 0; i < data.Length; i += 2) {
            var n0 = (i + data.Length - 2) % data.Length;
            var n1 = i;
            var n2 = (i + 2) % data.Length;

            for (int j = 0; j < 2; j++) {
                // 前後の中点を出す
                var c = (data[n2 + j] + data[n0 + j]) / 2;
                // 中点と現在位置の中間を取得
                res[n1 + j] = (data[n1 + j] + c) / 2;
            }
        }
        return res;
    }

    private Vector3[,,] MakeSpiral(NailGroup g, float[] data)
    {
        // 0:座標
        // 1:UV
        // 2:法線
        var res = new Vector3[3, spiralNum, data.Length / 2];
        var minZ = 0f;
        // var nmz = 0.5f * (float)g.texWidth / (float)g.orgTexWidth * g.aspect;
        var nmz = 0.5f * (float)g.texWidth / (float)g.orgTexWidth * g.aspect;
        var nmz2 = nmz * nmz;
        // var pmx = 1.5f;
        // var pmy = 1f / 1.5f;
        var pmx = SROptions.Current.NailMeshRoundX / 100f;
        var pmy = SROptions.Current.NailMeshRoundY / 100f;
        // var pmx = 1.0f;
        // var pmy = 1.5f;
        var ep1 = SROptions.Current.NailEdgeTransparent
            ? (float)SROptions.Current.NailEdge1TransparentPer / 100f
            : 1f;
        var ep2 = SROptions.Current.NailEdgeTransparent
            ? (float)SROptions.Current.NailEdge2TransparentPer / 100f
            : 1f;

        for (var i = 0; i < spiralNum; i++) {
            // 座標への倍率
            var s = 1f;
            if (i < spiralNum - 1) {
                s = (float)i / (float)(spiralNum - 2) * ep1;
            } else {
                s = ep2;
            }
            // s *= 2;
            // g.texWidthは20ぐらい、g.orgTexWidthは700ぐらい
            var mx1 = 1f;
            var my1 = 1f;
            var mx2 = 1f;
            var my2 = 1f;
            if (i > 0) {
                mx2 = s / (float)g.orgTexWidth * g.aspect;
                my2 = s / (float)g.orgTexHeight * -1f;
                mx1 = s / (float)g.texWidth / mx2;
                my1 = s / (float)g.texHeight / my2;
            }
            // var mx3 = 0.5f * (float)g.orgTexWidth / (float)(g.maxX - g.minX);
            // var my3 = 0.5f * (float)g.orgTexHeight / (float)(g.maxY - g.minY);
            // mx3 *= pmx;
            // my3 *= pmy;
            // my3 *= 1.5f;
            for (var j = 0; j < res.GetLength(2); j++) {
                if (i > 0) {
                    var nx = j * 2 + 0;
                    var ny = j * 2 + 1;
                    res[0, i, j].x = (data[nx] - g.cx) * mx2;
                    res[0, i, j].y = (data[ny] - g.cy) * my2;

                    // res[i, j].z = dx * 0.1f;
                    // if (i == spiralNum - 1) {
                    //     Debug.Log(j + ": " + dx + "," + dy + "," + res[i, j].z);
                    // }
                } else {
                    res[0, i, j].x = 0;
                    res[0, i, j].y = 0;
                }

                // zの調整
                var dx = res[0, i, j].x * pmx;
                var dy = res[0, i, j].y * pmy;
                var dd = dx * dx + dy * dy;
                res[0, i, j].z = dd < nmz2 ? -Mathf.Sqrt(nmz2 - dd) : 0f;

                // UV
                res[1, i, j].x = res[0, i, j].x * mx1 + 0.5f;
                res[1, i, j].y = res[0, i, j].y * my1 + 0.5f;
                res[1, i, j].z = 0;

                // 法線
                res[2, i, j] = res[0, i, j];
                res[2, i, j].x *= pmx;
                res[2, i, j].y *= pmy;
                res[2, i, j] = res[2, i, j].normalized;

                if (i == 0 && j == 0) {
                    minZ = res[0, i, j].z;
                } else {
                    minZ = Mathf.Max(minZ, res[0, i, j].z);
                }
            }
        }

        // まとめて処理
        for (var i = 0; i < res.GetLength(1); i++) {
            for (var j = 0; j < res.GetLength(2); j++) {
                // 座標をまとめて下げる
                res[0, i, j].z -= minZ;
                // UVの範囲外を切る
                // res[1, i, j].x = Mathf.Min(1, Mathf.Max(0, res[1, i, j].x));
                // res[1, i, j].y = Mathf.Min(1, Mathf.Max(0, res[1, i, j].y));
                // 繰り返さないとラメの模様が崩れる
            }
        }

        // // デバッグ表示
        // for (var i = 0; i < res.GetLength(1); i++) {
        //     for (var j = 0; j < res.GetLength(2); j++) {
        //         // Debug.Log(i + "," + j + ": " + res[0, i, j].x + "," + res[0, i, j].y + " -> " + res[1, i, j].x + "," + res[1, i, j].y);
        //         Debug.Log(i + "," + j + ": " + res[0, i, j].x + "," + res[0, i, j].y + " -> " + res[1, i, j].x + "," + res[1, i, j].y + " @ " + res[2, i, j].x + "," + res[2, i, j].y + "," + res[2, i, j].z);
        //     }
        // }

        for (var i = 0; i < res.GetLength(1); i++) {
            var max = i == 0 ? 1 : res.GetLength(2);
            for (var j = 0; j < max; j++) {
                vertices.Add(res[0, i, j]);
                normals.Add(res[2, i, j]);
                uv.Add(res[1, i, j]);
                colors.Add(i < res.GetLength(1) - 1 ? Color.white : edgeColor);
            }
        }

        return res;
    }
}
